using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public class Sequence
	{
		protected long length;
		public long Length
		{
			get { return length; }
		}

		protected long index;
		public long Index
		{
			get { return index; }
		}
		public bool IsEndOfSignal
		{
			get { return index == Length-1; }
		}
		public bool IsStartOfSignal
		{
			get { return index == 0; }
		}
		public double[] Data
		{
			get
			{
				return data;
			}
		}
		public double DataElement
		{
			get
			{
				return data[index];
			}
		}
		public double FirstDataElement
		{
			get
			{
				return data[0];
			}
		}
		public double LastDataElement
		{
			get
			{
				return data[length-1];
			}
		}
		protected double[] data;
		public Sequence()
		{
			data = new double[0];
			length = 0;
			index = 0;
		}
		public Sequence(long Size)
		{
			length = Size;
			data = new double[Size];
			index = 0;
		}
		public Sequence(double[] Data)
		{
			length = Data.LongLength;
			data = Data;
			index = 0;
		}
		public void Forward()
		{
			if (index < Length - 1)
				index++;
		}
		public void Backward()
		{
			if (index > 0)
				index--;
		}
		public void Rewind()
		{
			index = 0;
		}
		public void AddToFront(double Value)
		{
			double[] tmp = new double[data.Length];

			if(index<length-1)
				index++;

			Buffer.BlockCopy(data, 0, tmp, sizeof(double), (data.Length - 1) * sizeof(double));
			data = tmp;
			data[0] = Value;
		}
		public void AddToBack(double Value)
		{
			double[] tmp = new double[data.Length];
			if(index>0)
				index--;
			Buffer.BlockCopy(data, sizeof(double), tmp, 0, (data.Length - 2) * sizeof(double));
			data = tmp;
			data[data.Length-1] = Value;
		}
		public void Append(Sequence a)
		{
			double[] c = new double[data.Length+a.data.Length];
			Array.Copy(data, c, data.Length);
			Array.Copy(a.Data, 0, c, data.Length, a.Length);
			data = c;
			length = data.Length;
		}		
		#region Static generation methods
		public static Sequence Linear(long samples, double Start, double End)
		{
			double[] Data = new double[samples];
			double CoEff = (End - Start) / samples;
			for (long I = 0; I < samples; I++)
			{
				Data[I] = I * CoEff + Start;
			}
			return new Sequence(Data);
		}
		public static Sequence Pulse(long samples, long pulseSamplesStart, long pulseSamplesEnd, double High, double Low)
		{
			double[] Data = new double[samples];
			for (long I = 0; I < samples; I++)
			{
				if (I >= pulseSamplesStart && I <= pulseSamplesEnd)
				{
					Data[I] = High;
				}
				else
				{
					Data[I] = Low;
				}
			}
			return new Sequence(Data);
		}
		public static Sequence SquareWave(long samples, long period, double High, double Low)
		{
			double[] Data = new double[samples];
			bool high = false;
			for (long I = 0; I < samples; I++)
			{
				if (I % period==0)
					high = !high;
				if (high)
				{
					Data[I] = High;
				}
				else
				{
					Data[I] = Low;
				}
				if (I == samples-1)
					Data[I] = 0;
			}
			return new Sequence(Data);
		}
		public static Sequence ExponentialDecline(long samples, long declineDuration, double Start, double End)
		{
			double[] Data = new double[samples];
			double CoEff = (Start - End) / (declineDuration*Math.E);
			for (long I = 0; I < samples; I++)
			{
				Data[I] = (Start - End) * Math.Exp(-CoEff * I);				
			}
			return new Sequence(Data);
		}
		#endregion
	}
}
