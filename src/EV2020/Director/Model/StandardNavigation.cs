using BlueWave.Interop.Asio;
using EV2020.LocationSystem;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The standard navigation class. Base of the "2D convergence" and target bearing model
	/// </summary>
	public class StandardNavigation : INavigator
	{
		bool _paused = false;
		Localizer localizer;
		Vector<double> Target;
		Vector<double> Position;
		IModelND model;
		PlotWindow pw;
		BackgroundWorker bw_generate = new BackgroundWorker();
		double bearing;
		int _iter = 0;
		long lastTimestamp = 0;

		object _localizerLock = new object();
		public Vector<double> CarPos
		{
			get { return Position; }
		}		
		public double CarX
		{
			get { return Position[0]; }
		}
		public double CarY
		{
			get { return Position[1]; }
		}
		public double TargetX
		{
			get { return Target[0]; }
		}
		public double TargetY
		{
			get { return Target[1]; }
		}
		public double CarBearing
		{
			get { return bearing; }
		}
		public StandardNavigation()
		{
			//constructor	
			model = new PDEpicModel();
			model.Init(0.1,1);
			Position = DenseVector.OfArray(new double[] { 0.1, 1 });
			Target = DenseVector.OfArray(new double[] { 0.1, 1 });
			bearing = 0;
		}
		~StandardNavigation()
		{
			if (pw != null)
			{
				pw.Close();
			}
		}
		[STAThread]
		public void Init() {
			lock (_localizerLock)
			{
				localizer = new Localizer(Data.cfg.Microphones.ToList(), Data.cfg.FieldWidth, Data.cfg.FieldHeight, Data.cfg.FieldMargin, Data.cfg.SampleWindow, Data.cfg.SampleLength, Data.cfg.BeaconHeight, Data.cfg.MatchedFilterEnabled, Data.cfg.MatchedFilterToep);
			}
			bw_generate.DoWork += bw_generate_DoWork;
			bw_generate.WorkerReportsProgress = true;
			bw_generate.ProgressChanged += bw_generate_ProgressChanged;
			bw_generate.RunWorkerCompleted += bw_generate_RunWorkerCompleted;
			bw_generate.RunWorkerAsync();
		}
		[STAThread]
		public CarCommand Tick(double LeftSensor, double RightSensor)
		{
			CarCommand c = new CarCommand(0, 0);
			if (_paused)
				return c;
			Vector<double> output = model.Tick(Position * 100, Target * 100);
			Data.db.UpdateProperty("ModelDebugInfo");
			//double modelbearing = output.Angle();
			Vector<double> targetd = Target - Position;
			double targetbearing = targetd.Angle();
			double bearingd = targetbearing - bearing;
			//FIXED after demo
			if (bearingd > 0)
			{
				c.Steering = -50;
			}
			else if (bearingd < 0)
			{
				c.Steering = 50;
			}
			/*if (bearingd > Math.PI / 2 || bearingd < Math.PI)
			{
				bearingd = targetbearing - (bearing-Math.PI);
				Debug.WriteLine("Going backwards.");
				c.Driving = -output.Magnitude();
				if (bearingd > 0)
				{
					c.Steering = 50;
				}
				else if (bearingd < 0)
				{
					c.Steering = -50;
				}
			}
			else
			{*/
				c.Driving = output.Magnitude();
			//}
			UpdateField();
			return c;
		}		
		
		public string GetDebugInfo() {
			return model.GetDebugInfo() + "\n" + Position.ToString() + "\n" + Target.ToString() + "\n" + CarBearing.ToString();
		}
		public void GoToPosition(Vector<double> pos) {
			if (pos.Count != 2)
			{
				throw new ArgumentException("Position needs to be a 2D vector.");
			}
			if (pos[0] < 0||pos[1]<0||pos[0]>Data.cfg.FieldWidth||pos[1]>Data.cfg.FieldHeight)
			{
				throw new ArgumentOutOfRangeException("pos",pos,"Position is outside the field.");
			}
			Target = pos;
			UpdateField();
		}
		public bool Pause() {
			if (_paused)
			{
				return false;
			}
			_paused = true;
			return true;
		}
		public bool Resume() {
			if (!_paused)
			{
				return false;
			}
			_paused = false;
			return true;
		}
		[STAThread]
		void localizer_OnLocationUpdated(object sender, LocationUpdatedEventArgs e)
		{
			if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
			{
				lock (_localizerLock)
				{
					if (localizer != null)
					{
						if (localizer.lastData != null)
						{
							List<string> legend = new List<string>();
							for (int i = 0; i < localizer.lastData.ColumnCount; i++)
							{
								legend.Add(String.Format("Channel {0}", i + 1));
							}
							if (pw != null)
							{
								if (pw.IsLoaded)
									pw.Update("Data", localizer.lastData, legend, ASIO.T, localizer.lastMaxes);
							}
							else
							{
								pw = new PlotWindow("Data", localizer.lastData, legend, ASIO.T, localizer.lastMaxes);
								pw.Show();
							}

						}
					}
				}
			}
			else
			{
				Debug.WriteLine("Could not display PlotWindow due to ThreadApartment");
			}
			//Debug.WriteLine(e.Position);
			//Position3D pos = new Position3D() { X = (lpos.X * 2 + e.Position.X) / 3, Y = (lpos.Y * 2 + e.Position.Y) / 3, Z = 0 };
			//posistionLog.AppendText(pos.ToString() + "\n");
			Vector<double> lastPosition = Position;
			Position = (Position * Data.cfg.SmoothFactor + DenseVector.OfArray(new double[] { e.Position.X, e.Position.Y })) / (1 + Data.cfg.SmoothFactor);
			Vector<double> difference = Position - lastPosition;
			if (difference.Magnitude() > 0.01)
			{
				bearing = difference.Angle();
			}
			
			//posistionLog.ScrollToEnd();
			UpdateField();
		}
		[STAThread]
		private void UpdateField(){
			/*if (Data.vis != null && _iter % 10 == 0)
			{*/
				//Debug.WriteLine("Drawing field: {0} ms ago was last time", (DateTime.Now.Ticks - (double)lastTimestamp)/TimeSpan.TicksPerMillisecond);
				Data.vis.drawField();
				//lastTimestamp = DateTime.Now.Ticks;
			/*	_iter++;
				if (_iter > 10)
				{
					_iter = 0;
				}
			}*/
		}		
		#region BackgroundWorker
		[STAThread]
		void bw_generate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lock (_localizerLock)
			{
				localizer.InitializeDriver(AsioDriver.InstalledDrivers.Find(Data.cfg.SelectedDriver));
				localizer.OnLocationUpdated += localizer_OnLocationUpdated;
			}
		}
		[STAThread]
		void bw_generate_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}
		[STAThread]
		void bw_generate_DoWork(object sender, DoWorkEventArgs e)
		{
			lock (_localizerLock)
			{
				localizer.GenerateFilterMatrix(Data.cfg.UseMeasuredSignal);
			}
		}
		#endregion
	}
}
