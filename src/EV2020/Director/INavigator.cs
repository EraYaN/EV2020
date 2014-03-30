﻿using System;
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
		double Tick(double Distance, double Target);
		string GetDebugInfo();
		bool GoToPosition(double pos);
		bool Pause();
		bool Resume();

	}
}
