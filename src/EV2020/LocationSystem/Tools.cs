using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.LocationSystem
{
	static public class Tools
	{
		public enum Timer0Freq
		{
			NoModulation = 0,
			Carrier5kHz = 5000,
			Carrier10kHz = 10000,
			Carrier15kHz = 15000,
			Carrier20kHz = 20000,
			Carrier25kHz = 25000,
			Carrier30kHz = 30000
		}
		public enum Timer1Freq
		{
			Code1000Hz = 1000,
			Code1500Hz = 1500,
			Code2000Hz = 2000,
			Code2500Hz = 2500,
			Code3000Hz = 3000,
			Code3500Hz = 3500,
			Code4000Hz = 4000,
			Code4500Hz = 4500,
			Code5000Hz = 5000
		}
		public enum Timer3Freq
		{
			Repeat1Hz = 1,
			Repeat2Hz = 2,
			Repeat3Hz = 3,
			Repeat4Hz = 4,
			Repeat5Hz = 5,
			Repeat6Hz = 6,
			Repeat7Hz = 7,
			Repeat8Hz = 8,
			Repeat9Hz = 9,
			Repeat10Hz = 10,
		}
		static public Vector<double> refsignal(Timer0Freq Timer0, Timer1Freq Timer1, Timer3Freq Timer3, string code, double Fs)
		{
			double f0 = Convert.ToDouble(Timer0);
			double f1 = Convert.ToDouble(Timer1);
			double f3 = Convert.ToDouble(Timer3);
			double T = 1 / Fs;
			int samplesperbit = Convert.ToInt32(Math.Round(Fs / f1));
			int samples = (int)Math.Ceiling(Fs / f3);
			Vector<double> signal = new DenseVector(samples);

			string bincodestring = Convert.ToString(Convert.ToInt64(code, 16), 2);// Convert.ToString(code, 2); //string.Join("",code.Reverse().Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
			int bitnumber = 0;
			for (int ind = 0; ind < bincodestring.Length; ind++)
			{
				char s = bincodestring[ind];
				bitnumber++;
				if (s == '1')
				{
					int indexfrom = (bitnumber - 1) * samplesperbit + 1;
					int indexto = bitnumber * samplesperbit;
					for (int sample = indexfrom; sample <= indexto; sample++)
					{
						signal[sample] = 1.0f;
					}
				}
			}
			for(int i = 0; i<samples; i++){
				double carrier = Math.Round(Math.Cos(2 * Math.PI * f0 / Fs * i) / 2 + 0.5);
				signal[i] *= carrier;
			}
			return signal;
		}
		static public Matrix<double> Toep(Vector<double> x0, int N, int L, bool circulant = true)
		{
			int N0 = x0.Count;
			if (N0 > N)
			{
				throw new ArgumentException("x0's length should be equal or less than N.", "x0");
			}
			Matrix<double> X;			
			try
			{
				//faster
				X = DenseMatrix.Create(N, L, 0);
				Debug.WriteLine("Used DenseMatrix for Toeplitz matrix.");
			}
			catch(OutOfMemoryException)
			{
				//slower
				X = SparseMatrix.Create(N, L, 0);
				Debug.WriteLine("Used SparseMatrix for Toeplitz matrix.");
			}
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < L; j++)
				{
					int index = (i + 1) - (j + 1);
					
					if (circulant)
					{
						//weirdo matlab modulo
						while (index < 0)
							index += N;
						index = index % N;
					}
					else
					{
						index = index % N;
					}					
					if (index>=0 && index < N0)
					{
						X[i, j] = x0[index];
					}
				}
			}
			return X;
		}
		static public Matrix<double> Toep(Vector<double> x0, int N, bool circulant = true)
		{
			int N0 = x0.Count;
			return Toep(x0, N, N - N0 + 1, circulant);
		}
	}
}
