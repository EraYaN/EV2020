using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The model made with a worse fit in MATLAB, but truer to reality.
	/// </summary>
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
			//Start was 300 cm from the wall.
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

			//Calculate the feedback. And clamp the value to the maximum values to be send to the car the "back" value is higher for faster braking.
			double feedback = ((-K * x)[0, 0]).Clamp(-9, 5);

			// Dead zone. Non-symmetrical
			if (feedback > -1 && feedback < 1.5)
				feedback = 0;

			//Calculate the change in x, xDot
			xDot = A * x + B * feedback + L * ((Distance - Target) - (C * x)[0,0]);

			//Save the "now"
			long nowTimestamp = DateTime.Now.Ticks;
			//If lastTimestamp is not 0 then add the change to the x value times dt.
			if (lastTimestamp != 0)
			{

				double dt = ((double)nowTimestamp - lastTimestamp) / TimeSpan.TicksPerSecond;
				x += xDot * dt;
			}
			// Set new timestamp in ticks.
			lastTimestamp = nowTimestamp;
			//return the feedback value for sending to the car.
			return feedback;
		}
		public String GetDebugInfo()
		{
			return x.ToString()+"\n"+debug1.ToString();
		}
	}
}
