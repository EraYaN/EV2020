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
		/// <summary>
		/// This methods makes sure the value lies between the min and max parameters
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="val">The value</param>
		/// <param name="min">The minimum value</param>
		/// <param name="max">The maximum value</param>
		/// <returns>The clamped value</returns>
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}
		/// <summary>
		/// The method applies the Clamp method on alle element of the matrix
		/// </summary>
		/// <param name="val">The value</param>
		/// <param name="min">The minimum value</param>
		/// <param name="max">The maximum value</param>
		/// <returns>The matrix with all values clamped</returns>
		public static Matrix<double> Clamp(this Matrix<double> val, double min, double max)
		{
			for (int i = 0; i < val.RowCount; i++)
			{
				for (int j = 0; j < val.ColumnCount; j++)
				{
					val[i, j] = val[i, j].Clamp(min,max);					
				}
			}			
			return val;
		}
		/// <summary>
		/// The method applies the Clamp method on alle element of the vector
		/// </summary>
		/// <param name="val">The value</param>
		/// <param name="min">The minimum value</param>
		/// <param name="max">The maximum value</param>
		/// <returns>The vector with all values clamped</returns>
		public static Vector<double> Clamp(this Vector<double> val, double min, double max)
		{
			for (int i = 0; i < val.Count; i++)
			{
				val[i] = val[i].Clamp(min, max);
			}
			return val;
		}
		/// <summary>
		/// This methods make the value when between upperBound and lowerBound
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="val">The value</param>
		/// <param name="lowerBound">The lower limit of the deadzone</param>
		/// <param name="upperBound">The upper limit of the deadzone</param>
		/// <returns>The value with the deadzone applied</returns>
		public static T Deadzone<T>(this T val, T lowerBound, T upperBound) where T : IComparable<T>
		{
			dynamic r = 0;
			if (val.CompareTo(upperBound) < 0 && val.CompareTo(lowerBound) > 0) return r;
			else return val;
		}
		/// <summary>
		/// The method applies the Deadzone method on alle element of the matrix
		/// </summary>
		/// <param name="val">The value</param>
		/// <param name="lowerBound">The lower limit of the deadzone</param>
		/// <param name="upperBound">The upper limit of the deadzone</param>
		/// <returns>The matrix with all values deadzoned</returns>
		public static Matrix<double> Deadzone(this Matrix<double> val, double lowerBound, double upperBound)
		{
			for (int i = 0; i < val.RowCount; i++)
			{
				for (int j = 0; j < val.ColumnCount; j++)
				{
					val[i, j] = val[i, j].Deadzone(lowerBound, upperBound);
				}
			}
			return val;
		}
		/// <summary>
		/// The method applies the Deadzone method on alle element of the vector
		/// </summary>
		/// <param name="val">The value</param>
		/// <param name="lowerBound">The lower limit of the deadzone</param>
		/// <param name="upperBound">The upper limit of the deadzone</param>
		/// <returns>The vector with all values deadzoned</returns>
		public static Vector<double> Deadzone(this Vector<double> val, double lowerBound, double upperBound)
		{
			for (int i = 0; i < val.Count; i++)
			{
				val[i] = val[i].Deadzone(lowerBound, upperBound);				
			}
			return val;
		}
		/// <summary>
		/// Finds the InstalledDriver from the string name
		/// </summary>
		/// <param name="haystack">Array with installed drivers</param>
		/// <param name="needle">The string name of the driver</param>
		/// <returns>The matched InstalledDriver instance or null</returns>
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
		/// <summary>
		/// Calculates the angle of a 2D vector
		/// </summary>
		/// <param name="val">The 2D vector</param>
		/// <returns>The angle in radians</returns>
		public static double Angle(this Vector<double> val)
		{
			if (val.Count != 2)
			{
				throw new ArgumentException("Vector needs to be 2D.");
			}
			if (val[0] != 0)
			{
				if (val[0] >= 0)
				{
					return Math.Atan(val[1] / val[0]);
				}	
				else if(val[0]<0&&val[1]<0){
					return Math.Atan(val[1] / val[0])-Math.PI;
				}
				else
				{
					return Math.Atan(val[1] / val[0]) + Math.PI;
				}	
			}
			else
			{
				//Tan of infinity
				if (val[1] >= 0)
				{
					return 0.5 * Math.PI;
				}
				else
				{
					return -0.5 * Math.PI;
				}
			}
		}		
		/// <summary>
		/// The magnitude of a 2D vector
		/// </summary>
		/// <param name="val">The 2D vector</param>
		/// <returns>The magnitude of the vector</returns>
		public static double Magnitude(this Vector<double> val)
		{
			if (val.Count != 2)
			{
				throw new ArgumentException("Vector needs to be 2D.");
			}			
			return Math.Sqrt(val[0]*val[0] + val[1]*val[1]);			
		}
		private static Action EmptyDelegate = delegate() { };
		/// <summary>
		/// Refreshes/Redraws a UIElement
		/// </summary>
		/// <param name="uiElement">The element to be refreshed/redrawed</param>
		public static void Refresh(this UIElement uiElement)
		{
			uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		}
		
	}
}
