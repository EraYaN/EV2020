using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The model interface.
	/// </summary>
	public interface IModel
	{
		void Init();	
		double Tick(double Distance, double Target);
		string GetDebugInfo();
	}
}
