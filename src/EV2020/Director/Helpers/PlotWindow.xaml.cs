using EV2020.LocationSystem;
using MathNet.Numerics.LinearAlgebra;
using MicroMvvm;
using OxyPlot;
using OxyPlot.Axes;
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

namespace EV2020.Director
{
	/// <summary>
	/// Interaction logic for PlotWindow.xaml
	/// </summary>
	public partial class PlotWindow : Window
	{
		//List<LinePlotData> lineplots = new List<LinePlotData>();
		Presenter p;
		public PlotWindow()
		{
			InitializeComponent();
		}
		public PlotWindow(String title, Matrix<double> plotdata, List<string> legend, double timestep, int[] markers)
		{
			InitializeComponent();
			p = (Presenter)this.DataContext;
			p.Update(title, plotdata, legend, timestep, markers);
		}
		public void Update(String title, Matrix<double> plotdata, List<string> legend, double timestep, int[] markers)
		{			
			p = (Presenter)this.DataContext;
			p.Update(title, plotdata, legend, timestep, markers);
		}
	}	
	public class Presenter : ObservableObject
	{
		private PlotModel _plotModel;
		
		public PlotModel PlotModel{
			get { return _plotModel; }			
		}

		public void InitializePlot(string title = null, string subtitle = null)
		{
			_plotModel = new PlotModel(title, subtitle);
			RaisePropertyChanged("PlotModel");
		}

		public void Update(String title, Matrix<double> plotdata, List<string> legend, double timestep, int[] markers)
		{
			if (_plotModel == null)
			{
				InitializePlot(title);
			}
			if (plotdata.ColumnCount > 20)
			{
				throw new ArgumentException("Plotdata can contain at most 20 columns.", "plotdata");
			}
			_plotModel.Series.Clear();
			_plotModel.Axes.Clear();
			double maxx = plotdata.RowCount * timestep;
			double maxy = 15;
			Axis x = new LinearAxis(AxisPosition.Bottom, 0, maxx);
			//Axis y =new LinearAxis(AxisPosition.Left, 0, 15);
			_plotModel.Axes.Add(x);
			//_plotModel.Axes.Add(y);
			for (int line = 0; line < plotdata.ColumnCount; line++)
			{
				LineSeries ls = new LineSeries();
				List<DataPoint> list = new List<DataPoint>();
				for (int i = 0; i < plotdata.RowCount; i++)
				{
					list.Add(new DataPoint(timestep * i, Math.Abs(plotdata[i, line])));
				}
				ls.ItemsSource = list;
				ls.Smooth = false;
				ls.StrokeThickness = 0.5;
				ls.Title = legend[line];				
				_plotModel.Series.Add(ls);
				
			}
			int nummarkers=  Math.Min(markers.Length, plotdata.ColumnCount);
			LineSeries markerseries = new LineSeries();
			List<DataPoint> mark = new List<DataPoint>();
			for (int i = 0; i < nummarkers; i++)
			{
				mark.Add(new DataPoint(timestep * markers[i], Math.Abs(plotdata[markers[i], i])));	
			}
			markerseries.ItemsSource = mark;
			markerseries.LineStyle = LineStyle.None;
			markerseries.MarkerType = MarkerType.Circle;
			markerseries.MarkerSize = 3;
			markerseries.MarkerFill = OxyColors.Black;
			markerseries.Title = "Detected Peaks";
			_plotModel.Series.Add(markerseries);

			_plotModel.IsLegendVisible = true;
			_plotModel.RefreshPlot(true);
			RaisePropertyChanged("PlotModel");
		}

		
	}



}
