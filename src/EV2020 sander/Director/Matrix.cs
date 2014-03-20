using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class Matrix
	{
		public static float[,] A = {
										{ 0f, 1f },
										{ 0f, -1.782000000000000f }
								   };
		public static float[,] B = {
										{ 5.639000000000000f },
										{ -39.940000000000000f }
								   };
		public static float[,] C = {
										{ 1f, 0f }
								   };
		public static float[,] L = {
										{ 1.218000000000000f },
										{ -0.170476000000000f }
								   };
		public static float[,] K = {
										{ 0f, 0.8f }
								   };

		public static float[,] Multiply(float[,] a, float[,] b)
		{
			if (a.GetLength(0) == b.GetLength(1))
			{
				float[,] c = new float[a.GetLength(1), b.GetLength(0)];
				for (int i = 0; i < c.GetLength(0); i++)
				{
					for (int j = 0; j < c.GetLength(1); j++)
					{
						c[i, j] = 0;
						for (int k = 0; k < a.GetLength(1); k++)
							c[i, j] = c[i, j] + a[i, k] * b[k, j];
					}
				}
				return c;
			}
			else
			{
				Console.WriteLine("\n Number of columns in First Matrix should be equal to Number of rows in Second Matrix.");
				return new float[0,0];
			}
		}

		public static float[,] Sum(float[,] a, float[,] b)
		{
			if (a.GetLength(0) == b.GetLength(0) && a.GetLength(1) == b.GetLength(1))
			{
				float[,] c = new float[a.GetLength(0), a.GetLength(1)];
				for (int i = 0; i < a.GetLength(0); i++)
				{
					for (int j = 0; j < a.GetLength(1); j++)
					{
						c[i, j] = a[i, j] + b[i, j];
					}
				}
				return c;
			}
			else
			{
				Console.WriteLine("\n Column and/or row numbers don't match.");
				return new float[0, 0];
			}
		}
	}
}
