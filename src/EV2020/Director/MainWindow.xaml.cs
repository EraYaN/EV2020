using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EV2020.Communication;
using MLE4WCSharp;
using System.IO.Ports;

namespace EV2020.Director
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		String ComPort;
		long ping;
		public String Ping
		{
			get { return ping + " ms"; }			
		}
		public MainWindow()
		{
			InitializeComponent();
			ping = 35;
		}
		private void comPortsComboBox_DropDownOpened(object sender, EventArgs e)
		{
			string[] ports = SerialPort.GetPortNames();
			comPortsComboBox.Items.Clear();
			foreach (string s in ports)
			{
				ComboBoxItem extra = new ComboBoxItem();
				extra.Content = s;

				if (ComPort == s)
				{
					extra.IsSelected = true;
				}
				comPortsComboBox.Items.Add(extra);
			}
		}

		private void initButton_Click(object sender, RoutedEventArgs e)
		{
			//Init classes
			
			if (comPortsComboBox.SelectedItem != null && baudRateComboBox.SelectedItem != null)
			{
				Data.ComPort = (string)((ComboBoxItem)comPortsComboBox.SelectedItem).Content;
				Data.BaudRate = int.Parse((string)((ComboBoxItem)baudRateComboBox.SelectedItem).Content);
				if (Data.ComPort != "" && Data.BaudRate > 0)
				{
					Data.com = new SerialInterface(Data.ComPort, Data.BaudRate);
					int res = Data.com.OpenPort();					
					if (res != 0)
						MessageBox.Show("SerialInterface Error: #" + res + "\n" + Data.com.lastError, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					MessageBox.Show("COM Port or Baud Rate not valid.", "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				MessageBox.Show("No COM Port or Baud Rate chosen.", "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			//enable buttons	
			initButton.IsEnabled = false;
			destroyButton.IsEnabled = true;
			comPortsComboBox.IsEnabled = false;
			baudRateComboBox.IsEnabled = false;			
		}

		private void destroyButton_Click(object sender, RoutedEventArgs e)
		{
			initButton.IsEnabled = true;
			destroyButton.IsEnabled = false;
			comPortsComboBox.IsEnabled = true;
			baudRateComboBox.IsEnabled = true;
			Data.com.Dispose();
			Data.com = null;
		}
	}
}
