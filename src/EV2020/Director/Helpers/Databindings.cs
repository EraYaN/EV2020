using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EV2020.Director
{
	/// <summary>
	/// Class used as a custom "ViewModel" without actually being a ViewModel.
	/// </summary>
	public class Databindings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string VersionInfo
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly a in assemblies)
				{
					AssemblyName an = a.GetName();
					if(a.GlobalAssemblyCache==false)
						sb.AppendLine(String.Format("{0} v{1} {2}", an.Name, an.Version, an.ProcessorArchitecture));					
				}
				
				return sb.ToString();
			}
		}
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
					return String.Format("Fixed Input {1} of {0}",Data.ctr.FixedInputSequenceExecutingLength, Data.ctr.FixedInputSequenceExecutingIndex+1);
				}
				else
				{
					return "No Fixed Input";
				}
			}
		}

		public List<DataPoint> BatteryGraphPoints
		{
			get{
				List<DataPoint> l = new List<DataPoint>();
				if (Data.ctr == null)
					return l;

				double x = 0;
				foreach (double d in Data.ctr.OutputBatteryVoltageHistory.Data){
					l.Add(new DataPoint(x, d));
					x += (double)Controller.TimerPeriod / 1000.0;
				}
				return l;
			}
		}
		public double BatteryGraphMaxTime
		{
			get
			{
				if (Data.ctr == null)
					return (double)Controller.BatteryHistoryPoints * (double)Controller.TimerPeriod / 1000.0;

				return (double)Controller.TimerPeriod / 1000.0 * Data.ctr.OutputBatteryVoltageHistory.Length;
			}
		}
		public double DistanceGraphMaxTime
		{
			get
			{
				if (Data.ctr == null)
					return (double)Controller.DistanceHistoryPoints * (double)Controller.TimerPeriod / 1000.0;

				return (double)Controller.TimerPeriod / 1000.0 * Data.ctr.DistanceHistory.Length;
			}
		}
		public List<DataPoint> DistanceLeftGraphPoints
		{
			get
			{
				List<DataPoint> l = new List<DataPoint>();
				if (Data.ctr == null)
					return l;

				double x = 0;
				foreach (double d in Data.ctr.DistanceHistory.LeftData)
				{
					l.Add(new DataPoint(x, d));
					x += (double)Controller.TimerPeriod / 1000.0;
				}
				return l;
			}
		}
		public List<DataPoint> DistanceRightGraphPoints
		{
			get
			{
				List<DataPoint> l = new List<DataPoint>();
				if (Data.ctr == null)
					return l;

				double x = 0;
				foreach (double d in Data.ctr.DistanceHistory.RightData)
				{
					l.Add(new DataPoint(x, d));
					x += (double)Controller.TimerPeriod / 1000.0;
				}
				return l;
			}
		}
		public double ControlGraphMaxTime
		{
			get
			{
				if (Data.ctr == null)
					return (double)Controller.ControlHistoryPoints * (double)Controller.TimerPeriod / 1000.0;

				return (double)Controller.TimerPeriod / 1000.0 * Data.ctr.ControlHistory.Length;
			}
		}
		public List<DataPoint> ControlDrivingGraphPoints
		{
			get
			{
				List<DataPoint> l = new List<DataPoint>();
				if (Data.ctr == null)
					return l;

				double x = 0;
				foreach (double d in Data.ctr.ControlHistory.LeftData)
				{
					l.Add(new DataPoint(x, d));
					x += (double)Controller.TimerPeriod / 1000.0;
				}
				return l;
			}
		}
		public List<DataPoint> ControlSteeringGraphPoints
		{
			get
			{
				List<DataPoint> l = new List<DataPoint>();
				if (Data.ctr == null)
					return l;

				double x = 0;
				foreach (double d in Data.ctr.ControlHistory.RightData)
				{
					l.Add(new DataPoint(x, d));
					x += (double)Controller.TimerPeriod / 1000.0;
				}
				return l;
			}
		}

		public String ModelDebugInfo
		{
			get
			{
				if (Data.nav != null)
				{
					return Data.nav.GetDebugInfo();
				}
				else
				{
					return "nav is null";
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
