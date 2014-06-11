using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EV2020.Director
{
	public enum ControllerState
	{
		Unknown,
		Idle,
		Charging,
		Navigating,
		EmergencyStop,
		Error
	}
}
