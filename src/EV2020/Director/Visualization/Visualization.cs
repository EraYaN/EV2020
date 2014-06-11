using EV2020.LocationSystem;
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
using System.Windows.Threading;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace EV2020.Director
{
	public class Visualization
	{
		Canvas c, jc;
		//general
		const double canvasPadding = 1;
		//joystick
		const double joystickStrokeThickness = 2;
		const double joystickDiameter = 10;
		const double directionArrowThickness = 3;
		readonly Brush joystickStroke = Brushes.Black;
		readonly Brush joystickFill = Brushes.DarkGray;
		//field
		const double fieldStrokeThickness = 1;
		readonly Brush fieldBorder = Brushes.Black;
		//margin
		const double marginStrokeThickness = 1;
		readonly Brush marginBorder = Brushes.Green;
		//micMarkers
		const double micMarkerSize = 0.1; //in meters
		readonly Brush micMarkerFill = Brushes.Black;
		//target
		const double targetSize = 0.3; //in meters
		readonly Brush targetFill = Brushes.Red;
		//car
		const double carStrokeThickness = 3;
		const double carLength = 0.52; //in meters
		const double carWidth = 0.42; //in meters
		readonly Brush carBorder = Brushes.DarkBlue;

		
		/// <summary>
		/// Initializes the Visualization class
		/// </summary>
		/// <param name="_c">The field canvas</param>
		/// <param name="_jc">THe joystick canvas</param>
		public Visualization(Canvas _c, Canvas _jc)
		{
			//Constructor
			c = _c;
			jc = _jc;

			// TEST!! convolution speed
			/*Vector<double> matchedfilter = new DenseVector(500);
			for (int i = 0; i < matchedfilter.Count; i++)
				matchedfilter[i] = 2341.13341;
			//Matrix<double> matrix = new DenseMatrix(5, 10000);
			Matrix<double> responses = new DenseMatrix(12000, 5);
			double rnd = 0.2354332;
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 12000; j++)
				{
					rnd = (rnd * 34123.32412312 + 0.32412255234) % 1;
					responses[j, i] = rnd;
				}
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 500; j++)
				{
					rnd = (rnd * 34123.32412312 + 0.32412255234) % 1;
					responses[j + 1000, i] = matchedfilter[j] - rnd/2;
				}
			}

			//Matrix<double> result = new DenseMatrix(5, 30000);
			long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

			double[,] responsesArray = new double[12000, 5];

			for (int i = 0; i < 5; i++)
				for (int j = 0; j < 12000; j++)
					responsesArray[j, i] = responses[j, i];

			int[] samplemaxes = new int[5];
			double[] maxes = new double[5];
			Matrix<double> filteredResponses = new DenseMatrix(responses.RowCount, responses.ColumnCount);
			for (int i = 0; i < 5; i++)
			{
				double maxval = 0;
				int start, end;
				if (i == 0)
				{
					start = 0;
					end = responsesArray.GetLength(0) - matchedfilter.Count;
				}
				else
				{
					start = Math.Max(samplemaxes[0] - 500, 0);
					end = Math.Min(samplemaxes[0] + 500, responsesArray.GetLength(0) - matchedfilter.Count);
				}
				for (int j = start; j < end; j++)
				{
					//double val = 0;
					/*for (int k = 0; k < matchedfilter.Count; k++)
						filteredResponses[j, i] += responsesArray[j + k, i] * matchedfilter[k];
					if (filteredResponses[j, i] > maxval)
					{
						maxval = filteredResponses[j, i];
						maxes[i] = Math.Abs(filteredResponses[j, i]);
						samplemaxes[i] = j;
					}*/

					/*double val = 0;
					for (int k = 0; k < matchedfilter.Count; k++)
						val += responsesArray[j + k, i] * matchedfilter[k];
					if (val > maxval)
					{
						maxval = val;
						maxes[i] = Math.Abs(val);
						samplemaxes[i] = j;
					}
				}
			}

			long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - milliseconds;
			time = time;*/
		}	
		/// <summary>
		/// This updates the joystick position
		/// </summary>
		public void drawJoystick(){
			//Check for access to the UI thread.
			if (jc.Dispatcher.CheckAccess())
			{
				double centerH = jc.ActualHeight / 2;
				double centerW = jc.ActualWidth / 2;
				if (Data.ctr == null)
				{
					return;
				}
				double Y = -Data.ctr.Driving * centerH / 15 + centerH - joystickDiameter / 2;
				double X = -Data.ctr.Steering * centerW / 50 + centerW - joystickDiameter / 2;
				jc.Children[0].SetValue(Canvas.LeftProperty, X);
				jc.Children[0].SetValue(Canvas.TopProperty, Y);
			}
			else
			{
				try
				{
					jc.Dispatcher.Invoke(drawJoystick, DispatcherPriority.Normal);
				}
				catch
				{

				}
			}
		}
		/// <summary>
		/// This mehtod draws the Field on the Canvas in the UI
		/// </summary>
		public void drawField()
		{
			if (c.Dispatcher.CheckAccess())
			{
				if (Data.cfg == null)
					return;
				//draw field
				c.Children.Clear();
				double cH = Math.Max(c.ActualHeight-2*canvasPadding,0);
				double cW = Math.Max(c.ActualWidth-2*canvasPadding,0);
				double scalingfactor = Math.Min(cH / (Data.cfg.FieldHeight + 2 * Data.cfg.FieldMargin),cW / (Data.cfg.FieldWidth + 2 * Data.cfg.FieldMargin));
				Rectangle field = new Rectangle();
				field.Width = Data.cfg.FieldWidth * scalingfactor;
				field.Height = Data.cfg.FieldHeight * scalingfactor;
				field.Stroke = fieldBorder;
				field.StrokeThickness = fieldStrokeThickness;
				c.Children.Add(field);
				field.SetValue(Canvas.LeftProperty, Data.cfg.FieldMargin*scalingfactor+canvasPadding);
				field.SetValue(Canvas.TopProperty, Data.cfg.FieldMargin * scalingfactor+canvasPadding);

				Rectangle margins = new Rectangle();
				margins.Width = (Data.cfg.FieldWidth + 2 * Data.cfg.FieldMargin) * scalingfactor;
				margins.Height = (Data.cfg.FieldHeight + 2 * Data.cfg.FieldMargin) * scalingfactor;
				margins.Stroke = marginBorder;
				margins.StrokeThickness = marginStrokeThickness;
				c.Children.Add(margins);
				margins.SetValue(Canvas.LeftProperty, canvasPadding);
				margins.SetValue(Canvas.TopProperty, canvasPadding);

				foreach (Microphone mic in Data.cfg.Microphones)
				{
					Ellipse micMarker = new Ellipse();
					micMarker.Width = micMarkerSize * scalingfactor;
					micMarker.Height = micMarkerSize * scalingfactor;
					micMarker.Fill = micMarkerFill;
					c.Children.Add(micMarker);
					micMarker.SetValue(Canvas.LeftProperty, (mic.X - micMarkerSize / 2 + Data.cfg.FieldMargin) * scalingfactor + canvasPadding);
					micMarker.SetValue(Canvas.TopProperty, (mic.Y - micMarkerSize / 2 + Data.cfg.FieldMargin) * scalingfactor + canvasPadding);
				}

				

				if (Data.nav == null)
					return;
				
				Ellipse target = new Ellipse();
				target.Width = targetSize * scalingfactor;
				target.Height = targetSize * scalingfactor;
				target.Fill = targetFill;
				c.Children.Add(target);
				target.SetValue(Canvas.LeftProperty, (Data.nav.TargetX - targetSize / 2 + Data.cfg.FieldMargin) * scalingfactor + canvasPadding);
				target.SetValue(Canvas.TopProperty, (Data.nav.TargetY - targetSize / 2 + Data.cfg.FieldMargin) * scalingfactor + canvasPadding);

				
				Car car = new Car();
				car.Length = carLength * scalingfactor;
				car.Width = carWidth * scalingfactor;
				car.Stroke = carBorder;
				car.StrokeThickness = 2;
				if (Data.ctr != null)
				{
					double stub = 0.01 * scalingfactor;
					car.LeftSensor = Math.Max(Data.ctr.CurrentLeftDistance / 100 * scalingfactor, stub);
					car.RightSensor = Math.Max(Data.ctr.CurrentRightDistance / 100 * scalingfactor, stub);
					car.BeamFOV = 30 * Math.PI / 180;
				}
				else
				{
					car.LeftSensor = 0.1*scalingfactor;
					car.RightSensor = 0.1*scalingfactor;
					car.BeamFOV = 0 * Math.PI / 180;
				}				
				car.Bearing = Data.nav.CarBearing;
				car.X = (Data.nav.CarX + Data.cfg.FieldMargin) * scalingfactor + canvasPadding;
				car.Y = (Data.nav.CarY + Data.cfg.FieldMargin) * scalingfactor + canvasPadding;
				c.Children.Add(car);


				//Ellipse rotCenter = new Ellipse();
				//rotCenter.Width = 10;
				//rotCenter.Height = 10;
				//rotCenter.Fill = Brushes.Orange;
				//c.Children.Add(rotCenter);
				//rotCenter.SetValue(Canvas.LeftProperty, rot.CenterX + car.X - car.Length / 2 - 5+canvasPadding);
				//rotCenter.SetValue(Canvas.TopProperty, rot.CenterY + car.Y - car.Width / 2 - 5 + canvasPadding);
				
			}
			else
			{
				try
				{
					c.Dispatcher.Invoke(drawField, DispatcherPriority.Normal);
				}
				catch
				{

				}
			}
		}
	}
}
