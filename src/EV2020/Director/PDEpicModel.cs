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
		readonly double Nbar = -0.225338007010516;

		Matrix<double> x;
		Matrix<double> xDot;

		long lastTimestamp;

		public PDEpicModel(){
			A = DenseMatrix.OfArray(new double[,] 
        { 
            //{ -1.540497550593680, 0.499996085982763 },
            //{ -0.050981466007821, -1.221182449406320 }
		   { 0, 1 },
           { 0, 1.782 }
        });
		B = DenseMatrix.OfArray(new double[,] 
        {
            { 0 },
            { -39.94 } 
        });
		C = DenseMatrix.OfArray(new double[,] 
        { 
            { 1, 0 } 
        });
		D = DenseMatrix.OfArray(new double[,] 
        { 
            { 0 }
        });
		K = DenseMatrix.OfArray(new double[,] 
        { 
            //{ -0.064057182010178, -0.020791231841726 }
			{ -0.225338007010516, -0.143164747120681 }
        });
		L = DenseMatrix.OfArray(new double[,] 
        { 
            { 18.218 }, 
            { 67.535524 } 
        });

		}

		public void Init()
		{
			x = DenseMatrix.OfArray(new double[,] { { 0 }, { 0 } });
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
			double feedback = Math.Round((-K * x)[0, 0]).Clamp(-10, 10);
			//Matrix<double> u = DenseMatrix.OfArray(new double[,] { { TargetScaled } }) + feedback;
			xDot = A * x + B * feedback + L * (DenseMatrix.OfArray(new double[,] { { Distance - Target } }) - C * x);
			if (lastTimestamp != 0)
			{
				double dt = ((double)DateTime.Now.Ticks - lastTimestamp) / TimeSpan.TicksPerSecond;
				x += xDot * dt;
			}
			lastTimestamp = DateTime.Now.Ticks;			
			return Math.Round((-K * x)[0, 0]).Clamp(-10, 10);
		}
		public String GetDebugInfo()
		{
			return x.ToString();
		}
	}
}
