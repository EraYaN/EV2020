using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EV2020.LocationSystem
{
	public class Localizer : IDisposable
	{
		ASIO asio;
		Matrix<double> matchedfilter;
		bool _disposed;
		public delegate void LocationUpdatedHandler(object sender, LocationUpdatedEventArgs e);
		public event LocationUpdatedHandler OnLocationUpdated;
		List<Microphone> Microphones;
		System.Timers.Timer timer;
		Matrix<double> beaconsignal;
		Matrix<double> responses;
		object _posistionLock;
		public Localizer(List<Microphone> mics, int DriverIndex, int[] InputChannels, int[] OutputChannels)			
		{
			Stopwatch sw = Stopwatch.StartNew();
			Debug.WriteLine("Generating beaconsignal.");
			beaconsignal = Tools.refsignal(Tools.Timer0Freq.Carrier10kHz, Tools.Timer1Freq.Code1000Hz, Tools.Timer3Freq.Repeat10Hz, "92340f0f", ASIO.Fs);
			Debug.WriteLine("Generating Toeplitz matrix.");
			
			//Figure out corrent matchedfilter matrix
			Matrix<double> X = Tools.Toep(beaconsignal, beaconsignal.RowCount, beaconsignal.RowCount);
			Debug.WriteLine("Generating matched filter matrix.");
			//Debug.WriteLine("Transposing...");
			//Matrix<double> Xt = X.Transpose();
			Matrix<double> Alpha = beaconsignal.Transpose() * beaconsignal;
			matchedfilter = (1/Alpha[0,0])*X.Transpose();
			//Debug.WriteLine("Multiplying...");
			//Matrix<double> XtX = Xt * X;
			//X = null;
			//Debug.WriteLine("Inverting...");
			//Matrix<double> XtXI = XtX.Inverse();
			//XtX = null;
			//Debug.WriteLine("Multiplying...");
			//matchedfilter = XtXI * Xt;			
			//Xt = null;
			//XtXI = null;
			//matchedfilter = X.TransposeThisAndMultiply(X).QR().Solve(X.Transpose());
			sw.Stop();
			Debug.WriteLine("Generation took {0:f2} seconds",sw.Elapsed.TotalSeconds);
			Debug.WriteLine("Starting ASIO interface.");
			asio = new ASIO(DriverIndex, InputChannels, OutputChannels, Convert.ToInt32(ASIO.Fs));
			Microphones = mics;			
			Debug.WriteLine("Starting timer.");
			timer = new System.Timers.Timer(1000);
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			performMeasurement();
		}
		~Localizer()
		{
			Dispose(false);
		}		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void performMeasurement()
		{			
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			asio.ClearOutput();
			asio.ClearInput();
			//TODO make obsolete
			double[] outsig = new double[beaconsignal.RowCount];
			for (int row = 0; row < beaconsignal.RowCount; row++)
			{
				outsig[row] = beaconsignal[row, 0];
			}
			asio.putOutputSamples(outsig);

			asio.IsInputEnabled = true;
			asio.IsOutputEnabled = true;
			Thread.Sleep(200);
			//get all channels in a matrix (every column is a channel)
			responses = asio.getAllInputSamplesMatrix(matchedfilter.ColumnCount);
			asio.IsInputEnabled = false;
			asio.IsOutputEnabled = false;
			int[] samplemaxes = new int[responses.ColumnCount];
			double[] maxes = new double[responses.ColumnCount];
			//Filter that shit.
			Matrix<double> filteredResponses = matchedfilter * responses;
			//loop over channels
			for (int i = 0; i < filteredResponses.ColumnCount; i++)
			{			
				//loop over samples in channel i	
				for (int sample = 0; sample < filteredResponses.RowCount; sample++)
				{
					if (filteredResponses[sample,0] > maxes[i])
					{
						maxes[i] = filteredResponses[sample, i];
						samplemaxes[i] = sample;
					}
				}
			}
			Debug.WriteLine("Delta: {0}",Math.Abs(samplemaxes[0]-samplemaxes[1])*1/ASIO.Fs);
			//TODO multilaterate posistion			
		}
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				// free other managed objects that implement
				// IDisposable only
				asio.Dispose();
			}

			// release any unmanaged objects
			// set the object references to null

			asio = null;
			matchedfilter = null;

			_disposed = true;
		}

		public void ShowASIOControlPanel()
		{
			asio.ShowControlPanel();
		}

		public ASIOLatencies GetASIOLatencies()
		{
			return asio.GetLatencies();
		}
	}
	public class LocationUpdatedEventArgs : EventArgs
	{
		Position3D Position;
		public LocationUpdatedEventArgs(Position3D _pos)
		{
			Position = _pos;
		}
	}
	public struct Position3D
	{
		public double X;
		public double Y;
		public double Z;
	}

	public struct Microphone
	{
		public Position3D Position;
		public int ChannelIndex;
	}
	
}
