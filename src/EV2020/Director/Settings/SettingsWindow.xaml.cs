using BlueWave.Interop.Asio;
using EV2020.LocationSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EV2020.Director
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{		
		public SettingsWindow()
		{			
			InitializeComponent();
			//Read out available comports 
			string[] ports = SerialPort.GetPortNames();
			foreach (string s in ports)
			{				
				((ConfigurationViewModel)DataContext).ComPorts.Add(s);
			}
			//DataContext = this;
		}
		private void SettingsDialogCloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}

		private void openControlPanelButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.cfg != null)
			{
				InstalledDriver id = AsioDriver.InstalledDrivers.Find(Data.cfg.SelectedDriver);
				if (id != null)
				{
					try
					{
						AsioDriver driver = AsioDriver.SelectDriver(id);
						driver.ShowControlPanel();
						driver.Release();
					}
					catch
					{

					}
				}
			}
		}

		private void addMicrophoneButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.cfg != null)
			{
				Data.cfg.Microphones.Add(new Microphone());
			}
		}

		private void removeMicrophoneButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.cfg != null)
			{
				if(microphoneListView.SelectedItem!=null)
					Data.cfg.Microphones.Remove((Microphone)microphoneListView.SelectedItem);
			}
		}
	}
}
