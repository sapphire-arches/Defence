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
		HeightMap _for;
		Perlin2D _p2d;
		RenderChunk[,] _renchunks;

		public HeightmapRenderer(HeightMap For) {
			_for = For;
			_p2d = new Perlin2D(100);
			_renchunks = new RenderChunk[NUM_RENDER_CHUNKS, NUM_RENDER_CHUNKS];
		}

		public void Render(float X, float Y) {
			GL.Enable(EnableCap.VertexArray);
			GL.Enable(EnableCap.ColorArray);

			int mincx = (int)(X / CHUNK_SIZE) - 4;
			int mincy = (int)(Y / CHUNK_SIZE) - 4;

			for (int cx = mincx; cx < mincx + 8; ++cx) {
				for (int cy = mincy; cy < mincy + 8; ++cy) {
					int x = cx % NUM_RENDER_CHUNKS;
					int y = cy % NUM_RENDER_CHUNKS;
					if (x < 0)
						x += NUM_RENDER_CHUNKS;
					if (y < 0)
						y += NUM_RENDER_CHUNKS;
					if (_renchunks [x, y] == null || _renchunks [x, y].X != cx || _renchunks [x, y].Y != cy)
						_renchunks [x, y] = new RenderChunk(_for, _p2d, cx, cy);
					_renchunks [x, y].Render();
				}
			}
			GL.Disable(EnableCap.VertexArray);
			GL.Enable(EnableCap.ColorArray);
		}
	}
}

