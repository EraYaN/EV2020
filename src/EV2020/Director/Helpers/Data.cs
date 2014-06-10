using EV2020.Communication;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MLE4WCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	/// <summary>
	/// The static Data class.
	/// </summary>
    public static class Data
    {
		/// <summary>
		/// Configuration unit
		/// </summary>
		static public Settings cfg;
		/// <summary>
		/// Serial communication interface
		/// </summary>
        static public SerialInterface com;
		/// <summary>
		/// GUI visualization
		/// </summary>
        static public Visualization vis;
		/// <summary>
		/// Apple Maps for revolutionary car navigation
		/// </summary>
		static public INavigator nav;
		/// <summary>
		/// Controller class
		/// </summary>
        static public Controller ctr;
		/// <summary>
		/// Misc data bindings structure used for visualization
		/// </summary>
        public static Databindings db = new Databindings();
		/// <summary>
		/// Matlab wrapper (not used)
		/// </summary>
        public static MATLABWrapper matlab;
		//public static Observer obsvr;		
    }
}
