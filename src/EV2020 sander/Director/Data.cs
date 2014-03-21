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

		public static Matrix<double> A = DenseMatrix.OfArray(new double[,]
									{
										{ 0f, 1f },
										{ 0f, -1.782000000000000f }
									});
		public static Matrix<double> B = DenseMatrix.OfArray(new double[,]
									{
										{ 5.639000000000000f },
										{ -39.940000000000000f }
									});
		public static Matrix<double> C = DenseMatrix.OfArray(new double[,]
									{
										{ 1f, 0f }
									});
		public static Matrix<double> L = DenseMatrix.OfArray(new double[,]
									{
										{ 1.218000000000000f },
										{ -0.170476000000000f }
									});
		public static Matrix<double> K = DenseMatrix.OfArray(new double[,]
									{
										{ 0f, -0.8f }
									});
		
	}
}
