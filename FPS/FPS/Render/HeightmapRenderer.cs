using System;
using FPS.Game;
using FPS.Game.HMap;
using FPS.GLInterface;
using FPS.Util;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class HeightmapRenderer {
		public static readonly int CHUNK_SIZE = Chunk.CHUNK_SIZE;
		public static readonly int NUM_RENDER_CHUNKS = 16;
		const int PASS_GROUND = 0;
		const int PASS_WATER = 1;
		HeightMap _for;
		Perlin2D _p2d;
		RenderChunk[,] _renchunks;

		public HeightmapRenderer(HeightMap For) {
			_for = For;
			_p2d = new Perlin2D(100);
			_renchunks = new RenderChunk[NUM_RENDER_CHUNKS, NUM_RENDER_CHUNKS];
		}

		public void Render(float X, float Y) {
			Render(X, Y, PASS_GROUND);
		}

		public void RenderWater(float X, float Y) {
			Render(X, Y, PASS_WATER);
		}

		void Render(float X, float Y, int Pass) {
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.NormalArray);

			int mincx = Floor(X / CHUNK_SIZE) - 4;
			int mincy = Floor(Y / CHUNK_SIZE) - 4;

			for (int cx = mincx; cx < mincx + 9; ++cx) {
				for (int cy = mincy; cy < mincy + 9; ++cy) {

					int cox = cx - mincx;
					int coy = cy - mincy;

					cox = Abs(cox - 4);
					coy = Abs(coy - 4);
					int d = cox + coy;
					--d;
					if (d < 0)
						d = 0;
					if (cox == 1 && coy == 1)
						d = 0;

					int LOD = 32;
					LOD >>= d;
					if (LOD < 1)
						LOD = 1;
					
					int x = cx % NUM_RENDER_CHUNKS;
					int y = cy % NUM_RENDER_CHUNKS;
					if (x < 0)
						x += NUM_RENDER_CHUNKS;
					if (y < 0)
						y += NUM_RENDER_CHUNKS;
					if (_renchunks [x, y] == null || _renchunks [x, y].X != cx || _renchunks [x, y].Y != cy || _renchunks [x, y].LOD != LOD)
						_renchunks [x, y] = new RenderChunk(_for, _p2d, cx, cy, LOD);
					switch (Pass) {
						case PASS_GROUND:
							_renchunks [x, y].Render();
							break;
						case PASS_WATER:
							_renchunks [x, y].RenderWater();
							break;
						default:
							throw new ArgumentException("Pass " + Pass + " out of range");
					}
				}
			}
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.NormalArray);
			GL.DisableClientState(ArrayCap.ColorArray);
		}

		private int Abs(int I) {
			if (I < 0)
				return -I;
			return I;
		}

		private int Floor(float F) {
			if (F < 0)
				return (int)F - 1;
			return (int)F;
		}
	}
}

