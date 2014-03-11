using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EV2020.Director
{
	public class Databindings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;		

		public string SerialPortStatus
		{
			get
			{
				if (Data.com == null)
					return "NULL";
				if (Data.com.IsOpen)
				{
					return Data.com.BytesInRBuffer + "|" + Data.com.BytesInTBuffer;
				}
				else
				{
					return "NC";
				}
			}
		}

		public string LastPing
		{
			get
			{
				if (Data.ctr != null)
				{
					if (Data.ctr.LastPing == -1)
						return "xx ms";
					else
						return Data.ctr.LastPing + " ms";
				}
				else
				{
					return "ctr is null";
				}
			}
		}

		public Brush SerialPortStatusColor
		{
			get
			{
				if (Data.com == null)
					return Brushes.Red;
				if (Data.com.IsOpen)
				{
					int b = Data.com.BytesInRBuffer + Data.com.BytesInTBuffer;
					if (b == 0)
					{
						return Brushes.Green;
					}
					else if (b > 0 && b <= 2)
					{
						return Brushes.LightGreen;
					}
					else
					{
						return Brushes.Orange;
					}
				}
				else
				{
					return Brushes.OrangeRed;
				}
			}
		}

		public String EmergencyStop
		{
			get
			{
				if(Data.ctr == null)
					return "ctr is null";
				if (Data.ctr.IsEmergencyStop)
				{
					return "Emergency!";
				}
				else
				{
					return String.Format("OK. ({0})",Data.ctr.EmergencyErrorCount);
				}
			}
		}

		public String FixedInputSequence
		{
			get
			{
				if (Data.ctr == null)
					return "ctr is null";
				if (Data.ctr.IsFixedInputSequenceExecuting)
				{
					return String.Format("Fixed Input {1} of {0}",Data.ctr.FixedInputSequenceExecutingLength, Data.ctr.FixedInputSequenceExecutingIndex);
				}
				else
				{
					return "No Fixed Input";
				}
			}
		}

		public void UpdateProperty(string name)
		{
			OnPropertyChanged(name);
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
