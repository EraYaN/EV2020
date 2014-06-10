using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace EV2020.Director
{
	/// <summary>
	/// A shape class that defines an Arrow, for use on a Canvas.
	/// Source: http://www.codeproject.com/articles/23116/wpf-arrow-and-custom-shapes
	/// </summary>
	public sealed class Car : Shape
	{
		#region Dependency Properties

		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty LengthProperty = DependencyProperty.Register("Length", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty LeftSensorProperty = DependencyProperty.Register("LeftSensor", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty RightSensorProperty = DependencyProperty.Register("RightSensor", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty BearingProperty = DependencyProperty.Register("Bearing", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty BeamFOVProperty = DependencyProperty.Register("BeamFOV", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
		
		#endregion

		#region CLR Properties

		[TypeConverter(typeof(LengthConverter))]
		public double X
		{
			get { return (double)base.GetValue(XProperty); }
			set { base.SetValue(XProperty, value); _changed = true; }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Y
		{
			get { return (double)base.GetValue(YProperty); }
			set { base.SetValue(YProperty, value); _changed = true; }
		}
		[TypeConverter(typeof(LengthConverter))]
		public double Width
		{
			get { return (double)base.GetValue(WidthProperty); }
			set { base.SetValue(WidthProperty, value); _changed = true; }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Length
		{
			get { return (double)base.GetValue(LengthProperty); }
			set { base.SetValue(LengthProperty, value); _changed = true; }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double LeftSensor
		{
			get { return (double)base.GetValue(LeftSensorProperty); }
			set { base.SetValue(LeftSensorProperty, value); _changed = true; }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double RightSensor
		{
			get { return (double)base.GetValue(RightSensorProperty); }
			set { base.SetValue(RightSensorProperty, value); _changed = true; }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Bearing
		{
			get { return (double)base.GetValue(BearingProperty); }
			set { base.SetValue(BearingProperty, value); _changed = true; }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double BeamFOV
		{
			get { return (double)base.GetValue(BeamFOVProperty); }
			set { base.SetValue(BeamFOVProperty, value); _changed = true; }
		}		

		#endregion
		private bool _changed = false;
		private StreamGeometry _geom;

		#region Overrides

		protected override Geometry DefiningGeometry
		{
			get
			{
				if(_changed || _geom ==null){				// Create a StreamGeometry for describing the shape
					_geom = new StreamGeometry();
					_geom.FillRule = FillRule.EvenOdd;
					using (StreamGeometryContext context = _geom.Open())
					{
						GenerateGeometry(context);
					}
					// Freeze the geometry for performance benefits
					_geom.Freeze();
					RotateTransform rot = new RotateTransform(Bearing * 180 / Math.PI);
					rot.CenterX = X;
					rot.CenterY = Y;
					this.RenderTransform = rot;
					_changed = false;
				}
				return _geom;
			}
		}

		#endregion

		#region Privates

		private void GenerateGeometry(StreamGeometryContext context)
		{			
			double sinbeam = Math.Sin(BeamFOV/2);
			double cosbeam = Math.Cos(BeamFOV/2);
			//distance from center
			double sensorpartofwidth = 0.75;
			
			//right side
			context.BeginFigure(new Point(X - Length / 2, Y - Width / 2), true, true);
			context.LineTo(new Point(X + Length / 2, Y - Width / 2), true, true);

			//right sensor connection point
			context.LineTo(new Point(X + Length / 2, Y - Width * sensorpartofwidth * 0.5), true, true);
			//right sensor
			context.LineTo(new Point(X + Length / 2 + RightSensor * cosbeam, Y - Width * sensorpartofwidth * 0.5 - RightSensor * sinbeam), true, true);
			context.LineTo(new Point(X + Length / 2 + RightSensor * cosbeam, Y - Width * sensorpartofwidth * 0.5 + RightSensor * sinbeam), true, true);
			//right sensor connection point
			context.LineTo(new Point(X + Length / 2, Y - Width * sensorpartofwidth * 0.5), true, true);
			//left sensor connection point
			context.LineTo(new Point(X + Length / 2, Y + Width * sensorpartofwidth * 0.5), true, true);
			//left sensor
			context.LineTo(new Point(X + Length / 2 + LeftSensor * cosbeam, Y + Width * sensorpartofwidth * 0.5 + LeftSensor * sinbeam), true, true);
			context.LineTo(new Point(X + Length / 2 + LeftSensor * cosbeam, Y + Width * sensorpartofwidth * 0.5 - LeftSensor * sinbeam), true, true);
			//left sensor connection point
			context.LineTo(new Point(X + Length / 2, Y + Width * sensorpartofwidth * 0.5), true, true);

			//left side
			context.LineTo(new Point(X + Length / 2, Y + Width / 2), true, true);
			context.LineTo(new Point(X - Length / 2, Y + Width / 2), true, true);
					
		}

		#endregion
	}
}
