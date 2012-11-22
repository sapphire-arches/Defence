using System;
using System.IO;
using FPS.Util;

namespace FPS.Game {
	public class HeightMap {
		int _width;
		int _height;

		public int Width {
			get { return _width; }
			private set { _width = value; }
		}

		public int Height {
			get { return _height; }
			private set { _height = value; }
		}

		float[,] _map;

		public HeightMap(string FName) {
			this.Width = 128;
			this.Height = 128;
			this._map = new float[Width, Height];

			Perlin2D p2d = new Perlin2D(100);

			for (int x = 0; x < Width; ++x) {
				for (int y = 0; y < Height; ++y) {
					this [x, y] = (float)p2d [x * 0.1, y * 0.1] * 5f;
				}
			}
		}

		public float this [int X, int Y] {
			get {
				if (X >= Width || X < 0 || Y >= Height || Y < 0) {
					return 0;
				}
				float val = 0;
				try {
					val = _map [X, Y];
				} catch (IndexOutOfRangeException ex) {
					Console.WriteLine("{0} {1}", X, Y);
				}
				return val;
			}

			private set {
				if (X >= Width || X < 0 || Y >= Height || Y < 0)
					return;
				_map [X, Y] = value;
			}
		}

		public float this [float X, float Y] {
			get {
				int ix = Floor(X);
				int iy = Floor(Y);
				float fx = X - ix;
				float fy = Y - iy;

				float d1 = this [ix + 0, iy + 0];
				float d2 = this [ix + 1, iy + 0];
				float d3 = this [ix + 0, iy + 1];
				float d4 = this [ix + 1, iy + 1];

				d1 = Inter(d1, d2, fx);
				d2 = Inter(d3, d4, fx);

				return Inter(d1, d2, fy);
			}

			private set {
				throw new InvalidOperationException("You can't set that!");
			}
		}

		private int Floor(float F) {
			if (F < 0)
				return (int)F - 1;
			return (int)F;
		}

		private float Inter(float D1, float D2, float F) {
			return D1 * (1 - F) + D2 * F;
		}
	}
}