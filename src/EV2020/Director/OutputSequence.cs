using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class OutputSequence : Sequence
	{
		Decimal[] left;
		Decimal[] right;
		
		public OutputSequence(long _length, long offset = 0)
		{
			length = _length;			
			if (offset > length)
				throw new ArgumentException("Offset is bigger than length.", "offset");			
			left = new Decimal[length];
			right = new Decimal[length];
			index = offset;
		}		
		public Decimal GetCurrentLeft()
		{
			return left[index];
		}
		public Decimal GetCurrentRight()
		{
			return right[index];
		}
		
	}
}
