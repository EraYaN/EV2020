using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class Observer
	{
		public Matrix<double> xDot;
		public Matrix<double> x;

		private int lastTime;

		public Observer()
		{
			xDot = DenseMatrix.OfArray(new double[,]
							{
								{0f},
								{0f}
							});
			x =	DenseMatrix.OfArray(new double[,]
							{
								{3f},
								{0f}
							});
			lastTime = Environment.TickCount;
		}

		// Input yReal in cm
		public int Update(int yReal, int driving)
		{
			// Integrate the summed values of (L * (y - yReal) + B * u - L * x)
			// xDot = -BKx + Ax + L(yReal - Cx)

			xDot = -Data.B * Data.K * x + Data.A * x + Data.L * (yReal - (Data.C * x)[0,0]);

			// Calculate new braking signal
			double brake = (-Data.K * x)[0,0];
			return (int)brake;
		}
	}
}
