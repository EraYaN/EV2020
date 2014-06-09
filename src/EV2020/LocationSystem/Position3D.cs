using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.LocationSystem
{
	[Serializable]
	public class Position3D : ObservableObject
	{
		double _x;
		double _y;
		double _z;
		public double X
		{
			get
			{
				return _x;
			}
			set
			{
				if (_x != value)
				{
					_x = value;
					RaisePropertyChanged("X");
				}
			}
		}
		public double Y
		{
			get
			{
				return _y;
			}
			set
			{
				if (_y != value)
				{
					_y = value;
					RaisePropertyChanged("Y");
				}
			}
		}
		public double Z
		{
			get
			{
				return _z;
			}
			set
			{
				if (_z != value)
				{
					_z = value;
					RaisePropertyChanged("Z");
				}
			}
		}

		public double Magnitude
		{
			get
			{
				return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
			}
		}

		public static Position3D operator -(Position3D x, Position3D y)
		{
			x.X -= y.X;
			x.Y -= y.Y;
			x.Z -= y.Z;
			return x;
		}
		public static Position3D operator +(Position3D x, Position3D y)
		{
			x.X += y.X;
			x.Y += y.Y;
			x.Z += y.Z;
			return x;
		}
		public static double GetMagnitude(Position3D x)
		{
			return x.Magnitude;
		}
		public override string ToString()
		{
			return "Posistion3D: " + X.ToString("F3") + "; " + Y.ToString("F3") + "; " + Z.ToString("F3");
		}
	}
}
