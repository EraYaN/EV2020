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
	/// The model made with a worse fit in MATLAB, but truer to reality.
	/// </summary>
	class PDEpicModel : IModelND
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

		long lastTimestamp;

		public PDEpicModel()
		{
			A = DenseMatrix.OfArray(new double[,] { { 0, 1 }, { 0, -1.782 } });
			B = DenseMatrix.OfArray(new double[,] { { 0 }, { -39.94 } });
			C = DenseMatrix.OfArray(new double[,] { { 1, 0 } });
			D = DenseMatrix.OfArray(new double[,] { { 0 } });
			K = DenseMatrix.OfArray(new double[,] { { -0.15, -0.093 } });
			L = DenseMatrix.OfArray(new double[,] { { 18.22 }, { 67.54 } });			
		}
		/// <summary>
		/// This initializes the internal model state.
		/// </summary>
		/// <param name="X">Start X coordinate in meters</param>
		/// <param name="Y">StartY coordinate in meters</param>
		public void Init(double X, double Y)
		{
			//Start position = 50 cm and 50 cm and bearing 0.5 pi. (y+ direction)
			x = DenseMatrix.OfArray(new double[,] { { X*100, Y*100 }, { 0, 0 } });			
			lastTimestamp = 0;
		}

		/// <summary>
		/// Processes one step in the model.
		/// </summary>
		/// <param name="Position">The current measured posistion in meters (col vec).</param>
		/// <param name="Target">The target location in meters (col vec).</param>
		/// <returns>Output signals</returns>
		public Vector<double> Tick(Vector<double> Position, Vector<double> Target)
		{			
			//Calculate the feedback. And clamp the value to the maximum values to be send to the car the "back" value is higher for faster braking.
			Matrix<double> feedback = (-K * x).Clamp(-1, 1);

			// Dead zone. Non-symmetrical
			feedback = feedback.Deadzone(-1,1.5);

			//Calculate the change in x, xDot
			xDot = A * x + B * feedback + L * (DenseMatrix.OfRowVectors(new Vector<double>[] {(Position*100 - Target*100) - (C * x).Row(0)}));

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
			return feedback.Row(0);
		}
		public String GetDebugInfo()
		{
			return x.ToString();
		}
	}
}
