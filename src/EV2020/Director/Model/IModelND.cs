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
		/// <summary>
		/// Interface for the 2D location algorithm
		/// </summary>
		/// <param name="X">Initial car X position</param>
		/// <param name="Y">Initial car Y position</param>
		void Init(double X, double Y);
		/// <summary>
		/// Location tick/calculation step
		/// </summary>
		/// <param name="Position"></param>
		/// <param name="Target"></param>
		/// <returns></returns>
		Vector<double> Tick(Vector<double> Position, Vector<double> Target);
		/// <summary>
		/// Debug purpose
		/// </summary>
		/// <returns></returns>
		string GetDebugInfo();
		
	}
}
