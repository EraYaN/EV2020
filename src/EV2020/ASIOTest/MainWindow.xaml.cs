using BlueWave.Interop.Asio;
using EV2020.LocationSystem;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Providers.LinearAlgebra.Mkl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ASIOTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Localizer localizer;
		bool nodrivers, discovered, tested;
		PlotWindow pw;

		BackgroundWorker bw_generate = new BackgroundWorker();
		public MainWindow()
		{
			InitializeComponent();
			MathNet.Numerics.Control.LinearAlgebraProvider = new MklLinearAlgebraProvider();
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
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
				ASIODriverComboBox.SelectedIndex = 0;
			}
			bw_generate.DoWork += bw_generate_DoWork;
			bw_generate.WorkerReportsProgress = true;
			bw_generate.ProgressChanged += bw_generate_ProgressChanged;
			bw_generate.RunWorkerCompleted += bw_generate_RunWorkerCompleted;
			
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
			ASIOTest.IsEnabled = false;
			bw_generate.RunWorkerAsync(ASIODriverComboBox.SelectedIndex);			
		}
		[STAThread]
		void bw_generate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			localizer.InitializeDriver();
			localizer.OnLocationUpdated += localizer_OnLocationUpdated;
			ASIOShowControlPanel.IsEnabled = true;			
		}
		[STAThread]
		void bw_generate_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		void bw_generate_DoWork(object sender, DoWorkEventArgs e)
		{
			List<Microphone> mics = new List<Microphone>();
			//TODO verify
			mics.Add(new Microphone { Position = new Position3D { X = 0, Y = 0, Z = 0.36 }, ChannelIndex = 0 });
			mics.Add(new Microphone { Position = new Position3D { X = 0, Y = 7.4, Z = 0.36 }, ChannelIndex = 1 });
			mics.Add(new Microphone { Position = new Position3D { X = 7, Y = 7.4, Z = 0.36 }, ChannelIndex = 2 });
			mics.Add(new Microphone { Position = new Position3D { X = 7, Y = 0, Z = 0.36 }, ChannelIndex = 3 });
			mics.Add(new Microphone { Position = new Position3D { X = 3.5, Y = 0, Z = 0.8 }, ChannelIndex = 4 });
			localizer = new Localizer(mics,(int) e.Argument, new int[] { 0, 1, 2, 3, 4 }, new int[] { });
			localizer.GenerateFilterMatrix();			
		}
		void localizer_OnLocationUpdated(object sender, LocationUpdatedEventArgs e)
		{
			App.Current.Dispatcher.Invoke((Action)delegate
			{
				
				if (localizer != null)
				{
					if (localizer.lastData != null)
					{
						List<string> legend = new List<string>();
						for (int i = 0; i < localizer.lastData.ColumnCount; i++)
						{
							legend.Add(String.Format("Channel {0}",i+1));
						}
						if (pw != null)
						{
							if(pw.IsLoaded)
								pw.Update("Data", localizer.lastData, legend, ASIO.T);
						}
						else
						{
							pw = new PlotWindow("Data", localizer.lastData, legend, ASIO.T);
							pw.Show();
						}	
						
					}
				}
				//Debug.WriteLine(e.Position);
				posistionLog.AppendText(e.Position.ToString()+"\n");
				posistionLog.ScrollToEnd();
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
