using BlueWave.Interop.Asio;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Windows;
using System.Windows.Threading;

namespace EV2020.Director
{
	/// <summary>
	/// A helper class the contains all extension methods.
	/// </summary>
	public static class Helpers
	{
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}
		public static Matrix<double> Clamp(this Matrix<double> val, double min, double max)
		{
			for (int i = 0; i < val.ColumnCount; i++)
			{
				for (int j = 0; j < val.RowCount; j++)
				{
					val[i, j] = val[i, j].Clamp(min,max);					
				}
			}			
			return val;
		}
		public static Vector<double> Clamp(this Vector<double> val, double min, double max)
		{
			for (int i = 0; i < val.Count; i++)
			{
				val[i] = val[i].Clamp(min, max);
			}
			return val;
		}

		public static T Deadzone<T>(this T val, T lowerBound, T upperBound) where T : IComparable<T>
		{
			dynamic r = 0;
			if (val.CompareTo(upperBound) < 0 && val.CompareTo(lowerBound) > 0) return r;
			else return val;
		}
		public static Matrix<double> Deadzone(this Matrix<double> val, double lowerBound, double upperBound)
		{
			for (int i = 0; i < val.ColumnCount; i++)
			{
				for (int j = 0; j < val.RowCount; j++)
				{
					val[i, j] = val[i, j].Deadzone(lowerBound, upperBound);
				}
			}
			return val;
		}
		public static Vector<double> Deadzone(this Vector<double> val, double lowerBound, double upperBound)
		{
			for (int i = 0; i < val.Count; i++)
			{
				val[i] = val[i].Deadzone(lowerBound, upperBound);				
			}
			return val;
		}
		public static InstalledDriver Find(this InstalledDriver[] haystack, String needle)
		{
			for (int i = 0; i < haystack.Length; i++)
			{
				if (haystack[i].Name == needle)
				{
					return haystack[i];
				}
			}
			return null;
		}
		private static Action EmptyDelegate = delegate() { };

		public static void Refresh(this UIElement uiElement)
		{
			uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		}
		
	}
}
