using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public class CarCommand
	{
		public double Driving;
		public double Steering;
		public CarCommand(double driving, double steering)
		{
			Driving = driving;
			Steering = steering;
		}
	}
}
