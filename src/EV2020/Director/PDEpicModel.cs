using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class PDEpicModel : IModel
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

		Matrix<double> x;
		Matrix<double> xDot;
		double debug1;

		long lastTimestamp;

		public PDEpicModel()
		{
			A = DenseMatrix.OfArray(new double[,] { { 0, 1 }, { 0, -1.782 } });
			B = DenseMatrix.OfArray(new double[,] { { 0 }, { -39.94 } });
			C = DenseMatrix.OfArray(new double[,] { { 1, 0 } });
			D = DenseMatrix.OfArray(new double[,] { { 0 } });
			K = DenseMatrix.OfArray(new double[,] { { -0.15, -0.093 } });
			L = DenseMatrix.OfArray(new double[,] { { 18.22 }, { 67.54 } });
			debug1 = 0;
		}

		public void Init()
		{
			x = DenseMatrix.OfArray(new double[,] { { 300 }, { 0 } });
			lastTimestamp = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Distance">The current measured distance.</param>
		/// <param name="Target">The target distance.</param>
		/// <returns>Output signals</returns>
		public double Tick(double Distance, double Target)
		{
			debug1 = ((-K * x)[0, 0]);
			double feedback = ((-K * x)[0, 0]).Clamp(-9, 5);
			if (feedback > -1 && feedback < 1.5)
				feedback = 0;
			/*if (feedback > 0)
				feedback = 5;
			else if (feedback<0)
				feedback = -9;*/
			xDot = A * x + B * feedback + L * ((Distance - Target) - (C * x)[0,0]);
			if (lastTimestamp != 0)
			{
				double dt = ((double)DateTime.Now.Ticks - lastTimestamp) / TimeSpan.TicksPerSecond;
				x += xDot * dt;
			}
			lastTimestamp = DateTime.Now.Ticks;
			return feedback;//Math.Round((-K * x)[0, 0]).Clamp(-15, 15);
		}
		public String GetDebugInfo()
		{
			return x.ToString()+"\n"+debug1.ToString();
		}
	}
}
