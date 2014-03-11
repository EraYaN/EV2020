using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class Sequence
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
			get { return !(index < Length); }
		}
		public bool IsStartOfSignal
		{
			get { return index == 0; }
		}
		public static Sequence operator ++(Sequence s)
		{
			if (s.index < s.Length - 1)
				s.index++;
			return s;
		}
		public static InputSequence operator --(Sequence s)
		{
			if (s.index > 0)
				s.index--;
			return s;
		}
	}
}
