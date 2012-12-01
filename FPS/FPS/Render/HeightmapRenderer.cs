using System;
using FPS.Game;
using FPS.Game.HMap;
using FPS.GLInterface;
using FPS.Util;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class HeightmapRenderer {
		public const int CHUNK_SIZE = Chunk.CHUNK_SIZE;
		public const int NUM_RENDER_CHUNKS = 16;
		public const int VIEW_DIST = (int)(WorldRenderer.MAX_DEPTH / CHUNK_SIZE) + 1;
		const int PASS_GROUND = 0;
		const int PASS_WATER = 1;
		HeightMap _for;
		Perlin2D _p2d;
		RenderChunk[,] _renchunks;
		WorldRenderer _in;

		public HeightmapRenderer(WorldRenderer In, HeightMap For) {
			_in = In;
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

			int mincx = Floor(X / CHUNK_SIZE) - VIEW_DIST;
			int mincy = Floor(Y / CHUNK_SIZE) - VIEW_DIST;

			for (int cx = mincx; cx < mincx + VIEW_DIST + VIEW_DIST + 1; ++cx) {
				for (int cy = mincy; cy < mincy + VIEW_DIST + VIEW_DIST + 1; ++cy) {
					if (InFrustrum(cx, cy)) {
						int cox = cx - mincx;
						int coy = cy - mincy;

						cox = Abs(cox - VIEW_DIST);
						coy = Abs(coy - VIEW_DIST);
						int d = 1;//(int)(Math.Sqrt(cox * cox + coy * coy));

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
			}
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.NormalArray);
			GL.DisableClientState(ArrayCap.ColorArray);
		}

		private bool InFrustrum(int cx, int cy) {
			//Doesn't work.

			/* X ->     Y
			 * TL   TR  |
			 * +-----+ \ /
			 * |     |  *
			 * |     |
			 * +-----+
			 * BL    BR
			 * */
			double lx = cx * CHUNK_SIZE - _in.Pos.X;
			double ly = cy * CHUNK_SIZE - _in.Pos.Z;
			if (lx / CHUNK_SIZE < 2 || ly / CHUNK_SIZE < 2) {
				return true;
			}

			double tl = CorrectAngle(Math.Atan2(ly, lx));
			double tr = CorrectAngle(Math.Atan2(ly, lx + CHUNK_SIZE));
			double bl = CorrectAngle(Math.Atan2(ly + CHUNK_SIZE, lx));
			double br = CorrectAngle(Math.Atan2(ly + CHUNK_SIZE, lx + CHUNK_SIZE));
			
			const double halfpi = Math.PI / 2;
			const double quarterpi = halfpi / 2;
			double half = (WorldRenderer.FOV + quarterpi) / 2;
			double negyaw = CorrectAngle(-_in.Yaw - halfpi);

			return AngleCmp(tl, negyaw, half) ||
				AngleCmp(tr, negyaw, half) ||
				AngleCmp(bl, negyaw, half) ||
				AngleCmp(br, negyaw, half);// ||
			//(tl == 0 || tr == 0 || bl == 0 || br == 0);
		}

		double CorrectAngle(double A) {
			const double FULL = 2 * Math.PI;
			while (A < 0)
				A = A + FULL;
			A %= FULL;
			return A;
		}

		bool AngleCmp(double A1, double A2, double MaxDiff) {
			return Math.Abs(A1 - A2) < MaxDiff;
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

