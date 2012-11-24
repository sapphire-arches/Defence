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
		int _lod;

		public int X {
			get { return _cx; }
			private set { ;}
		}

		public int Y {
			get { return _cy; }
			private set { ;}
		}

		public int LOD {
			get { return _lod; }
			set { _lod = value;}
		}

		public RenderChunk(HeightMap In, Perlin2D Color, int CX, int CY, int Level) {
			_lod = Level;
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 coords per vert.
			_verts = new float[_lod * _lod * 2 * 3 * 3];
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 colors per vert.
			_color = new float[_lod * _lod * 2 * 3 * 3];
			
			_cx = CX;
			_cy = CY;

			int basex = _cx * CHUNK_SIZE;
			int basey = _cy * CHUNK_SIZE;

			int itr = CHUNK_SIZE / _lod;

			for (int x = 0; x < _lod; ++x) {
				int xp = basex + x * itr;
				for (int y = 0; y < _lod; ++y) {
					int yp = basey + y * itr;
					int basei = ((x + _lod * y) * 18);
					_verts [basei + 00] = xp + 0;
					_verts [basei + 01] = In [xp + 0, yp + 0];
					_verts [basei + 02] = yp + 0;

					_verts [basei + 03] = xp + 0;
					_verts [basei + 04] = In [xp + 0, yp + itr];
					_verts [basei + 05] = yp + itr;

					_verts [basei + 06] = xp + itr;
					_verts [basei + 07] = In [xp + itr, yp + 0];
					_verts [basei + 08] = yp + 0;

					_verts [basei + 09] = xp + itr;
					_verts [basei + 10] = In [xp + itr, yp + 0];
					_verts [basei + 11] = yp + 0;

					_verts [basei + 12] = xp + 0;
					_verts [basei + 13] = In [xp + 0, yp + itr];
					_verts [basei + 14] = yp + itr;

					_verts [basei + 15] = xp + itr;
					_verts [basei + 16] = In [xp + itr, yp + itr];
					_verts [basei + 17] = yp + itr;

					for (int i = 0; i < 18; i += 3) {
						float xx = _verts [basei + i + 0];
						float zz = _verts [basei + i + 2];
						float f1 = (float)Color [xx * 0.5, zz * 0.5];
						if (_verts [basei + i + 1] >= 3) {
							_color [basei + i + 0] = f1 * 0.1f + 0.3f;
							_color [basei + i + 1] = f1 * 0.3f + 0.6f;
							_color [basei + i + 2] = f1 * 0.1f + 0.3f;
						} else {
							_water = true;
							//Base color is .76, .79, .21
							_color [basei + i + 0] = f1 * 0.1f + 0.71f;
							_color [basei + i + 1] = f1 * 0.1f + 0.74f;
							_color [basei + i + 2] = f1 * 0.1f + 0.26f;
						}
					}
				}
			}
			if (_water) {
				int witr = itr * 2;
				//2 tris, 3 verts/tri, 3 coords / vert;
				_wverts = new float[((_lod * _lod) / 2) * 3 * 3];
				_wcolor = new float[_wverts.Length];
				for (int x = 0; x < _lod / 2; ++x) {
					int xp = basex + x * witr;
					for (int y = 0; y < _lod / 2; ++y) {
						int yp = basey + y * witr;
						int basei = ((x + (_lod / 2) * y) * 18);
						_wverts [basei + 00] = xp + 0;
						_wverts [basei + 01] = 0;
						_wverts [basei + 02] = yp + 0;
				
						_wverts [basei + 03] = xp + 0;
						_wverts [basei + 04] = 0;
						_wverts [basei + 05] = yp + witr;
				
						_wverts [basei + 06] = xp + witr;
						_wverts [basei + 07] = 0;
						_wverts [basei + 08] = yp + 0;
				
						_wverts [basei + 09] = xp + witr;
						_wverts [basei + 10] = 0;
						_wverts [basei + 11] = yp + 0;
				
						_wverts [basei + 12] = xp + 0;
						_wverts [basei + 13] = 0;
						_wverts [basei + 14] = yp + witr;
				
						_wverts [basei + 15] = xp + witr;
						_wverts [basei + 16] = 0;
						_wverts [basei + 17] = yp + witr;
						for (int i = 0; i < 18; i += 3) {
							float f1 = (float)Color [_wverts [basei + i + 0], _wverts [basei + i + 2]];
							_wcolor [basei + i + 0] = 0.3f * f1 + 0.2f;
							_wcolor [basei + i + 1] = 0.3f * f1 + 0.3f;
							_wcolor [basei + i + 2] = 0.3f * f1 + 0.7f;
						}
					}
				}
			}
		}

		public void Render() {
			GL.VertexPointer(3, VertexPointerType.Float, 0, _verts);
			GL.ColorPointer(3, ColorPointerType.Float, 0, _color);
			GL.DrawArrays(BeginMode.Triangles, 0, _verts.Length / 3);
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

