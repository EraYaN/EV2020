using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class PDDefaultModel : IModel
	{
		readonly Matrix<double> A = null;
		// Input-to-state matrix
		readonly Matrix<double> B = null;
		// State-to-output matrix
		readonly Matrix<double> C = null;
		// Feedthrough matrix
		readonly Matrix<double> D = null;
		// Controller Gain
		readonly Matrix<double> K = null;
		// Observer Gain
		readonly Matrix<double> L = null;
		// Input scale gain
		readonly double Nbar = -0.128112399834024;

		Matrix<double> x;
		Matrix<double> xDot;

		long lastTimestamp;

		public PDDefaultModel()
		{
			A = DenseMatrix.OfArray(new double[,]
        {
            //{ -1.540497550593680, 0.499996085982763 },
            //{ -0.050981466007821, -1.221182449406320 }
			{ -2.68, 1.93 },
			{ 1.78, -3.5 }
        });
			// Input-to-state matrix
			B = DenseMatrix.OfArray(new double[,]
        {
            { -24.048787384198400 },
            { 38.614088839675570 }
        });
			// State-to-output matrix
			C = DenseMatrix.OfArray(new double[,]
        {
            { 1, 0 }
        });
			// Feedthrough matrix
			D = DenseMatrix.OfArray(new double[,]
        {
            { 0 }
        });
			// Controller Gain
			K = DenseMatrix.OfArray(new double[,]
        {
            //{ -0.064057182010178, -0.020791231841726 }
			{ -0.11, -0.04 }
        });
			// Observer Gain
			L = DenseMatrix.OfArray(new double[,]
        {
            { 5 },
            { 10 }
        });
		}
		public void Init()
		{
			x = DenseMatrix.OfArray(new double[,] { { 0 }, { 0 } });
			lastTimestamp = 0;
		}

		/// <summary>
		/// All inputs in meters. Output scaled with Nbar.
		/// </summary>
		/// <param name="Distance">The current measured distance.</param>
		/// <param name="Target">The target distance.</param>
		/// <returns>Output signals</returns>
		public double Tick(double Distance, double Target)
		{
			double TargetScaled = Nbar * (Target); // In Meters
			Matrix<double> feedback = -K * x;
			Matrix<double> u = DenseMatrix.OfArray(new double[,] { { TargetScaled } }) + feedback;
			xDot = A * x + B * u + L * (DenseMatrix.OfArray(new double[,] { { Distance } }) - C * x);
			if (lastTimestamp != 0)
			{
				double dt = ((double)DateTime.Now.Ticks - lastTimestamp) / TimeSpan.TicksPerSecond;
				x += xDot * dt;
			}
			lastTimestamp = DateTime.Now.Ticks;
			return (x[0, 0] - Target) / Nbar;
		}
		public String GetDebugInfo()
		{
			return x.ToString();
		}
	}
}
