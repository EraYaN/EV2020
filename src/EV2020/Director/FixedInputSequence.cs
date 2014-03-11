using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public class FixedInputSequence
	{
		long lenght;
		public long Length
		{
			get { return Length; }
		}
		Decimal[] driving;
		Decimal[] steering;
		Decimal multiplier;
		long index;
		public long Index
		{
			get { return index; }
		}
		public bool IsEndOfSignal
		{
			get { return !(index<lenght); }
		}
		public bool IsStartOfSignal
		{
			get { return index == 0; }
		}
		public FixedInputSequence(Decimal[] _driving, Decimal[] _steering, long offset = 0, Decimal Multiplier=1)
		{
			lenght = Math.Min(_driving.LongLength,_steering.LongLength);
			if (lenght<0)
				throw new ArgumentException("Lenght is smaller than zero.", "_lenght");
			if (offset > lenght)
				throw new ArgumentException("Offset is smaller than lenght.", "offset");			
			driving = _driving;
			steering = _steering;
			index = offset;
			multiplier = Multiplier;
		}		
		public Decimal GetCurrentDriving()
		{
			return (driving[index] * multiplier).Clamp(Controller.DrivingMin, Controller.DrivingMax);
		}
		public Decimal GetCurrentSteering()
		{
			return (steering[index] * multiplier).Clamp(Controller.SteeringMin,Controller.SteeringMax);
		}
		public static FixedInputSequence operator ++(FixedInputSequence fis)
		{
			if (fis.index < fis.lenght-1)
				fis.index++;
			return fis;
		}
		public static FixedInputSequence operator --(FixedInputSequence fis)
		{
			if (fis.index > 0)
				fis.index--;
			return fis;
		}
		public static FixedInputSequence Linear(long samples, Decimal DrivingStart, Decimal DrivingEnd, Decimal SteeringStart, Decimal SteeringEnd)
		{
			Decimal[] driving = new Decimal[samples];
			Decimal[] steering = new Decimal[samples];
			Decimal DrivingCoEff = (DrivingEnd-DrivingStart)/samples;
			Decimal SteeringCoEff = (SteeringEnd-SteeringStart)/samples;
			for (long I = 0; I < samples; I++)
			{
				driving[I] = I * DrivingCoEff + DrivingStart;
				steering[I] = I * SteeringCoEff + SteeringStart;
			}
			return new FixedInputSequence(driving,steering,0);
		}
		public static FixedInputSequence Pulse(long samples, long pulseSamplesDrivingStart, long pulseSamplesDrivingEnd, Decimal DrivingHigh, Decimal DrivingLow, long pulseSamplesSteeringStart, long pulseSamplesSteeringEnd, Decimal SteeringHigh, Decimal SteeringLow)
		{
			Decimal[] driving = new Decimal[samples];
			Decimal[] steering = new Decimal[samples];
			for (long I = 0; I < samples; I++)
			{
				if (I >= pulseSamplesDrivingStart && I <= pulseSamplesDrivingEnd)
				{
					driving[I] = DrivingHigh;
				}
				else
				{
					driving[I] = DrivingLow;
				}
				if (I >= pulseSamplesSteeringStart && I <= pulseSamplesSteeringEnd)
				{
					steering[I] = SteeringHigh;
				}
				else
				{
					steering[I] = SteeringLow;
				}
			}
			return new FixedInputSequence(driving, steering, 0);
		}
	}
}
