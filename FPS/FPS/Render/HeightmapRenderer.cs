using System;
using FPS.Game;
using FPS.GLInterface;
using FPS.Util;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class HeightmapRenderer {
		const int CHUNK_SIZE = 16;
		HeightMap _for;
		VertexArray[,] _chunks;

		public HeightmapRenderer(HeightMap For) {
			_for = For;
			_chunks = new VertexArray[(_for.Width / 16 + 1), (_for.Height / 16 + 1)];
			Perlin2D p2d = new Perlin2D(100);
			for (int cx = 0; cx < _for.Width / CHUNK_SIZE + 1; ++cx) {
				for (int cy = 0; cy < _for.Height / CHUNK_SIZE + 1; ++cy) {
					//number of quads in chunk * 2 tris per quad * 3 verts per tri * 3 coords per vert
					float[] verts = new float[CHUNK_SIZE * CHUNK_SIZE * 18];
					float[] color = new float[CHUNK_SIZE * CHUNK_SIZE * 18];
					int basex = cx * CHUNK_SIZE;
					int basey = cy * CHUNK_SIZE;

					for (int x = 0; x < CHUNK_SIZE; ++x) {
						int xp = basex + x;
						for (int y = 0; y < CHUNK_SIZE; ++y) {
							int yp = basey + y;
							verts [((x + CHUNK_SIZE * y) * 18) + 00] = xp + 0;
							verts [((x + CHUNK_SIZE * y) * 18) + 01] = _for [xp + 0, yp + 0];
							verts [((x + CHUNK_SIZE * y) * 18) + 02] = yp + 0;
									
							verts [((x + CHUNK_SIZE * y) * 18) + 03] = xp + 0;
							verts [((x + CHUNK_SIZE * y) * 18) + 04] = _for [xp + 0, yp + 1];
							verts [((x + CHUNK_SIZE * y) * 18) + 05] = yp + 1;
									
							verts [((x + CHUNK_SIZE * y) * 18) + 06] = xp + 1;
							verts [((x + CHUNK_SIZE * y) * 18) + 07] = _for [xp + 1, yp + 0];
							verts [((x + CHUNK_SIZE * y) * 18) + 08] = yp + 0;

							verts [((x + CHUNK_SIZE * y) * 18) + 09] = xp + 1;
							verts [((x + CHUNK_SIZE * y) * 18) + 10] = _for [xp + 1, yp + 0];
							verts [((x + CHUNK_SIZE * y) * 18) + 11] = yp + 0;
									
							verts [((x + CHUNK_SIZE * y) * 18) + 12] = xp + 0;
							verts [((x + CHUNK_SIZE * y) * 18) + 13] = _for [xp + 0, yp + 1];
							verts [((x + CHUNK_SIZE * y) * 18) + 14] = yp + 1;
									
							verts [((x + CHUNK_SIZE * y) * 18) + 15] = xp + 1;
							verts [((x + CHUNK_SIZE * y) * 18) + 16] = _for [xp + 1, yp + 1];
							verts [((x + CHUNK_SIZE * y) * 18) + 17] = yp + 1;

							for (int i = 0; i < 18; i += 3) {
								color [((x + CHUNK_SIZE * y) * 18) + i + 0] = 0;
								color [((x + CHUNK_SIZE * y) * 18) + i + 1] = (float)p2d [xp * 0.1, yp * 0.1] / 3f;
								color [((x + CHUNK_SIZE * y) * 18) + i + 2] = 0;
							}
						}
					}

					_chunks [cx, cy] = new VertexArray(verts, color, null, verts.Length / 3, BeginMode.Triangles);
				}
			}
		}

		public void Render() {
			GL.Enable(EnableCap.VertexArray);
			GL.Enable(EnableCap.ColorArray);
			for (int cx = 0; cx < _for.Width / CHUNK_SIZE + 1; ++cx) {
				for (int cy = 0; cy < _for.Height / CHUNK_SIZE + 1; ++cy) {
					_chunks [cx, cy].Draw();
				}
			}
			GL.Disable(EnableCap.VertexArray);
			GL.Enable(EnableCap.ColorArray);
		}
	}
}

