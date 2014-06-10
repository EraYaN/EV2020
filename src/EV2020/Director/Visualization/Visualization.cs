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
		
		public Visualization(Canvas _c, Canvas _jc)
		{
			//Constructor
			c = _c;
			jc = _jc;
		}	

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

		public void drawField()
		{
			if (c.Dispatcher.CheckAccess())
			{
				if (Data.cfg == null)
					return;
				//draw field
				c.Children.Clear();
				double cH = c.ActualHeight-2*canvasPadding;
				double cW = c.ActualWidth-2*canvasPadding;
				double scalingfactor = Math.Min(cH / (Data.cfg.FieldHeight + 2 * Data.cfg.FieldMargin), cW / (Data.cfg.FieldWidth + 2 * Data.cfg.FieldMargin));
				Rectangle field = new Rectangle();
				field.Width = Data.cfg.FieldWidth * scalingfactor;
				field.Height = Data.cfg.FieldHeight * scalingfactor;
				field.Stroke = fieldBorder;
				field.StrokeThickness = fieldStrokeThickness;
				field.SetValue(Canvas.LeftProperty, 0);
				field.SetValue(Canvas.TopProperty, 0);
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
