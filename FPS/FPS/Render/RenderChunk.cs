using System;
using FPS.Game.HMap;
using FPS.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class RenderChunk {
		public static readonly int CHUNK_SIZE = Chunk.CHUNK_SIZE;
		int _cx;
		int _cy;
		float[] _verts;
		float[] _color;
		float[] _norms;
		bool _water;
		float[] _wverts;
		float[] _wcolor;
		float[] _wnorms;
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
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 normal coords per vert.
			_norms = new float[_lod * _lod * 2 * 3 * 3];
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 colors per vert.
			_color = new float[_lod * _lod * 2 * 3 * 3];
			
			_cx = CX;
			_cy = CY;

			int basex = _cx * CHUNK_SIZE;
			int basey = _cy * CHUNK_SIZE;

			int itr = CHUNK_SIZE / _lod;

			Vector3[] vtemp = new Vector3[3];
			Vector3[] ctemp = new Vector3[3];

			for (int x = 0; x < _lod; ++x) {
				int xp = basex + x * itr;
				for (int y = 0; y < _lod; ++y) {
					int yp = basey + y * itr;
					int basei = ((x + _lod * y) * 18);

					vtemp [0].X = xp + 0;
					vtemp [0].Y = In [xp + 0, yp + 0];
					vtemp [0].Z = yp + 0;

					vtemp [1].X = xp + 0;
					vtemp [1].Y = In [xp + 0, yp + itr];
					vtemp [1].Z = yp + itr;

					vtemp [2].X = xp + itr;
					vtemp [2].Y = In [xp + itr, yp + 0];
					vtemp [2].Z = yp + 0;

					Tri(vtemp, ctemp, _verts, _norms, _color, basei);

					vtemp [0].X = xp + itr;
					vtemp [0].Y = In [xp + itr, yp + 0];
					vtemp [0].Z = yp + 0;

					vtemp [1].X = xp + 0;
					vtemp [1].Y = In [xp + 0, yp + itr];
					vtemp [1].Z = yp + itr;

					vtemp [2].X = xp + itr;
					vtemp [2].Y = In [xp + itr, yp + itr];
					vtemp [2].Z = yp + itr;

					Tri(vtemp, ctemp, _verts, _norms, _color, basei + 9);

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
				_wnorms = new float[_wverts.Length];
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

		void Tri(Vector3[] v, Vector3[] c, float[] verts, float[] norms, float[] color, int basei) {
			Vector3 N = Normal(v);
			for (int i = 0; i < 9; i += 3) {
				int ii = i / 3;
				//Vert
				verts [basei + i + 0] = v [ii].X;
				verts [basei + i + 1] = v [ii].Y;
				verts [basei + i + 2] = v [ii].Z;
				//Color
				color [basei + i + 0] = c [ii].X;
				color [basei + i + 1] = c [ii].Y;
				color [basei + i + 2] = c [ii].Z;
				//Normal
				norms [basei + i + 0] = N.X;
				norms [basei + i + 1] = N.Y;
				norms [basei + i + 2] = N.Z;
			}
		}

		Vector3 Normal(Vector3[] v) {
			//Normal calc
			Vector3 U = Vector3.Subtract(v [1], v [0]);
			Vector3 V = Vector3.Subtract(v [2], v [0]);
			Vector3 N = new Vector3(0, 0, 0);
			N.X = (U.Y * V.Z) - (U.Z * V.Y);
			N.Y = (U.Z * V.X) - (U.X * V.Z);
			N.Z = (U.X * V.Y) - (U.Y * V.X);
			return Vector3.Normalize(N);
		}

		public void Render() {
			GL.VertexPointer(3, VertexPointerType.Float, 0, _verts);
			GL.ColorPointer(3, ColorPointerType.Float, 0, _color);
			GL.NormalPointer(NormalPointerType.Float, 0, _norms);
			GL.DrawArrays(BeginMode.Triangles, 0, _verts.Length / 3);
		}

		public void RenderWater() {
			if (_water) {
				GL.VertexPointer(3, VertexPointerType.Float, 0, _wverts);
				GL.ColorPointer(3, ColorPointerType.Float, 0, _wcolor);
				GL.NormalPointer(NormalPointerType.Float, 0, _wnorms);
				GL.DrawArrays(BeginMode.Triangles, 0, _wverts.Length / 3);
			}
		}
	}
}

