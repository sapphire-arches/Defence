using System;
using FPS.Game.HMap;
using FPS.Util;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class RenderChunk {
		public static readonly int CHUNK_SIZE = Chunk.CHUNK_SIZE;
		int _cx;
		int _cy;
		float[] _verts;
		float[] _color;

		public int X {
			get { return _cx; }
			private set { ;}
		}

		public int Y {
			get { return _cy; }
			private set { ;}
		}

		public RenderChunk(HeightMap In, Perlin2D Color, int CX, int CY) {
			//CHUNK_SIZE squared squares * 2 tris per square * 3 verts per tri * 3 coords per vert.
			_verts = new float[CHUNK_SIZE * CHUNK_SIZE * 2 * 3 * 3];
			//CHUNK_SIZE squared squares * 2 tris per square * 3 verts per tri * 3 colors per vert.
			_color = new float[CHUNK_SIZE * CHUNK_SIZE * 2 * 3 * 3];
			
			_cx = CX;
			_cy = CY;

			int basex = _cx * CHUNK_SIZE;
			int basey = _cy * CHUNK_SIZE;

			for (int x = 0; x < CHUNK_SIZE; ++x) {
				int xp = basex + x;
				for (int y = 0; y < CHUNK_SIZE; ++y) {
					int yp = basey + y;
					int basei = ((x + CHUNK_SIZE * y) * 18);
					_verts [basei + 00] = xp + 0;
					_verts [basei + 01] = In [xp + 0, yp + 0];
					_verts [basei + 02] = yp + 0;

					_verts [basei + 03] = xp + 0;
					_verts [basei + 04] = In [xp + 0, yp + 1];
					_verts [basei + 05] = yp + 1;

					_verts [basei + 06] = xp + 1;
					_verts [basei + 07] = In [xp + 1, yp + 0];
					_verts [basei + 08] = yp + 0;

					_verts [basei + 09] = xp + 1;
					_verts [basei + 10] = In [xp + 1, yp + 0];
					_verts [basei + 11] = yp + 0;

					_verts [basei + 12] = xp + 0;
					_verts [basei + 13] = In [xp + 0, yp + 1];
					_verts [basei + 14] = yp + 1;

					_verts [basei + 15] = xp + 1;
					_verts [basei + 16] = In [xp + 1, yp + 1];
					_verts [basei + 17] = yp + 1;

					for (int i = 0; i < 18; i += 3) {
						float f1 = (float)Color [xp * 0.5, yp * 0.5];
						_color [basei + i + 0] = f1 * 0.1f + 0.1f;
						_color [basei + i + 1] = f1 * 0.9f + 0.1f;
						_color [basei + i + 2] = f1 * 0.1f + 0.1f;
						//_verts [basei + i + 1] = 0;
					}
				}
			}
		}

		public void Render() {
			GL.VertexPointer(3, VertexPointerType.Float, 0, _verts);
			GL.ColorPointer(3, ColorPointerType.Float, 0, _color);
			GL.DrawArrays(BeginMode.Triangles, 0, _verts.Length / 3);
		}
	}
}

