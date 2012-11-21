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
			this.Width = 32;
			this.Height = 32;
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
				return _map [X, Y];
			}

			private set {
				if (X >= Width || X < 0 || Y >= Height || Y < 0)
					return;
				_map [X, Y] = value;
			}
		}
	}
}