﻿using MathNet.Numerics.LinearAlgebra;
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
		Vector<double> CarPos { get; }
		double CarX { get; }
		double CarY { get; }
		double TargetX { get; }
		double TargetY { get; }
		double CarBearing { get; }
		void Init();
		CarCommand Tick(double LeftSensor, double RightSensor);
		string GetDebugInfo();
		void GoToPosition(Vector<double> pos);
		bool Pause();
		bool Resume();
	}
}
