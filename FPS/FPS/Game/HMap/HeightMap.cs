using System;
using System.IO;
using FPS.Util;

namespace FPS.Game.HMap {
	public class HeightMap {
		ChunkCache _cache;

		public HeightMap(string FName) {
			Perlin2D p2d = new Perlin2D(100);
			this._cache = new ChunkCache(new IslandGenerator(p2d));
		}

		public float this [int X, int Y] {
			get {
				int cx = X / Chunk.CHUNK_SIZE;
				int cy = Y / Chunk.CHUNK_SIZE;
				int lx = X % Chunk.CHUNK_SIZE;
				int ly = Y % Chunk.CHUNK_SIZE;
				if (X < 0 && lx != 0)
					--cx;
				if (Y < 0 && ly != 0)
					--cy;
				return -0.1f;//_cache [cx, cy] [lx, ly];
			}

			private set {
				int cx = X / Chunk.CHUNK_SIZE;
				int cy = Y / Chunk.CHUNK_SIZE;
				int lx = X % Chunk.CHUNK_SIZE;
				int ly = Y % Chunk.CHUNK_SIZE;
				if (X < 0 && lx != 0)
					--cx;
				if (Y < 0 && ly != 0)
					--cy;
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