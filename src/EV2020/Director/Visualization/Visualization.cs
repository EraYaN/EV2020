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

namespace EV2020.Director
{
	public class Visualization
	{
		Canvas c, jc;
		//general
		const double canvasPadding = 10;
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
		//car
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
				double scalingfactory = cH / (Data.cfg.FieldHeight + 2 * Data.cfg.FieldMargin);
				double scalingfactorx = cW / (Data.cfg.FieldWidth + 2 * Data.cfg.FieldMargin);
				Rectangle field = new Rectangle();
				field.Width = Data.cfg.FieldWidth * scalingfactorx;
				field.Height = Data.cfg.FieldHeight * scalingfactory;
				field.Stroke = fieldBorder;
				field.StrokeThickness = fieldStrokeThickness;
				c.Children.Add(field);
				field.SetValue(Canvas.LeftProperty, Data.cfg.FieldMargin*scalingfactorx+canvasPadding);
				field.SetValue(Canvas.TopProperty, Data.cfg.FieldMargin * scalingfactory+canvasPadding);

				Rectangle margins = new Rectangle();
				margins.Width = (Data.cfg.FieldWidth + 2 * Data.cfg.FieldMargin) * scalingfactorx;
				margins.Height = (Data.cfg.FieldHeight + 2 * Data.cfg.FieldMargin) * scalingfactory;
				margins.Stroke = marginBorder;
				margins.StrokeThickness = marginStrokeThickness;
				c.Children.Add(margins);
				margins.SetValue(Canvas.LeftProperty, canvasPadding);
				margins.SetValue(Canvas.TopProperty, canvasPadding);
			}
			else
			{
				try
				{
					c.Dispatcher.Invoke(drawJoystick, DispatcherPriority.Normal);
				}
				catch
				{

				}
			}
		}
	}
}
