using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// A class to make it easier to store the input signals.
	/// </summary>
	public class InputSequence
	{
		Sequence driving;
		Sequence steering;
		double multiplier;

		public long Index
		{
			get
			{
				return driving.Index;
			}
		}
		public long Length
		{
			get
			{
				return driving.Length;
			}
		}
		public bool IsEndOfSignal
		{
			get
			{
				return driving.IsEndOfSignal || driving.IsEndOfSignal;
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Driving">The driving command</param>
		/// <param name="Steering">The steering command</param>
		/// <param name="Multiplier">Multiplier on GetCurrentDriving and GetCurrentSteering values</param>
		public InputSequence(Sequence Driving, Sequence Steering, double Multiplier=1)
		{
			if (Driving.Length != Steering.Length)
				throw new ArgumentException("The length of the two sequences must be the same.");
			driving = Driving;
			steering = Steering;
			//sync Sequences
			driving.Rewind();
			steering.Rewind();
			multiplier = Multiplier;
		}
		/// <summary>
		/// The last measured driving from the car
		/// </summary>
		/// <returns></returns>
		public double GetCurrentDriving()
		{
			return (driving.DataElement * multiplier).Clamp(Controller.DrivingMin, Controller.DrivingMax);
		}
		/// <summary>
		/// The last measured steering from the car
		/// </summary>
		/// <returns></returns>
		public double GetCurrentSteering()
		{
			return (steering.DataElement * multiplier).Clamp(Controller.SteeringMin, Controller.SteeringMax);
		}
		/// <summary>
		/// Shift the index in the internal sequence one further
		/// </summary>
		public void Forward()
		{
			driving.Forward();
			steering.Forward();
		}
		/// <summary>
		/// Shift the index in the internal sequence one back
		/// </summary>
		public void Backward()
		{
			driving.Backward();
			steering.Backward();
		}
		/// <summary>
		/// Shift the index in the internal sequence to the start
		/// </summary>
		public void Rewind()
		{
			driving.Rewind();
			steering.Rewind();
		}
		
	}
}
