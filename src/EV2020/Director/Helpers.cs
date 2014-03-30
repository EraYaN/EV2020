using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private static Action EmptyDelegate = delegate() { };

		public static void Refresh(this UIElement uiElement)
		{
			uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		}
		
	}
}
