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
        public static string ComPort = "COM10";
        public static int BaudRate = 115200;
        static public SerialInterface com;
        static public Visualization vis;
		static public INavigator nav;
        static public Controller ctr;
        public static Databindings db = new Databindings();
        public static MATLABWrapper matlab;
		public static Observer obsvr;		
    }
}
