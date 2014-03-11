using EV2020.Communication;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EV2020.Director
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();			
			statusBarComPort.DataContext = Data.db;
			statusBarLastPing.DataContext = Data.db;
			statusBarEmergencyStop.DataContext = Data.db;
			statusBarFixedInputSequence.DataContext = Data.db;
		}
		private void comPortsComboBox_DropDownOpened(object sender, EventArgs e)
		{
			string[] ports = SerialPort.GetPortNames();
			comPortsComboBox.Items.Clear();
			foreach (string s in ports)
			{
				ComboBoxItem extra = new ComboBoxItem();
				extra.Content = s;

				if (Data.ComPort == s)
				{
					extra.IsSelected = true;
				}
				comPortsComboBox.Items.Add(extra);
			}
		}

		private void initButton_Click(object sender, RoutedEventArgs e)
		{
			//Init classes
			Data.nav = new Navigation();
			Data.vis = new Visualization(visCanvas, joystickCanvas);
			
			if (comPortsComboBox.SelectedItem != null && baudRateComboBox.SelectedItem != null)
			{
				Data.ComPort = (string)((ComboBoxItem)comPortsComboBox.SelectedItem).Content;
				Data.BaudRate = int.Parse((string)((ComboBoxItem)baudRateComboBox.SelectedItem).Content);
				if (Data.ComPort != "" && Data.BaudRate > 0)
				{
					Data.com = new SerialInterface(Data.ComPort, Data.BaudRate);
					int res = Data.com.OpenPort();
					Data.ctr = new Controller();
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
			/*if(Data.matlab==null)
				Data.matlab = new MATLABWrapper(@"H:\mcode","Groep B1");*/
			Data.db.UpdateProperty("InputSequence");
			Data.db.UpdateProperty("SerialPortStatus");
			Data.db.UpdateProperty("SerialPortStatusColor");
			Data.db.UpdateProperty("EmergencyStop");
							
			//enable buttons	
			initButton.IsEnabled = false;
			destroyButton.IsEnabled = true;
			comPortsComboBox.IsEnabled = false;
			baudRateComboBox.IsEnabled = false;
			//update vis
			Data.vis.drawJoystick();
		}

		private void destroyButton_Click(object sender, RoutedEventArgs e)
		{
			initButton.IsEnabled = true;
			destroyButton.IsEnabled = false;
			comPortsComboBox.IsEnabled = true;
			baudRateComboBox.IsEnabled = true;
			Data.ctr = null;
			Data.vis = null;
			Data.nav = null;
			if (Data.matlab != null)
			{
				Data.matlab.CloseAll();
				Data.matlab.Dispose();
				Data.matlab = null;
			}
			Data.com.Dispose();
			Data.com = null;
		}

		private void getStatusButton_Click(object sender, RoutedEventArgs e)
		{
			Data.ctr.GetStatus();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				Data.ctr.Stop();
				Data.ctr.Center();
			}			
		}

		private void joystickCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			updateJoystick(e);
			if (Data.ctr == null)
				return;
			if (Data.ctr.IsEmergencyStop)
			{
				Data.ctr.ResetEmergencyStop();
			}
		}

		private void updateJoystick(MouseEventArgs e)
		{
			if (Data.ctr == null)
				return;
			Point pos = e.GetPosition(joystickCanvas);		
			double centerH = joystickCanvas.ActualHeight / 2;
			double centerW = joystickCanvas.ActualWidth / 2;
			Data.ctr.SetDrivingSteering((int)Math.Round(-(pos.Y - centerH) / (centerH / 15)), -(int)Math.Round((pos.X - centerW) / (centerW / 50)));
			Data.vis.drawJoystick();
		}

		private void joystickCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (Data.ctr == null)
				return;
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				updateJoystick(e);
			}
		}

		private void joystickCanvas_MouseLeave(object sender, MouseEventArgs e)
		{
			/*if (Data.ctr == null)
				return;
			Data.ctr.Stop();
			Data.ctr.Center();
			Data.vis.drawJoystick();*/
		}

		private void joystickCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (Data.ctr == null)
				return; 
			Data.ctr.Stop();
			Data.ctr.Center();
			Data.vis.drawJoystick();
		}

		private void testPlotButton_Click(object sender, RoutedEventArgs e)
		{
			double[] x = { 1, 2, 3, 4, 5 };
			double[] y = { 2, 4, 6, 8, 1 };
			Data.matlab.plot(x, y, "Titel", "X (label)", "Y (label)", "Data legenda");
		}

		private void sendPulseButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.ctr == null)
				return;
			InputSequence fis = InputSequence.Pulse(100, 0, 25, 5, 0, 0, 0, 0, 0);
			Data.ctr.StartFixedInputSequence(ref fis);
		}				
	}
}
