using EV2020.Communication;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using MLE4WCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
    public static class Data
    {
        public static string ComPort = "COM1";
        public static int BaudRate = 115200;
        static public SerialInterface com;
        static public Visualization vis;
        static public Navigation nav;
        static public Controller ctr;
        public static Databindings db = new Databindings();
        public static MATLABWrapper matlab;
		public static Observer obsvr;
        // State matrix [-1.540497550593680 0.499996085982763;-0.050981466007821 -1.221182449406320]
		public static Matrix<double> A = DenseMatrix.OfArray(new double[,] 
        { 
            //{ -1.540497550593680, 0.499996085982763 },
            //{ -0.050981466007821, -1.221182449406320 }
		   { -2.68, 1.93 },
           { 1.78, -3.5 }
        });
        // Input-to-state matrix
		public static Matrix<double> B = DenseMatrix.OfArray(new double[,] 
        {
            { -24.048787384198400 },
            { 38.614088839675570 } 
        });
        // State-to-output matrix
		public static Matrix<double> C = DenseMatrix.OfArray(new double[,] 
        { 
            { 1, 0 } 
        });
        // Feedthrough matrix
		public static Matrix<double> D = DenseMatrix.OfArray(new double[,] 
        { 
            { 0 }
        });
        // Controller Gain 
		public static Matrix<double> K = DenseMatrix.OfArray(new double[,] 
        { 
            //{ -0.064057182010178, -0.020791231841726 }
			{ -0.11, -0.04 }
        });
		// Observer Gain
		public static Matrix<double> L = DenseMatrix.OfArray(new double[,] 
        { 
            { 5 }, 
            { 10 } 
        });
        // Input scale gain
		public static double Nbar = -0.128112399834024;
    }
}
