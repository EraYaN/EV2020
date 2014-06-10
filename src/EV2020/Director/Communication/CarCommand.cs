using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public class CarCommand
	{
		/// <summary>
		/// The amount of driving command
		/// </summary>
		public double Driving;
		/// <summary>
		/// The amount of steering command
		/// </summary>
		public double Steering;
		/// <summary>
		/// Constructor which intializes with the given driving and steering
		/// </summary>
		/// <param name="driving">The driving command to be initialized with</param>
		/// <param name="steering">The steering command to be initialized with</param>
		public CarCommand(double driving, double steering)
		{
			Driving = driving;
			Steering = steering;
		}
	}
}
