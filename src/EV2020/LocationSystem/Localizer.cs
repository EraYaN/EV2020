using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Numerics;

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
		public Matrix<double> lastData;
		Matrix<double> beaconsignal;
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
			//X is a circulant matrix.
			matchedfilter = X;

			//Debug.WriteLine("Transposing...");
			//Matrix<double> Xt = X.Transpose();
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
			if (asio == null)
				return;
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
			ASIOLatencies lat = GetASIOLatencies();
			Debug.WriteLine("Latencies; IN: {0}, OUT: {1}",lat.Input,lat.Output);
			Thread.Sleep(new TimeSpan(Convert.ToInt64((lat.Input*ASIO.T+0.1)*TimeSpan.TicksPerSecond)));
			//get all channels in a matrix (every column is a channel)
			if (asio == null)
				return;
			Matrix<double> responses = asio.getAllInputSamplesMatrix(matchedfilter.ColumnCount); //MATCHED Filter way
			//double[][] responses = asio.getAllInputSamples(); //FFT way
			asio.IsInputEnabled = false;
			asio.IsOutputEnabled = false;
			//FFT way
			/*
			int[] samplemaxes = new int[responses.Length];
			double[] maxes = new double[responses.Length];
			Complex[] workingarrayY;
			Complex[] workingarrayX;
			Complex[] workingarrayH;
			
			using (StreamWriter sw = new StreamWriter("FilteredData.csv",true))
			{
				for (int i = 0; i < responses.Length; i++)
				{
					workingarrayY = new Complex[responses[i].Length];
					workingarrayX = new Complex[responses[i].Length];
					for (int k = 0; k < responses[i].Length; k++)
					{
						workingarrayY[k] = responses[i][k];
						if (k < beaconsignal.RowCount)
							workingarrayX[k] = beaconsignal[k, 0];
						else
							workingarrayX[k] = 0;
					}
					Transform.FourierForward(workingarrayX, FourierOptions.Matlab);
					Transform.FourierForward(workingarrayY, FourierOptions.Matlab);
					workingarrayH = new Complex[responses[i].Length];
					for (int k = 0; k < responses[i].Length; k++)
						workingarrayH[k] = workingarrayY[k] / workingarrayX[k];
					Transform.FourierInverse(workingarrayH,FourierOptions.Matlab);
					//TODO figure out why H is now a sinus kinda.
					int L = workingarrayY.Length - beaconsignal.RowCount + 1;
					for (int sample = 0; sample < L; sample++)
					{
						sw.Write("{0};", workingarrayH[sample].Magnitude);
						if (workingarrayH[sample].Magnitude > maxes[i])
						{
							maxes[i] = workingarrayH[sample].Magnitude;
							samplemaxes[i] = sample;
						}
					}
					sw.WriteLine();
				}
			}*/
			
			 //Matrix multiply way
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
					if (filteredResponses[sample, 0] > maxes[i])
					{
						maxes[i] = filteredResponses[sample, i];
						samplemaxes[i] = sample;
					}
				}
			}
			using (StreamWriter sw = new StreamWriter("FilteredData.csv"))
			{
				sw.WriteLine("x1;x2;");
				for (int i = 0; i < filteredResponses.RowCount; i++)
				{
					//loop over samples in channel i	
					for (int j = 0; j < filteredResponses.ColumnCount; j++)
					{
						sw.Write("{0};",filteredResponses[i, j]);						
					}
					sw.WriteLine();
				}
			}
			Debug.WriteLine("Delta: {0}",Math.Abs(samplemaxes[0]-samplemaxes[1])*ASIO.T);
			//TODO multilaterate posistion	
			//lastData = responses.Append(filteredResponses);
			lastData = responses;

			OnLocationUpdated(this, new LocationUpdatedEventArgs(new Position3D() { X = 0, Y = 0, Z = 0 }));
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
