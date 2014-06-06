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
		const bool filterenabled = false;
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
		public Localizer(List<Microphone> mics, int DriverIndex)			
		{			
			Microphones = mics;
			_driverIndex = DriverIndex;
			int[] ins = new int[mics.Count];
			for(int i = 0; i< mics.Count;i++){
				ins[i] = mics[i].ChannelIndex;
			}
			_inputChannels = ins;
			_outputChannels = new int[0];
		}

		public bool GenerateFilterMatrix()
		{
			if (filterenabled)
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
				Vector<double> bsreverse = new DenseVector(Convert.ToInt32(Math.Round(ASIO.Fs * samplelength)));
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
			}
			else
			{
				Debug.WriteLine("Matched filter is disabled.");
			}
			return true;
		}

		public bool InitializeDriver()
		{
			if (matchedfilter == null)
				return false;
			asio = new ASIO(_driverIndex, _inputChannels, _outputChannels, Convert.ToInt32(ASIO.Fs));
			Debug.WriteLine("Starting timer.");
			//TODO back to 250 ms
			timer = new System.Timers.Timer(samplelength*1000);
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
			Matrix<double> filteredResponses;
			if(filterenabled){
				filteredResponses = matchedfilter * responses;
			} else {
				filteredResponses = responses;
			}
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
			List<Position3D> l = new List<Position3D>();
			l.Add(Localize(samplemaxes,lat));
			//l.Add(Localize2(samplemaxes, lat));
			Debug.WriteLine(l[0]);
			//Debug.WriteLine(l[1]);
			Position3D lpos = GetAveragePosition(l);
			Debug.WriteLine(lpos);
			
			OnLocationUpdated(this, new LocationUpdatedEventArgs(lpos));
		}

        protected Position3D Localize(int[] samplemaxes, ASIOLatencies lat)
        {
            //Working vars
            Position3D loc = new Position3D();

			int nmics = Math.Min(Microphones.Count, asio.InChannels);
			int row = 0;

			Matrix<double> a = DenseMatrix.Create(nmics * (nmics - 1), nmics - 1, 0);
			Vector<double> b = DenseVector.Create(nmics * (nmics - 1), 0);

            
            for (int i = 0; i < nmics; i++)
            {
                for (int j = 0; j < nmics; j++)
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
		protected Position3D Localize2(int[] samplemaxes, ASIOLatencies lat)
		{

			Complex d1 = 0, d2 = 0, d3 = 0;
			//Working vars
			Position3D new_loc, loc = new Position3D();
			int new_points = 0, points = 0;
			double new_points_std = 0, points_std = 0;

			for (int k = 0; k < 4; k++)
			{
				Complex C;
				if (samplemaxes.Length >= 4)
				{
					if (k == 1)
					{
						d1 = samplemaxes[0] * c / ASIO.Fs; // Base point (0,0)
						d2 = samplemaxes[1] * c / ASIO.Fs; // Y-direction
						d3 = samplemaxes[3] * c / ASIO.Fs; // X-direction
					}
					else if (k == 2)
					{
						d1 = samplemaxes[2] * c / ASIO.Fs; // Base point (0,0)
						d2 = samplemaxes[3] * c / ASIO.Fs; // Y-direction
						d3 = samplemaxes[1] * c / ASIO.Fs; // X-direction
					}
					else if (k == 3)
					{
						d1 = samplemaxes[1] * c / ASIO.Fs; // Base point (0,0)
						d2 = samplemaxes[0] * c / ASIO.Fs; // Y-direction
						d3 = samplemaxes[2] * c / ASIO.Fs; // X-direction
					}
					else if (k == 4)
					{
						d1 = samplemaxes[3] * c / ASIO.Fs; // Base point (0,0)
						d2 = samplemaxes[2] * c / ASIO.Fs; // Y-direction
						d3 = samplemaxes[0] * c / ASIO.Fs; // X-direction
					}

					C =
					(
						(B * Complex.Pow(d1, 3) - Complex.Pow(B, 3) * d1 + Complex.Pow(B, 3) * d2 - H * Complex.Pow(d1, 3) + 2 * d1 *
						Complex.Sqrt(
							((B + d1 - d3) * (B - d1 + d3) * (Complex.Pow(B, 4) - 2 * Complex.Pow(B, 3) * d1 + 2 * Complex.Pow(B, 3) * d2 + Complex.Pow(B, 2) * Complex.Pow(H, 2) + 2 * Complex.Pow(B, 2) * H * d1 - 2 * Complex.Pow(B, 2) * H * d2 + Complex.Pow(B, 2) * Complex.Pow(d1, 2) - 2 * Complex.Pow(B, 2) * d1 * d3 - Complex.Pow(B, 2) * Complex.Pow(d2, 2) + 2 * Complex.Pow(B, 2) * d2 * d3 - 2 * B * H * Complex.Pow(d1, 2) + 4 * B * H * d1 * d2 - 2 * B * H * Complex.Pow(d2, 2) - 2 * B * Complex.Pow(d1, 2) * d2 + 2 * B * Complex.Pow(d1, 2) * d3 + 4 * B * d1 * Complex.Pow(d2, 2) - 4 * B * d1 * d2 * d3 - 2 * B * Complex.Pow(d2, 3) + 2 * B * Complex.Pow(d2, 2) * d3 - Complex.Pow(H, 2) * Complex.Pow(d1, 2) + 2 * Complex.Pow(H, 2) * d1 * d3 - Complex.Pow(H, 2) * Complex.Pow(d3, 2) + 2 * H * Complex.Pow(d1, 2) * d2 - 2 * H * Complex.Pow(d1, 2) * d3 - 4 * H * d1 * Complex.Pow(d2, 2) + 4 * H * d1 * d2 * d3 + 2 * H * Complex.Pow(d2, 3) - 2 * H * Complex.Pow(d2, 2) * d3 + Complex.Pow(d1, 2) * Complex.Pow(d2, 2) - 2 * Complex.Pow(d1, 2) * d2 * d3 + Complex.Pow(d1, 2) * Complex.Pow(d3, 2) - 2 * d1 * Complex.Pow(d2, 3) + 4 * d1 * Complex.Pow(d2, 2) * d3 - 2 * d1 * d2 * Complex.Pow(d3, 2) + Complex.Pow(d2, 4) - 2 * Complex.Pow(d2, 3) * d3 + Complex.Pow(d2, 2) * Complex.Pow(d3, 2))) / 4
						)
						-
						2 * d2 *
						Complex.Sqrt(
							(
								(B + d1 - d3) * (B - d1 + d3) * (Complex.Pow(B, 4) - 2 * Complex.Pow(B, 3) * d1 + 2 * Complex.Pow(B, 3) * d2 + Complex.Pow(B, 2) * Complex.Pow(H, 2) + 2 * Complex.Pow(B, 2) * H * d1 - 2 * Complex.Pow(B, 2) * H * d2 + Complex.Pow(B, 2) * Complex.Pow(d1, 2) - 2 * Complex.Pow(B, 2) * d1 * d3 - Complex.Pow(B, 2) * Complex.Pow(d2, 2) + 2 * Complex.Pow(B, 2) * d2 * d3 - 2 * B * H * Complex.Pow(d1, 2) + 4 * B * H * d1 * d2 - 2 * B * H * Complex.Pow(d2, 2) - 2 * B * Complex.Pow(d1, 2) * d2 + 2 * B * Complex.Pow(d1, 2) * d3 + 4 * B * d1 * Complex.Pow(d2, 2) - 4 * B * d1 * d2 * d3 - 2 * B * Complex.Pow(d2, 3) + 2 * B * Complex.Pow(d2, 2) * d3 - Complex.Pow(H, 2) * Complex.Pow(d1, 2) + 2 * Complex.Pow(H, 2) * d1 * d3 - Complex.Pow(H, 2) * Complex.Pow(d3, 2) + 2 * H * Complex.Pow(d1, 2) * d2 - 2 * H * Complex.Pow(d1, 2) * d3 - 4 * H * d1 * Complex.Pow(d2, 2) + 4 * H * d1 * d2 * d3 + 2 * H * Complex.Pow(d2, 3) - 2 * H * Complex.Pow(d2, 2) * d3 + Complex.Pow(d1, 2) * Complex.Pow(d2, 2) - 2 * Complex.Pow(d1, 2) * d2 * d3 + Complex.Pow(d1, 2) * Complex.Pow(d3, 2) - 2 * d1 * Complex.Pow(d2, 3) + 4 * d1 * Complex.Pow(d2, 2) * d3 - 2 * d1 * d2 * Complex.Pow(d3, 2) + Complex.Pow(d2, 4) - 2 * Complex.Pow(d2, 3) * d3 + Complex.Pow(d2, 2) * Complex.Pow(d3, 2))
							)
							/
							4
						)
						- Complex.Pow(d1, 3) * d2 + d1 * Complex.Pow(d3, 3) + Complex.Pow(d1, 3) * d3 - d2 * Complex.Pow(d3, 3) + Complex.Pow(B, 4) - Complex.Pow(B, 2) * Complex.Pow(d1, 2) - Complex.Pow(B, 2) * Complex.Pow(d2, 2) - Complex.Pow(B, 2) * Complex.Pow(d3, 2) + Complex.Pow(d1, 2) * Complex.Pow(d2, 2) - 2 * Complex.Pow(d1, 2) * Complex.Pow(d3, 2) + Complex.Pow(d2, 2) * Complex.Pow(d3, 2) + Complex.Pow(B, 2) * H * d1 - Complex.Pow(B, 2) * H * d2 - B * Complex.Pow(d1, 2) * d2 + Complex.Pow(B, 2) * d1 * d2 + B * d1 * Complex.Pow(d3, 2) - 2 * B * Complex.Pow(d1, 2) * d3 + Complex.Pow(B, 2) * d1 * d3 - B * d2 * Complex.Pow(d3, 2) + Complex.Pow(B, 2) * d2 * d3 + H * Complex.Pow(d1, 2) * d2 - H * d1 * Complex.Pow(d3, 2) + 2 * H * Complex.Pow(d1, 2) * d3 + H * d2 * Complex.Pow(d3, 2) + d1 * d2 * Complex.Pow(d3, 2) - 2 * d1 * Complex.Pow(d2, 2) * d3 + Complex.Pow(d1, 2) * d2 * d3 + 2 * B * d1 * d2 * d3 - 2 * H * d1 * d2 * d3) / (Complex.Pow(B, 2) - 2 * Complex.Pow(d1, 2) + 2 * d1 * d2 + 2 * d1 * d3 - Complex.Pow(d2, 2) - Complex.Pow(d3, 2)) - Complex.Pow(B, 2) - Complex.Pow(d1, 2) + Complex.Pow(d2, 2)
					)
					/
					(
					2 * d1 - 2 * d2
					);
				}
				else
				{
					C = 0;
				}
				//Make it a time.
				C = C / c;
				double[] t = new double[samplemaxes.Length];
				for (int i = 0; i < t.Length; i++)
				{
					t[i] = lat.Input / ASIO.Fs + C.Real + samplemaxes[i] / ASIO.Fs;
				}

				Laterate(t, h, c, out new_loc, out new_points, out new_points_std);
				if (new_points > points || (new_points == points && new_points_std < points_std))
				{
					loc = new_loc;
					points = new_points;
					points_std = new_points_std;
				}
			}
			return loc;
		}
		protected void Laterate(double[] t, double h, double c, out Position3D loc, out int points, out double points_std)
		{
			/*loc = new Position3D() { X = 0, Y = 0, Z = h };
			points = 5;
			points_std = 0;*/
			List<Position3D> y = new List<Position3D>();
			int passes = Math.Min(Microphones.Count, t.Length);
			for (int i = 0; i < passes; i++)
			{
				for (int j = 0; j < passes; j++)
				{
					if (i == j || i < j)
					{
						continue; // Do not compare with own time or twice thesame
					}
					double d = Math.Sqrt(Math.Pow(Microphones[i].Position.X - Microphones[j].Position.X, 2) + Math.Pow(Microphones[i].Position.Y - Microphones[j].Position.Y, 2));
					double h1 = Microphones[i].Position.X - h;
					double h2 = Microphones[j].Position.X - h;

					double t1_ = (Complex.Sqrt(Math.Pow(t[i] * c, 2) - Math.Pow(h1, 2))).Real / c;
					double t2_ = (Complex.Sqrt(Math.Pow(t[j] * c, 2) - Math.Pow(h2, 2))).Real / c;

					double d1 = (Math.Pow(t1_ * c, 2) - Math.Pow(t2_ * c, 2) + Math.Pow(d, 2)) / (2 * d);
					double x = (Complex.Sqrt((Math.Pow(t1_ * c, 2) - Math.Pow(d1, 2)))).Real;
					double mx = d1 / d * (Microphones[j].Position.X - Microphones[i].Position.X);
					double my = d1 / d * (Microphones[j].Position.Y - Microphones[i].Position.Y);


					Vector<double> r = DenseVector.OfArray(new double[] { Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2)), x });
					double angle = -Math.Atan2(my, mx);
					Matrix<double> rot = DenseMatrix.OfArray(new double[,] { { Math.Cos(angle), -Math.Sin(angle) }, { Math.Sin(angle), Math.Cos(angle) } });
					Vector<double> disp = DenseVector.OfArray(new double[] { Microphones[i].Position.X, Microphones[i].Position.Y });
					Vector<double> coord = r * rot + disp;
					y.Add(new Position3D() { X = coord[0], Y = coord[1], Z = h });

				}
			}
			y = Filter(y);
			y = RemoveOutliers(y); // Remove the mirror images and other outliers due to input data errors
			loc = GetAveragePosition(y);
			points = y.Count;
			points_std = y.Select(o => o.X).StdDev() + y.Select(o => o.Y).StdDev();
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
		protected List<Position3D> RemoveOutliers(List<Position3D> x)
		{
			List<Position3D> result = new List<Position3D>();
			if (x.Count <= 1)
			{
				return x;
			}
			for (int i = 0; i < x.Count; i++)
			{
				List<Position3D> y = new List<Position3D>();
				for (int j = 0; j < x.Count; j++)
				{
					if (Position3D.Magnitude(x[j] - x[i]) < 0.20)
					{// Max 20 cm from the rest
						// Pick the beacon location instead of its mirror image
						bool firstBest = false;
						int index = (int)Math.Floor((double)(j) / 2) * 2;
						if (index >= 0 && index + 1 < x.Count)
						{
							if (Position3D.Magnitude(x[index + 1] - x[i]) > Position3D.Magnitude(x[index] - x[i]))
							{
								firstBest = true;
							}
						}
						// Only get one per two results
						if ((j % 2 == 0 && firstBest) || (j % 2 == 1 && !firstBest))
						{
							y.Add(x[j]);
						}
					}
				}
				if (y.Count > result.Count)
				{
					result = y;
				}
			}
			return result;
		}
		protected Position3D GetAveragePosition(List<Position3D> x)
		{
			if (x.Count > 0)
			{
				return new Position3D() { X = x.Average(p => p.X), Y = x.Average(p => p.Y), Z = x.Average(p => p.Z) };
			}
			else { return new Position3D(); }

		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				// free other managed objects that implement
				// IDisposable only
				if(asio!=null)
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
