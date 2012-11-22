using System;

namespace FPS.Util {
	public class Perlin2D {
		readonly int SIZE = 64;
		private float[,] vals;

		public Perlin2D(int seed) {
			Random r = new Random(seed);
			vals = new float[SIZE, SIZE];
			for (int x = 0; x < SIZE; ++x) {
				for (int y = 0; y < SIZE; ++y) {
					vals [x, y] = (float)r.NextDouble();
				}
			}
		}

		public double this [double X, double Y] {
			get {
				int ix = Floor(X);
				int iy = Floor(Y);
				double dx = X - ix;
				double dy = Y - iy;

				double d1 = Get(ix + 0, iy + 0);
				double d2 = Get(ix + 1, iy + 0);
				double d3 = Get(ix + 0, iy + 1);
				double d4 = Get(ix + 1, iy + 1);

				d1 = Inter(d1, d2, dx);
				d2 = Inter(d3, d4, dx);
				return Inter(d1, d2, dy);
			}

			private set {
				throw new InvalidOperationException("You can't set perlin values!");
			}
		}

		private int Floor(double D) {
			if (D < 0) {
				return (int)D - 1;
			}
			return (int)D;
		}

		private double Get(int X, int Y) {
			X %= SIZE;
			Y %= SIZE;
			if (X < 0)
				X += SIZE;
			if (Y < 0)
				Y += SIZE;
			return vals [X, Y];
		}

		private double Inter(double D1, double D2, double F) {
			double F2 = Math.Sin(F * Math.PI * 0.5);
			return D1 * (1 - F2) + D2 * F2;
		}
	}
}

