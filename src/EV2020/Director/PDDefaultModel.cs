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
		// State matrix
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
		readonly double Nbar = -0.596358408970907;

		Matrix<double> x;
		Matrix<double> xDot;

		long lastTimestamp;

		public PDDefaultModel()
		{
			A = DenseMatrix.OfArray(new double[,] { { 0, 1 }, { -2.524491182968118, -2.024016922829008 } });
			B = DenseMatrix.OfArray(new double[,] { { -24.048787384198400 }, { 38.614088839675570 } });
			C = DenseMatrix.OfArray(new double[,] { { 1, 0 } });
			D = DenseMatrix.OfArray(new double[,] { { 0 } });
			K = DenseMatrix.OfArray(new double[,] { { -0.1114, -0.0388 } });
			L = DenseMatrix.OfArray(new double[,] { { 37.2383 }, { 705.2425 } });
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
			double feedback = (-K * x)[0,0];
			double u = TargetScaled + feedback;
			u = u.Clamp(-7, 7);
			xDot = A * x + B * u + L * (Distance - (C * x)[0,0]);
			if (lastTimestamp != 0)
			{
				double dt = ((double)DateTime.Now.Ticks - lastTimestamp) / TimeSpan.TicksPerSecond;
				x += xDot * dt;
			}
			lastTimestamp = DateTime.Now.Ticks;
			return u;
		}
		public String GetDebugInfo()
		{
			return x.ToString();
		}
	}
}
