using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public interface IModelND
	{
		void Init(double X, double Y);
		Matrix<double> Tick(Matrix<double> Distance, Matrix<double> Target);
		string GetDebugInfo();
		
	}
}
