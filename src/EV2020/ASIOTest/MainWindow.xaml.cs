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
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System.Diagnostics;
using MathNet.Numerics.Algorithms.LinearAlgebra.Mkl;

namespace ASIOTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Localizer localizer;
		bool nodrivers, discovered, tested, recording;
		PlotWindow pw;
		public MainWindow()
		{
			InitializeComponent();
			MathNet.Numerics.Control.LinearAlgebraProvider = new MklLinearAlgebraProvider();
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			recording = false;
			nodrivers = false;
			discovered = false;
			tested = false;
			//beaconsignal = Tools.refsignal(Tools.Timer0Freq.Carrier5kHz, Tools.Timer1Freq.Code1000Hz, Tools.Timer3Freq.Repeat10Hz, "92340f0f", 48000);
			//beaconsignal = new float[]{1,0};

			/*Vector<double> x0 = DenseVector.OfEnumerable(new double[]{1 , 2, 3, 4, 5});
			Matrix<double> X = Tools.Toep(x0, x0.Count*2-1, x0.Count,false);
			for (int row = 0; row < X.RowCount; row++)
			{
				StringBuilder line = new StringBuilder();
				for (int col = 0; col < X.ColumnCount; col++)
				{
					line.Append(X[row,col].ToString());
					line.Append(" ");
				}
				Debug.WriteLine(line.ToString());
			}*/	

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
			List<Microphone> mics = new List<Microphone>();
			mics.Add(new Microphone { Position = new Position3D { X = 0.02, Y = 0, Z = 0 }, ChannelIndex = 1 });
			mics.Add(new Microphone { Position = new Position3D { X = 0.35, Y = 0, Z = 0 }, ChannelIndex = 0 });
			localizer = new Localizer(mics,ASIODriverComboBox.SelectedIndex, new int[] { 0, 1 }, new int[] { 0 });
			localizer.OnLocationUpdated += localizer_OnLocationUpdated;
			ASIOShowControlPanel.IsEnabled = true;
			ASIOTest.IsEnabled = false;
		}
		void localizer_OnLocationUpdated(object sender, LocationUpdatedEventArgs e)
		{

			App.Current.Dispatcher.Invoke((Action)delegate
			{
				if (pw != null)
				{
					pw.Close();
				}
				if (localizer != null)
				{
					if (localizer.lastData != null)
					{
						pw = new PlotWindow("Data", localizer.lastData, new List<string>() { "Channel 1", "Channel 2", "Channel 1 Filtered", "Channel 2 Filtered" }, ASIO.T);
						pw.Show();
					}
				}
			});			
			
		}
		[STAThread]
		private void ASIOShowControlPanel_Click(object sender, RoutedEventArgs e)
		{
			if(!tested){
				return;
			}
			localizer.ShowASIOControlPanel();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			App.Current.Dispatcher.Invoke((Action)delegate
			{
				if (pw != null)
				{
					pw.Close();
				}
			});
			if (localizer != null)
			{
				localizer.Dispose();
				localizer = null;
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
