using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	class Observer
	{
		public float[,] xDot;
		public float[,] x;

		private int lastTime;

		public Observer()
		{
			xDot = new float[,] {
									{0f},
									{0f}
								};
			x =	new float[,] {
								{3f},
								{0f}
							};
			lastTime = Environment.TickCount;
		}

		// Input yReal in cm
		public int Update(int yReal, int driving)
		{
			// Integrate the summed values of (L * (y - yReal) + B * u - L * x)
			// xDot = -BKx + Ax + L(yReal - Cx)

			float[,] minusBKx = Matrix.Multiply(Matrix.B, Matrix.K);
			minusBKx[0, 0] = minusBKx[0, 0] * driving;
			minusBKx[1, 0] = minusBKx[1, 0] * driving;
			float[,] Ax = Matrix.Multiply(Matrix.A, x);

			float deltaY = yReal / 100f - x[0, 0];
			float[,] LdeltaY = new float[,] {{Matrix.L[0,0] * deltaY}, {Matrix.L[1,0] * deltaY}};
			xDot = Matrix.Sum(Matrix.Sum(minusBKx, xDot), LdeltaY);

			// Integrate to x
			float deltaTime = (float)(Environment.TickCount - lastTime) / 1000f;
			x[0, 0] = x[0, 0] + xDot[0, 0] * deltaTime;
			x[1, 0] = x[1, 0] + xDot[1, 0] * deltaTime;

			// Calculate new braking signal
			float[,] brake = Matrix.Multiply(Matrix.K, x);
			return (int)brake[0,0];
		}
	}
}
