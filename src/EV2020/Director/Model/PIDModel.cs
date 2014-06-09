using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The model with the PID Controller. Not finished.
	/// </summary>
	class PIDModel
	{			
		long lastTimestamp;

		public PIDModel()
		{
					
		}

		public void Init()
		{
			lastTimestamp = 0;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="Position">The current measured distance.</param>
		/// <param name="Target">The target distance.</param>
		/// <returns>Output signals</returns>
		public double Tick(double Distance, double Target)
		{
            throw new NotImplementedException();
		}
		public String GetDebugInfo()
		{
            throw new NotImplementedException();
		}
	}
}
