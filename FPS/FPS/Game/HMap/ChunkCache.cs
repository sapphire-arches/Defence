using System;
using FPS.Util;

namespace FPS.Game.HMap {
	public class ChunkCache {
		public static readonly int NUM_CHUNKS = 16;
		Chunk[,] _chunks;
		Perlin2D _p2d;

		public ChunkCache(Perlin2D Source) {
			_chunks = new Chunk[NUM_CHUNKS, NUM_CHUNKS];
			_p2d = Source;
		}

		public Chunk this [int X, int Y] {
			get {
				int x = X % NUM_CHUNKS;
				int y = Y % NUM_CHUNKS;
				if (x < 0)
					x += NUM_CHUNKS;
				if (y < 0)
					y += NUM_CHUNKS;
				Chunk tr = _chunks [x, y];
				if (tr == null || tr.X != X || tr.Y != Y) {
					float[,] temp = new float[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
					for (int xx = 0; xx < Chunk.CHUNK_SIZE; ++xx) {
						for (int yy = 0; yy < Chunk.CHUNK_SIZE; ++yy) {
							int xxx = xx + X * Chunk.CHUNK_SIZE;
							int yyy = yy + Y * Chunk.CHUNK_SIZE;
							temp [xx, yy] = (float)_p2d [xxx * 0.1, yyy * 0.1] * 5f;
						}
					}
					tr = new Chunk(X, Y, temp);
					_chunks [x, y] = tr;
				}
				return tr;
			}	
			private set {

			}
		}
	}
}

