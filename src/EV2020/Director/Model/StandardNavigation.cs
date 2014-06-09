using EV2020.LocationSystem;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The standard navigation class. (a dud)
	/// </summary>
	public class StandardNavigation : INavigator
	{
		Localizer localizer;
		Matrix<double> Target;
		Matrix<double> Position;
		IModelND model;
		PlotWindow pw;
		BackgroundWorker bw_generate = new BackgroundWorker();
		double angle;
		public StandardNavigation()
		{
			//constructor	
			model = new PDEpicModel();
			model.Init(0.50,0.50);
			Target = DenseMatrix.OfArray(new double[,] { {.50}, {.50} });
		}
		~StandardNavigation()
		{
			if (pw != null)
			{
				pw.Close();
			}
		}

		public void Init() {  }
		public CarCommand Tick(double LeftSensor, double RightSensor) {
			CarCommand c = new CarCommand(0, 0);
			Matrix<double> output = model.Tick(Position*100, Target*100);
			Data.db.UpdateProperty("ModelDebugInfo");
			//return output;
			return c;
		}
		public string GetDebugInfo() {
			return model.GetDebugInfo();
		}
		public bool GoToPosition(Matrix<double> pos) { throw new NotImplementedException(); }
		public bool Pause() { throw new NotImplementedException(); }
		public bool Resume() { throw new NotImplementedException(); }

		void localizer_OnLocationUpdated(object sender, LocationUpdatedEventArgs e)
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
			//Debug.WriteLine(e.Position);
			//Position3D pos = new Position3D() { X = (lpos.X * 2 + e.Position.X) / 3, Y = (lpos.Y * 2 + e.Position.Y) / 3, Z = 0 };
			//posistionLog.AppendText(pos.ToString() + "\n");
			Position = DenseMatrix.OfArray(new double[,] { { (Position[0, 0] * Data.cfg.SmoothFactor + e.Position.X) / (Data.cfg.SmoothFactor+1) },
			{ (Position[1, 0] * Data.cfg.SmoothFactor + e.Position.Y) / (Data.cfg.SmoothFactor+1) } });			
			//posistionLog.ScrollToEnd();
		}
		
		#region BackgroundWorker
		[STAThread]
		void bw_generate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			localizer.InitializeDriver();
			localizer.OnLocationUpdated += localizer_OnLocationUpdated;
		}
		[STAThread]
		void bw_generate_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		void bw_generate_DoWork(object sender, DoWorkEventArgs e)
		{			
			localizer = new Localizer(Data.cfg.Microphones.ToList(), (int)e.Argument, Data.cfg.FieldWidth, Data.cfg.FieldHeight, Data.cfg.FieldMargin, Data.cfg.SampleWindow, Data.cfg.SampleLength, Data.cfg.BeaconHeight, Data.cfg.MatchedFilterEnabled);
			localizer.GenerateFilterMatrix(Data.cfg.UseMeasuredSignal);
		}
		#endregion
	}
}
