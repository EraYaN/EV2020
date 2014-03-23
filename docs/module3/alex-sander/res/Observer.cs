using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public class Observer
	{
		Matrix<double> x;
		Matrix<double> xDot;
		public String X
		{
			get { return x.ToString(); }
		}
		long lastTimestamp;
		public Observer()
		{
			x = DenseMatrix.OfArray(new double[,] { { 0 }, { 0 } });
			lastTimestamp = 0;
		}

		/// <summary>
		/// All inputs in meters. Output scaled with Nbar.
		/// </summary>
		/// <param name="Distance">The current measured distance.</param>
		/// <param name="Target">The target distance.</param>
		/// <returns></returns>
		public double Tick(double Distance, double Target)
		{
			double TargetScaled = Data.Nbar * (Target); // In Meters
			Matrix<double> feedback = -Data.K * x;
			Matrix<double> u = DenseMatrix.OfArray(new double[,] { { TargetScaled } }) + feedback;
			xDot = Data.A * x + Data.B * u + Data.L * (DenseMatrix.OfArray(new double[,] { { Distance } }) - Data.C * x);
			if (lastTimestamp != 0)
			{				
				double dt = ((double)DateTime.Now.Ticks-lastTimestamp)/TimeSpan.TicksPerSecond;
				x += xDot * dt;
				Data.db.UpdateProperty("StateString");
			}
			lastTimestamp = DateTime.Now.Ticks;
			return (x[0, 0]-Target) / Data.Nbar;
		}
	}
}
