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
		const double joystickStrokeThickness = 2;
		const double joystickDiameter = 10;
		const double directionArrowThickness = 3;
		readonly Brush joystickStroke = Brushes.Black;
		readonly Brush joystickFill = Brushes.DarkGray;

		
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

				double Y = -Data.ctr.Driving * centerH / 15 + centerH - joystickDiameter / 2;
				double X = -Data.ctr.Steering * centerW / 50 + centerW - joystickDiameter / 2;
				jc.Children[0].SetValue(Canvas.LeftProperty, X);
				jc.Children[0].SetValue(Canvas.TopProperty, Y);
			}
			else
			{
				try
				{
					c.Dispatcher.Invoke(drawJoystick, DispatcherPriority.Normal);
				}
				catch (Exception)
				{

				}
			}
		}
	}
}
