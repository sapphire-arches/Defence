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
		bool _water;
		float[] _wverts;
		float[] _wcolor;

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
						if (_verts [basei + i + 1] > 0) {
							_color [basei + i + 0] = f1 * 0.05f + 0.2f;
							_color [basei + i + 1] = f1 * 0.8f + 0.2f;
							_color [basei + i + 2] = f1 * 0.05f + 0.2f;
						} else {
							_water = true;
							//Base color is .56, .79, .11
							_color [basei + i + 0] = f1 * 0.1f + 0.51f;
							_color [basei + i + 1] = f1 * 0.1f + 0.74f;
							_color [basei + i + 2] = f1 * 0.1f + 0.06f;
						}
					}
				}
			}
			if (_water) {
				//2 tris, 3 verts/tri, 3 coords / vert;
				_wverts = new float[2 * 3 * 3];
				_wcolor = new float[2 * 3 * 3];
				_wverts [00] = basex + 0;
				_wverts [01] = 0;
				_wverts [02] = basey + 0;
				
				_wverts [03] = basex + 0;
				_wverts [04] = 0;
				_wverts [05] = basey + CHUNK_SIZE;
				
				_wverts [06] = basex + CHUNK_SIZE;
				_wverts [07] = 0;
				_wverts [08] = basey + 0;
				
				_wverts [09] = basex + CHUNK_SIZE;
				_wverts [10] = 0;
				_wverts [11] = basey + 0;
				
				_wverts [12] = basex + 0;
				_wverts [13] = 0;
				_wverts [14] = basey + CHUNK_SIZE;
				
				_wverts [15] = basex + CHUNK_SIZE;
				_wverts [16] = 0;
				_wverts [17] = basey + CHUNK_SIZE;
				for (int i = 0; i < _wcolor.Length; i += 3) {
					float f1 = (float)Color [_wverts [i + 0], _wverts [i + 2]];
					_wcolor [i + 0] = 0.3f * f1 + 0.1w f;
					_wcolor [i + 1] = 0.3f * f1 + 0.1f;
					_wcolor [i + 2] = 0.3f * f1 + 0.7f;
				}
			}
		}

		public void Render() {
			GL.VertexPointer(3, VertexPointerType.Float, 0, _verts);
			GL.ColorPointer(3, ColorPointerType.Float, 0, _color);
			GL.DrawArrays(BeginMode.Triangles, 0, _verts.Length / 3);
			RenderWater();
		}

		public void RenderWater() {
			if (_water) {
				GL.VertexPointer(3, VertexPointerType.Float, 0, _wverts);
				GL.ColorPointer(3, ColorPointerType.Float, 0, _wcolor);
				GL.DrawArrays(BeginMode.Triangles, 0, _wverts.Length / 3);
			}
		}
	}
}

