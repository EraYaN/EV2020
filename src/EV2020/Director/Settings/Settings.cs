using BlueWave.Interop.Asio;
using EV2020.LocationSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace EV2020.Director
{
	[Serializable]
	public class Settings : ISerializable
	{
		[System.ComponentModel.DefaultValueAttribute("COM10")]
		public string Comport
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(115200)]
		public int BaudRate
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(1.0)]
		public double FieldWidth
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(1.0)]
		public double FieldHeight
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(1.0)]
		public double FieldMargin
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(2400)]
		public double SampleWindow
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(0.25)]
		public double SampleLength
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(0.28)]
		public double BeaconHeight
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(0)]
		public double SmoothFactor
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool MatchedFilterEnabled
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool MatchedFilterToep
		{
			get;
			set;
		}
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool UseMeasuredSignal
		{
			get;
			set;
		}
		ObservableCollection<Microphone> _microphones = new ObservableCollection<Microphone>();
		public ObservableCollection<Microphone> Microphones
		{
			get
			{
				return _microphones;
			}
			set
			{
				_microphones = value;
			}
		}

		public String SelectedDriver
		{
			get;
			set;
		}
		public Settings()
		{
			_microphones = new ObservableCollection<Microphone>();
		}
		protected Settings(SerializationInfo info, StreamingContext context)
		{
			try { Comport = info.GetString("Comport"); }
			catch { }
			try { BaudRate = info.GetInt32("BaudRate"); }
			catch { }
			try { SelectedDriver = info.GetString("SelectedDriver"); }
			catch { }
			try { FieldWidth = info.GetDouble("FieldWidth"); }
			catch { }
			try { FieldHeight = info.GetDouble("FieldHeight"); }
			catch { }
			try { FieldMargin = info.GetDouble("FieldMargin"); }
			catch { }
			try { SampleWindow = info.GetDouble("SampleWindow"); }
			catch { }
			try { SampleLength = info.GetDouble("SampleLength"); }
			catch { }
			try { BeaconHeight = info.GetDouble("BeaconHeight"); }
			catch { }
			try { SmoothFactor = info.GetDouble("SmoothFactor"); }
			catch { }
			try { MatchedFilterEnabled = info.GetBoolean("MatchedFilterEnabled"); }
			catch { }
			try { MatchedFilterToep = info.GetBoolean("MatchedFilterToep"); }
			catch { }
			try { UseMeasuredSignal = info.GetBoolean("UseMeasuredSignal"); }
			catch { }
			try
			{
				ObservableCollection<Microphone> temp = new ObservableCollection<Microphone>();
				temp = (ObservableCollection<Microphone>)info.GetValue("Microphones", temp.GetType());
				if (temp != null)
				{
					_microphones = temp;
					temp = null;
				}
			}
			catch { }
		}
		[SecurityPermissionAttribute(SecurityAction.Demand,
		SerializationFormatter = true)]

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Comport", Comport);
			info.AddValue("BaudRate", BaudRate);
			info.AddValue("SelectedDriver", SelectedDriver);
			info.AddValue("FieldWidth", FieldWidth);
			info.AddValue("FieldHeight", FieldHeight);
			info.AddValue("FieldMargin", FieldMargin);
			info.AddValue("SampleWindow", SampleWindow);
			info.AddValue("SampleLength", SampleLength);
			info.AddValue("BeaconHeight", BeaconHeight);
			info.AddValue("SmoothFactor", SmoothFactor);
			info.AddValue("MatchedFilterEnabled", MatchedFilterEnabled);
			info.AddValue("MatchedFilterToep", MatchedFilterToep);
			info.AddValue("UseMeasuredSignal", UseMeasuredSignal);
			info.AddValue("Microphones", Microphones);
		}
	}
}
