using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The standard navigation class. (a dud)
	/// </summary>
	public class StandardNavigation : INavigator
	{
		public StandardNavigation()
		{
			//constructor			
		}

		public void Init() {  }
		public double Tick(double Distance, double Target) { throw new NotImplementedException(); }
		public string GetDebugInfo() { throw new NotImplementedException(); }
		public bool GoToPosition(double pos) { throw new NotImplementedException(); }
		public bool Pause() { throw new NotImplementedException(); }
		public bool Resume() { throw new NotImplementedException(); }		
	}
}
