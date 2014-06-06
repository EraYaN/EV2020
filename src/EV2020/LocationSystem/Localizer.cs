using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Timers;

namespace EV2020.LocationSystem
{
	public class Localizer : IDisposable
	{
		const double samplelength = 0.25; //in seconds
		const double margin = 1;
		//Width and Height of the rectangle field
		const double H = 7.4;
		const double B = 7;
		//Height of beacon
		const double h = 0.28;
		//speed of sound
		const double c = 343.2;
		ASIO asio;
		Matrix<double> matchedfilter;
		bool _disposed;
		public delegate void LocationUpdatedHandler(object sender, LocationUpdatedEventArgs e);
		public event LocationUpdatedHandler OnLocationUpdated;
		List<Microphone> Microphones;
		System.Timers.Timer timer;
		public Matrix<double> lastData;
		public int[] lastMaxes;
		Vector<double> beaconsignal;
		object _posistionLock;
		Stopwatch sw;
		int _driverIndex;
		int[] _inputChannels;
		int[] _outputChannels;
		double samplewindow = ASIO.Fs / 5;
		public Localizer(List<Microphone> mics, int DriverIndex, int[] InputChannels, int[] OutputChannels)			
		{			
			Microphones = mics;
			_driverIndex = DriverIndex;
			_inputChannels = InputChannels;
			_outputChannels = OutputChannels;
		}

		public bool GenerateFilterMatrix()
		{
			Debug.WriteLine("Generating beaconsignal.");

			FileInfo beaconfile = new FileInfo(@"resources/beaconfile.bin");
			if (beaconfile.Exists && false)
			{
				long samples = beaconfile.Length / 4; //singles
				beaconsignal = DenseVector.Create(Convert.ToInt32(samples), 0);
				using (BinaryReader br = new BinaryReader(beaconfile.OpenRead()))
				{
					int i = 0;
					while (i < samples)
					{
						beaconsignal[i] = br.ReadSingle();
						i++;
					}
				}
				Debug.WriteLine("Used measurement data for beaconsignal.");
			}
			else
			{
				//Own Code: e65a20e5b37ac60d
				beaconsignal = Tools.refsignal(Tools.Timer0Freq.Carrier10kHz, Tools.Timer1Freq.Code2500Hz, Tools.Timer3Freq.Repeat5Hz, "e65a20e5", ASIO.Fs);
				Debug.WriteLine("Used reference data for beaconsignal.");
			}
			//Circulant Convolution
			Debug.WriteLine("Generating Toeplitz matrix.");

			//Figure out corrent matchedfilter matrix
			Vector<double> bsreverse = new DenseVector(Convert.ToInt32(Math.Round(ASIO.Fs*samplelength)));
			int samplenumber = 0;
			foreach (double d in beaconsignal.Reverse())
			{
				bsreverse[samplenumber] = d;
				samplenumber++;
				if (samplenumber == bsreverse.Count)
					break;
			}
			//Normal Convolution
			//Matrix<double> X = Tools.Toep(bsreverse, bsreverse.Count*2-1, bsreverse.Count, false);
			//Circulant Convolution
			Matrix<double> X = Tools.Toep(bsreverse, bsreverse.Count, bsreverse.Count);
			bsreverse = null;
			Debug.WriteLine("Generating matched filter matrix.");
			//X is a circulant matrix.
			matchedfilter = X;
			return true;
		}

		public bool InitializeDriver()
		{
			if (matchedfilter == null)
				return false;
			asio = new ASIO(_driverIndex, _inputChannels, _outputChannels, Convert.ToInt32(ASIO.Fs));
			Debug.WriteLine("Starting timer.");
			//TODO back to 250 ms
			timer = new System.Timers.Timer(1000);
			timer.Elapsed += timer_Elapsed;
			timer.Start();
			asio.IsInputEnabled = true;
			return true;
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
			//asio.ClearInput();		
			
			ASIOLatencies lat = GetASIOLatencies();
			Debug.WriteLine("Latencies; IN: {0}, OUT: {1}",lat.Input,lat.Output);
			Thread.Sleep(new TimeSpan(Convert.ToInt64(((lat.Input + matchedfilter.ColumnCount) * ASIO.T) * TimeSpan.TicksPerSecond)));
			//get all channels in a matrix (every column is a channel)
			if (asio == null)
				return;
			Matrix<double> responses = asio.getAllInputSamplesMatrix(matchedfilter.ColumnCount); //MATCHED Filter way
			//asio.IsInputEnabled = false;						
			 //Matrix multiply way
			int[] samplemaxes = new int[responses.ColumnCount];
			double[] maxes = new double[responses.ColumnCount];
			//Filter that shit.
			Matrix<double> filteredResponses = /*matchedfilter **/ responses;
			//loop over channels	
			double windowStart = 0;
			double windowEnd = filteredResponses.RowCount;
			for (int i = 0; i < filteredResponses.ColumnCount; i++)
			{				
				//loop over samples in channel i	
				for (int sample = (int)Math.Floor(windowStart); sample < Math.Ceiling(windowEnd); sample++)
				{						
					if (Math.Abs(filteredResponses[sample, i]) > maxes[i])
					{
						maxes[i] = Math.Abs(filteredResponses[sample, i]);
						samplemaxes[i] = sample;
					}
				}
				if (i == 0)
				{
					windowStart = Math.Max(0, samplemaxes[i] - samplewindow / 2);
					windowEnd = Math.Min(filteredResponses.RowCount, samplemaxes[i] + samplewindow / 2); 
				}
			}
			Debug.WriteLine("Channel 0 is the 'zero' at @ {0} samples.", samplemaxes[0]);
			for (int i = 1; i < samplemaxes.Length; i++)
			{
				Debug.WriteLine("Channel {2} delta is {0:f2} ms at @ {1} samples.", Math.Abs(samplemaxes[0] - samplemaxes[1]) * ASIO.T * 1000, samplemaxes[i], i);
			}			
			//lastData = responses.Append(filteredResponses);
			//lastData = responses;
			lastData = filteredResponses;
			lastMaxes = samplemaxes;
			Position3D lpos = Localize(samplemaxes,lat);
			
			OnLocationUpdated(this, new LocationUpdatedEventArgs(lpos));
		}

        protected Position3D Localize(int[] samplemaxes, ASIOLatencies lat)
        {
            //Working vars
            Position3D loc = new Position3D();


            Matrix<double> a = DenseMatrix.Create(10, 6, 0);
            Vector<double> b = DenseVector.Create(10, 0);

            int nmics = Microphones.Count;
            int row = 0;
            for (int i = 0; i < nmics; i++)
            {
                for (int j = 0; j < nmics; i++)
                {
                    if (j <= i)
                        continue;
                    a[row, 0] = 2 * (Microphones[j].Position.X - Microphones[i].Position.X);
                    a[row, 1] = 2 * (Microphones[j].Position.Y - Microphones[i].Position.Y);
                    a[row, 1 + j] = -2 * (samplemaxes[i] - samplemaxes[j]) / ASIO.Fs * c;
                    b[row] = Math.Pow(((samplemaxes[i] - samplemaxes[j]) / ASIO.Fs * c), 2) - Math.Pow(Position3D.Magnitude(Microphones[i].Position), 2) + Math.Pow(Position3D.Magnitude(Microphones[j].Position), 2);
                    row++;
                }
            }

            Vector<double> y = a.Solve(b);
            loc = new Position3D() { X = y[0], Y = y[1], Z = 0 };
            return loc;
        }

		protected List<Position3D> Filter(List<Position3D> x)
		{
			List<Position3D> result = new List<Position3D>();			
			for (int i = 0; i < x.Count; i++)
			{
				if (x[i].X < -margin || x[i].X > B + margin)
				{
					continue;
				}
				if (x[i].Y < -margin || x[i].Y > H + margin)
				{
					continue;
				}
				result.Add(x[i]);
			}
			return result;
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
		public readonly Position3D Position;
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
		public static Position3D operator -(Position3D x, Position3D y)
		{
			x.X -= y.X;
			x.Y -= y.Y;
			x.Z -= y.Z;
			return x;
		}
		public static Position3D operator +(Position3D x, Position3D y)
		{
			x.X += y.X;
			x.Y += y.Y;
			x.Z += y.Z;
			return x;
		}
		public static double Magnitude(Position3D x)
		{
			return Math.Sqrt(Math.Pow(x.X, 2) + Math.Pow(x.Y, 2) + Math.Pow(x.Z, 2));
		}
		public override string ToString()
		{
			return "Posistion3D: " + X.ToString("F3") + "; " + Y.ToString("F3") + "; " + Z.ToString("F3");
		}
	}

	public struct Microphone
	{
		public Position3D Position;
		public int ChannelIndex;
	}
	
}
