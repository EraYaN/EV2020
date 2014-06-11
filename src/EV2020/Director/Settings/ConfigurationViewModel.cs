using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;
using BlueWave.Interop.Asio;
using EV2020.LocationSystem;

namespace EV2020.Director
{
	/// <summary>
	/// The ViewModel for the Settings Dialog
	/// </summary>
	public class ConfigurationViewModel : ObservableObject
	{
		#region Construction
        /// <summary>
        /// Constructs the default instance of a SongViewModel
        /// </summary>
		public ConfigurationViewModel()
        {
           // _song = new Song { ArtistName = "Unknown", SongTitle = "Unknown" };
        }
        #endregion		
		
		#region Members
		ObservableCollection<string> _comPorts = new ObservableCollection<string>();
		ObservableCollection<int> _baudRates = new ObservableCollection<int>() { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000 };
		ObservableCollection<InstalledDriver> _installedDrivers = new ObservableCollection<InstalledDriver>(AsioDriver.InstalledDrivers);
		#endregion

		#region Properties
		public ObservableCollection<string> Comports
		{
			get
			{
				return _comPorts;
			}
			set
			{
				_comPorts = value;
			}
		}

		public ObservableCollection<InstalledDriver> InstalledDrivers
		{
			get
			{
				return _installedDrivers;
			}			
		}

		public ObservableCollection<int> BaudRates
		{
			get
			{
				return _baudRates;
			}
			set
			{
				_baudRates = value;
			}
		}
		public string CurrentComport
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.Comport;
				}
				return String.Empty;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.Comport != value)
					{
						Data.cfg.Comport = value;
						RaisePropertyChanged("CurrentComport");
					}
				}
			}
		}
		public int CurrentBaudRate
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.BaudRate;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.BaudRate != value)
					{
						Data.cfg.BaudRate = value;
						RaisePropertyChanged("CurrentBaudRate");
					}
				}
			}
		}
		public InstalledDriver CurrentDriver
		{
			get
			{
				if (Data.cfg != null)
				{
					return AsioDriver.InstalledDrivers.Find(Data.cfg.SelectedDriver);
				}
				return null;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (AsioDriver.InstalledDrivers.Find(Data.cfg.SelectedDriver) != value)
					{
						Data.cfg.SelectedDriver = value.Name;
						RaisePropertyChanged("CurrentDriver");
					}
				}
			}
		}
		public double FieldWidth
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.FieldWidth;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.FieldWidth != value)
					{
						Data.cfg.FieldWidth = value;
						RaisePropertyChanged("FieldWidth");
					}
				}
			}
		}
		public double FieldHeight
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.FieldHeight;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.FieldHeight != value)
					{
						Data.cfg.FieldHeight = value;
						RaisePropertyChanged("FieldHeight");
					}
				}
			}
		}
		public double FieldMargin
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.FieldMargin;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.FieldMargin != value)
					{
						Data.cfg.FieldMargin = value;
						RaisePropertyChanged("FieldMargin");
					}
				}
			}
		}
		public double SampleWindow
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.SampleWindow;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.SampleWindow != value)
					{
						Data.cfg.SampleWindow = value;
						RaisePropertyChanged("SampleWindow");
					}
				}
			}
		}
		public double SampleLength
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.SampleLength;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.SampleLength != value)
					{
						Data.cfg.SampleLength = value;
						RaisePropertyChanged("SampleLength");
					}
				}
			}
		}
		public double BeaconHeight
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.BeaconHeight;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.BeaconHeight != value)
					{
						Data.cfg.BeaconHeight = value;
						RaisePropertyChanged("BeaconHeight");
					}
				}
			}
		}
		public double SmoothFactor
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.SmoothFactor;
				}
				return 0;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.SmoothFactor != value)
					{
						Data.cfg.SmoothFactor = value;
						RaisePropertyChanged("SmoothFactor");
					}
				}
			}
		}
		public bool MatchedFilterEnabled
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.MatchedFilterEnabled;
				}
				return false;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.MatchedFilterEnabled != value)
					{
						Data.cfg.MatchedFilterEnabled = value;
						RaisePropertyChanged("MatchedFilterEnabled");
					}
				}
			}
		}
		public bool MatchedFilterToep
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.MatchedFilterToep;
				}
				return false;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.MatchedFilterToep != value)
					{
						Data.cfg.MatchedFilterToep = value;
						RaisePropertyChanged("MatchedFilterToep");
					}
				}
			}
		}
		public bool UseMeasuredSignal
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.UseMeasuredSignal;
				}
				return false;
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.UseMeasuredSignal != value)
					{
						Data.cfg.UseMeasuredSignal = value;
						RaisePropertyChanged("UseMeasuredSignal");
					}
				}
			}
		}
		public ObservableCollection<Microphone> Microphones
		{
			get
			{
				if (Data.cfg != null)
				{
					return Data.cfg.Microphones;
				}
				return new ObservableCollection<Microphone>();
			}
			set
			{
				if (Data.cfg != null)
				{
					if (Data.cfg.Microphones != value)
					{
						Data.cfg.Microphones = value;
						RaisePropertyChanged("Microphones");
					}
				}
			}
		}		
		#endregion

	}
}
