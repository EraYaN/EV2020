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
using System.Threading;
using System.IO;
using EV2020.LocationSystem;
using OxyPlot;
using System.Globalization;
using BlueWave.Interop.Asio;

namespace ASIOTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ASIO audiosystem;
		bool nodrivers, discovered, tested, recording;
		float[] beaconsignal;
		float[] response;
		float[] response2;
		int a = 1;
		public List<DataPoint> GraphPoints
		{
			get
			{
				List<DataPoint> l = new List<DataPoint>();
				if (response == null)
					return l;

				float x = 0;
				foreach (float d in response)
				{
					l.Add(new DataPoint(x, d));
					x += (float)1/48000;
				}
				return l;
			}
		}
		public MainWindow()
		{
			InitializeComponent();
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			recording = false;
			nodrivers = false;
			discovered = false;
			tested = false;
			beaconsignal = Tools.refsignal(Tools.Timer0Freq.Carrier5kHz, Tools.Timer1Freq.Code1000Hz, Tools.Timer3Freq.Repeat10Hz, "92340f0f", 48000);
			//beaconsignal = new float[]{1,0};
			
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
			tested = true;
			audiosystem = new ASIO(ASIODriverComboBox.SelectedIndex, new int[] { 0, 1 }, new int[] { 1 }, 48000);
			ASIOShowControlPanel.IsEnabled = true;
			ASIOTest.IsEnabled = false;
			ASIORecord.IsEnabled = true;
			
		}
		[STAThread]
		private void ASIOShowControlPanel_Click(object sender, RoutedEventArgs e)
		{
			if(!tested){
				return;
			}
			audiosystem.ShowControlPanel();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (audiosystem != null)
			{
				audiosystem.Dispose();
				audiosystem = null;
			}
		}
		[STAThread]
		private void ASIORecord_Click(object sender, RoutedEventArgs e)
		{
			if (recording == true)
			{
				return;
			}
			else
			{
				List<double> times = new List<double>();
				Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
				for (int measurement = 0; measurement < 10; measurement++)
				{
					audiosystem.ClearOutput();
					audiosystem.ClearInput();
					audiosystem.putOutputSamples(beaconsignal);
					audiosystem.IsInputEnabled = true;
					audiosystem.IsOutputEnabled = true;
					Thread.Sleep(200);
					response = audiosystem.getAllInputSamples(0);
					response2 = audiosystem.getAllInputSamples(1);
					audiosystem.IsInputEnabled = false;
					audiosystem.IsOutputEnabled = false;
					float max = 0;
					int samplemax = 0;
					float max2 = 0;
					int samplemax2 = 0;
					for (int sample = 0; sample < response.Length; sample++)
					{
						if (response[sample] > max)
						{
							max = response[sample];
							samplemax = sample;
						}
					}
					for (int sample = 0; sample < response2.Length; sample++)
					{
						if (response2[sample] > max)
						{
							max2 = response2[sample];
							samplemax2 = sample;
						}
					}
					//System.Diagnostics.Debug.WriteLine("buffertime: {0} ms", audiosystem.getBufferTime());
					times.Add(Math.Abs((double)(samplemax - samplemax2)) / 48.0);
					Thread.Sleep(250);
				}
				LinePlot.ItemsSource = GraphPoints;
				LinePlotAxisX.MaximumPadding = 0;
				using (StreamWriter sr = new StreamWriter("data"+a.ToString()+".csv"))
				{
					for (int i = 0; i < times.Count; i++)
					{
						sr.WriteLine("{0:f8}", times[i]);
					}
					a++;
				}
				System.Diagnostics.Debug.WriteLine("Average: {0:f4}; min: {1:f4}, max: {2:f4}", times.Average(), times.Min(), times.Max());
				//recording = true;
			}
		}

		private void ASIOGetData_Click(object sender, RoutedEventArgs e)
		{
			/*using (StreamWriter sr = new StreamWriter("data.csv"))
			{
				for (int i = 0; i < Math.Min(RecordedSize, AudioSize); i++)
				{
					sr.WriteLine("{0}; {1}", audio[i], recorded[i]);
				}
			}*/
		}
	}
}
