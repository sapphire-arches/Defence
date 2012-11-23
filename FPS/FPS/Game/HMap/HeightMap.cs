using System;
using System.IO;
using FPS.Util;

namespace FPS.Game.HMap {
	public class HeightMap {
		int _width;
		int _height;
		ChunkCache _cache;

		public int Width {
			get { return _width; }
			private set { _width = value; }
		}

		public int Height {
			get { return _height; }
			private set { _height = value; }
		}

		public HeightMap(string FName) {
			this.Width = 128;
			this.Height = 128;
			Perlin2D p2d = new Perlin2D(100);
			this._cache = new ChunkCache(p2d);
		}

		public float this [int X, int Y] {
			get {
				int cx = X / Chunk.CHUNK_SIZE;
				int cy = Y / Chunk.CHUNK_SIZE;
				int lx = X - (cx * Chunk.CHUNK_SIZE);
				int ly = Y - (cy * Chunk.CHUNK_SIZE);

				if (lx < 0)
					lx += Chunk.CHUNK_SIZE;
				if (ly < 0)
					ly += Chunk.CHUNK_SIZE;
				return _cache [cx, cy] [lx, ly];
			}

			private set {
				int cx = X / Chunk.CHUNK_SIZE;
				int cy = Y / Chunk.CHUNK_SIZE;
				int lx = X - (cx * Chunk.CHUNK_SIZE);
				int ly = Y - (cy * Chunk.CHUNK_SIZE);

				if (lx < 0)
					lx += Chunk.CHUNK_SIZE;
				if (ly < 0)
					ly += Chunk.CHUNK_SIZE;
				_cache [cx, cy] [lx, ly] = value;
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