using EV2020.LocationSystem;
using MathNet.Numerics.LinearAlgebra;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ASIOTest
{
	/// <summary>
	/// Interaction logic for PlotWindow.xaml
	/// </summary>
	public partial class PlotWindow : Window
	{
		//List<LinePlotData> lineplots = new List<LinePlotData>();
		Presenter p;
		public PlotWindow(String title, Matrix<double> plotdata, List<string> legend, double timestep)
		{
			InitializeComponent();
			p = (Presenter)this.DataContext;
			p.InitializePlot(title);
			if (plotdata.ColumnCount > 20)
			{
				throw new ArgumentException("Plotdata can contain at most 20 columns.","plotdata");
			}
			for (int line = 0; line < plotdata.ColumnCount; line++)
			{
				LineSeries ls = new LineSeries();
				List<DataPoint> list = new List<DataPoint>();
				for (int i = 0; i < plotdata.RowCount; i++)
				{					
					list.Add(new DataPoint(timestep * i, plotdata[i,line]));
				}
				ls.ItemsSource = list;
				ls.Smooth = false;
				ls.StrokeThickness = 0.5;
				ls.Title = legend[line];
				p.PlotModel.Series.Add(ls);
			}
			p.PlotModel.IsLegendVisible = true;
		}
	}
	public class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChangedEvent(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class Presenter : ObservableObject
	{
		private PlotModel _plotModel;
		
		public PlotModel PlotModel{
			get { return _plotModel; }
			set
			{
				_plotModel = value;
				RaisePropertyChangedEvent("PlotModel");
			}
		}

		public void InitializePlot(string title = null, string subtitle = null)
		{
			_plotModel = new PlotModel(title, subtitle);
			RaisePropertyChangedEvent("PlotModel");

		}

		
	}



}
