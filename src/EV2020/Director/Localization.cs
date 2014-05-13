using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class Localization
	{
		static Func<int, int, double> initZero = delegate(int x, int y)
                     {
                         return 0;
                     };
		static Matrix<double> Toep(Matrix<double> x0, int N, int L)
		{
			if (x0.ColumnCount != 1)
			{
				throw new ArgumentException("x0 needs to be a column vector.", "x0");
			}
			int N0 = x0.RowCount;
			if (N0>N)
			{
				throw new ArgumentException("x0's lenght should be equal or less than N.", "x0");
			}
			Matrix<double> X = DenseMatrix.Create(N, L, initZero);
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < L; j++)
				{
					int index = ((i + 1) - (j + 1)) % N + 1;
					if (index <= N0)
					{
						X[i, j] = x0[index-1,1];
					}
				}
			}
			return X;
		}
		static Matrix<double> Toep(Matrix<double> x0, int N)
		{
			int N0 = x0.RowCount;
			return Toep(x0, N, N - N0 + 1);
		}
	}
}
