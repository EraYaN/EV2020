using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.LocationSystem
{
	[Serializable]
	public class Microphone : ObservableObject
	{
		public Position3D Position
		{
			get;
			set;
		}
		public double X
		{
			get
			{
				if (Position != null)
				{
					return Position.X;
				}
				return 0;
			}
			set
			{
				if (Position == null)
				{
					Position = new Position3D();
				}
				Position.X = value;
			}
		}
		public double Y
		{
			get
			{
				if (Position != null)
				{
					return Position.Y;
				}
				return 0;
			}
			set
			{
				if (Position == null)
				{
					Position = new Position3D();
				}
				Position.Y = value;
			}
		}
		public double Z
		{
			get
			{
				if (Position != null)
				{
					return Position.Z;
				}
				return 0;
			}
			set
			{
				if (Position == null)
				{
					Position = new Position3D();
				}
				Position.Z = value;
			}
		}

		public Microphone()
		{
			Position = new Position3D();
			ChannelIndex = 0;
		}
		public int ChannelIndex { get; set; }
	}
}
