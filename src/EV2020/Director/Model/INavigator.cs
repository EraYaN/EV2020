using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The interface for the navigation classes.
	/// This will be used in the future, instead of the constant in the Controller class.
	/// </summary>
	public interface INavigator
	{
		void Init();
		CarCommand Tick(double LeftSensor, double RightSensor);
		string GetDebugInfo();
		bool GoToPosition(Matrix<double> pos);
		bool Pause();
		bool Resume();
	}
}
