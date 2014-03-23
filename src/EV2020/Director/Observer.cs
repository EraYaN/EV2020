using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public class Observer
	{
		IModel model;

		public String DebugInfo
		{
			get { return model.GetDebugInfo(); }
		}
		public Observer(IModel Model)
		{
			model = Model;
			model.Init();
		}
		public Observer()
		{
			model = new PDDefaultModel();
			model.Init();
		}
		public double Tick(double Distance, double Target)
		{
			double output = model.Tick(Distance, Target);
			Data.db.UpdateProperty("ModelDebugInfo");
			return output;
		}
	}
}
