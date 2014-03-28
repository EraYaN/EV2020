﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
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
