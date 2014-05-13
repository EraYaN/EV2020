using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlueWave.Interop.Asio;
using System.Threading;
using System.IO;
using EV2020.AudioSystem;

namespace ASIOTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ASIO audiosystem;
		bool nodrivers, discovered, tested;
		public MainWindow()
		{
			InitializeComponent();
			audio = new float[AudioSize];
			recorded = new float[RecordedSize];
			for (int i = 0; i < AudioSize; i++)
			{
				if (i > AudioSize / 2)
				{
					audio[i] = 0;
				}
				else
				{
					audio[i] = (float)Math.Sin(Math.PI * 800 * i / Fs);
				}
			}
		}
		[STAThread]
		private void ASIODiscover_Click(object sender, RoutedEventArgs e)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			if (AsioDriver.InstalledDrivers.Length == 0)
			{
				ComboBoxItem coi = new ComboBoxItem();
				coi.Content = "No drivers on system";
				ASIODriverComboBox.Items.Add(coi);
				nodrivers = true;
			}
			else
			{
				for (int index = 0; index < AsioDriver.InstalledDrivers.Length; index++)
				{
					ComboBoxItem coi = new ComboBoxItem();
					coi.Content = string.Format("  {0}. {1}", index + 1, AsioDriver.InstalledDrivers[index]);
					ASIODriverComboBox.Items.Add(coi);
				}
				discovered = true;
			}
			ASIODiscover.IsEnabled = false;
			ASIOTest.IsEnabled = true;
		}
		[STAThread]
		private void ASIOTest_Click(object sender, RoutedEventArgs e)
		{
			if (nodrivers)
			{
				return;
			}
			if (!discovered)
			{
				return;
			}
			audiosystem = new ASIO(ASIODriverComboBox.SelectedIndex);
			
		}
		[STAThread]
		private void ASIOShowControlPanel_Click(object sender, RoutedEventArgs e)
		{
			if(!tested){
				return;
			}
			driver.ShowControlPanel();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			driver.Stop();
			//driver.DisposeBuffers();			
			driver.Release();
		}
		

		private void ASIOStart_Click(object sender, RoutedEventArgs e)
		{
			if (!tested)
			{
				return;
			}
			audiosystem.Start();
			ASIOStart.IsEnabled = false;
		}

		private void ASIORecord_Click(object sender, RoutedEventArgs e)
		{
			if (recording == true)
			{
				return;
			}
			else
			{
				recPos = 0;
				recording = true;
			}
		}

		private void ASIOGetData_Click(object sender, RoutedEventArgs e)
		{
			using (StreamWriter sr = new StreamWriter("data.csv"))
			{
				for (int i = 0; i < Math.Min(RecordedSize, AudioSize); i++)
				{
					sr.WriteLine("{0}; {1}", audio[i], recorded[i]);
				}
			}
		}
	}
}
