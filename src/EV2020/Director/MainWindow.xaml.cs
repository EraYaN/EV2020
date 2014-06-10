using EV2020.Communication;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OxyPlot;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using System.Diagnostics;
using BlueWave.Interop.Asio;
using MathNet.Numerics.Providers.LinearAlgebra.Mkl;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace EV2020.Director
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		SettingsWindow settingsWindow;
		public MainWindow()
		{
			this.DataContext = Data.db;
			InitializeComponent();
			/*statusBarComport.DataContext = Data.db;
			statusBarLastPing.DataContext = Data.db;
			statusBarEmergencyStop.DataContext = Data.db;
			statusBarFixedInputSequence.DataContext = Data.db;*/
			MathNet.Numerics.Control.LinearAlgebraProvider = new MklLinearAlgebraProvider();
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			if (File.Exists("cfg.bin"))
			{
				try
				{
					IFormatter formatter = new BinaryFormatter();
					using (Stream stream = new FileStream("cfg.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						Data.cfg = (Settings)formatter.Deserialize(stream);
					}
				}
				catch
				{
					Data.cfg = new Settings();
				}
				finally
				{
				}
			}
			else
			{
				Data.cfg = new Settings();
			}
			settingsWindow = new SettingsWindow();
			Data.vis = new Visualization(visCanvas, joystickCanvas);
			Data.vis.drawField();
		}

		private void initButton_Click(object sender, RoutedEventArgs e)
		{
			initButton.IsEnabled = false;
			initButton.Refresh();
			//Init classes				
			Data.nav = new StandardNavigation();			

			if (!String.IsNullOrEmpty(Data.cfg.Comport) && Data.cfg.BaudRate > 0)
			{
				Data.com = new SerialInterface(Data.cfg.Comport, Data.cfg.BaudRate);
				int res = Data.com.OpenPort();
				Data.ctr = new Controller();
				if (res != 0)
					MessageBox.Show("SerialInterface Error: #" + res + "\n" + Data.com.lastError, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				MessageBox.Show("COM Port or Baud Rate not valid.", "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			//MATLAB is for now not needed.
			/*if(Data.matlab==null)
				Data.matlab = new MATLABWrapper(@"fieldHeight:\mcode","Groep B1");*/
			Data.db.UpdateProperty(String.Empty); //Update All Bindings			

			//enable buttons				
			destroyButton.IsEnabled = true;
			//update vis
			Data.vis.drawJoystick();
		}

		private void destroyButton_Click(object sender, RoutedEventArgs e)
		{
			destroyButton.IsEnabled = false;
			destroyButton.Refresh();
			initButton.IsEnabled = true;
			Data.ctr = null;	
			if (Data.nav != null)
				Data.nav = null;
			if (Data.matlab != null)
			{
				Data.matlab.CloseAll();
				Data.matlab.Dispose();
				Data.matlab = null;
			}
			if (Data.com != null)
				Data.com.Dispose();
			Data.com = null;
			Data.db.UpdateProperty(String.Empty); //Update All Bindings
		}

		private void getStatusButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.ctr != null)
				Data.ctr.GetStatus();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				Data.ctr.Center();
				Data.ctr.Brake();
				//Data.ctr.Stop();

			}
			if (e.Key == Key.S)
			{
				if (Data.ctr != null)
				{
					using (StreamWriter sw = new StreamWriter(String.Format("data{0}.txt", DateTime.Now.Ticks)))
					{
						sw.WriteLine("input, steering, datal, datar");
						int len = Data.ctr.ControlHistory.LeftData.Length;
						for (int I = len - 1; I >= 0; I--)
						{
							sw.WriteLine("{0}, {1}, {2}, {3}", Data.ctr.ControlHistory.LeftData[I], Data.ctr.ControlHistory.RightData[I], Data.ctr.DistanceHistory.LeftData[I], Data.ctr.DistanceHistory.RightData[I]);
						}
					}
				}
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

		private void sendPulseButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.ctr == null)
				return;
			InputSequence fis = new InputSequence(Sequence.Pulse(100, 0, 20, -10, 0), new Sequence(100));
			Data.ctr.StartFixedInputSequence(ref fis);
		}

		private void sendWaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.ctr == null)
				return;
			Sequence s1 = Sequence.Pulse(40, 0, 15, 5, 0);
			Sequence s2 = Sequence.Pulse(40, 0, 15, -9, 0);
			Sequence s = new Sequence();
			s.Append(s1);
			s.Append(s2);
			s.Append(s1);
			s.Append(s2);
			s.Append(s1);
			s.Append(s2);
			InputSequence fis = new InputSequence(s, new Sequence(s.Length));
			Data.ctr.StartFixedInputSequence(ref fis);
		}

		private void ToggleAudioBeaconMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if(Data.ctr!=null)
				Data.ctr.ToggleAudio();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			settingsWindow.Close();
			settingsWindow = null;
			IFormatter formatter = new BinaryFormatter();
			using (Stream stream = new FileStream("cfg.bin", FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				formatter.Serialize(stream, Data.cfg);
			}
		}

		private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			settingsWindow.Show();
			settingsWindow.Focus();
		}

		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private void RedrawFieldMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Data.vis != null)
				Data.vis.drawField();
		}

		private void setTargetButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.nav != null)
			{
				try
				{
					Data.nav.GoToPosition(DenseVector.OfArray(new double[] { Convert.ToDouble(TargetX.Text), Convert.ToDouble(TargetY.Text) }));
				} catch(ArgumentOutOfRangeException ex){
					MessageBox.Show("Can't set target.\n"+ex.ToString());
				}
			} 
		}
	}
}
