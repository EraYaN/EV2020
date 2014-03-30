using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// Class to make it easier to save the output from the car (left and right)
	/// </summary>
	public class OutputSequence
	{
		Sequence left;
		Sequence right;
		public double[] LeftData
		{
			get { return left.Data; }
		}
		public double[] RightData
		{
			get { return right.Data; }
		}
		public long Index
		{
			get
			{
				return left.Index;
			}
		}
		public long Length
		{
			get
			{
				return left.Length;
			}
		}
		
		public OutputSequence(long Size)
		{
			left = new Sequence(Size);
			right = new Sequence(Size);
		}		
		public double GetCurrentLeft()
		{
			return left.DataElement;
		}
		public double GetCurrentRight()
		{
			return right.DataElement;
		}
		public void Forward()
		{
			left.Forward();
			right.Forward();
		}
		public void Backward()
		{
			left.Backward();
			right.Backward();
		}
		public void Rewind()
		{
			left.Rewind();
			right.Rewind();
		}
		public void AddToFront(double LeftValue, double RightValue)
		{
			left.AddToFront(LeftValue);
			right.AddToFront(RightValue);
		}
		public void AddToBack(double LeftValue, double RightValue)
		{
			left.AddToBack(LeftValue);
			right.AddToBack(RightValue);
		}
		
	}
}
