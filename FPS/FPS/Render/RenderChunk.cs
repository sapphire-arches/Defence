using FPS.Game.HMap;
using FPS.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class RenderChunk {
		public static readonly int CHUNK_SIZE = Chunk.CHUNK_SIZE;
		static int _tex = 0;
		int _cx;
		int _cy;
		float[] _verts;
		float[] _color;
		float[] _norms;
		float[] _tc;
		bool _water;
		float[] _wverts;
		float[] _wcolor;
		float[] _wnorms;
		float[] _wtc;
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

		public RenderChunk(HeightMap In, Perlin2D PColor, int CX, int CY, int Level) {
			if (_tex == 0) {
				_tex = GL.GenTexture();
				Bitmap texture = new Bitmap(1, 1);
				texture.SetPixel(0, 0, System.Drawing.Color.White);
				BitmapData data = texture.LockBits(
					new Rectangle(0, 0, texture.Width, texture.Height),
					ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				GL.BindTexture(TextureTarget.Texture2D, _tex);
				GL.TexImage2D(TextureTarget.Texture2D, 0,
				              PixelInternalFormat.Rgba, data.Width, data.Height,
				              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
				texture.UnlockBits(data);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			}

			_lod = Level;
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 coords per vert.
			_verts = new float[_lod * _lod * 2 * 3 * 3];
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 normal coords per vert.
			_norms = new float[_lod * _lod * 2 * 3 * 3];
			//LOD squared squares * 2 tris per square * 3 verts per tri * 3 colors per vert.
			_color = new float[_lod * _lod * 2 * 3 * 3];
			//LOD squared squares * 2 tris per square * 3 verts per tri * 2 texture coords per vert.
			_tc = new float[_lod * _lod * 2 * 3 * 2];
			
			_cx = CX;
			_cy = CY;
			int itr = CHUNK_SIZE / _lod;
			FillPosAndColor(In, PColor, itr);
			FillWaterPosAndColor(itr, PColor);
		}

		public void Render() {
			GL.VertexPointer(3, VertexPointerType.Float, 0, _verts);
			GL.ColorPointer(3, ColorPointerType.Float, 0, _color);
			GL.NormalPointer(NormalPointerType.Float, 0, _norms);
			GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, _tc);
			GL.DrawArrays(BeginMode.Triangles, 0, _verts.Length / 3);
		}

		public void RenderWater() {
			if (_water) {
				GL.VertexPointer(3, VertexPointerType.Float, 0, _wverts);
				GL.ColorPointer(3, ColorPointerType.Float, 0, _wcolor);
				GL.NormalPointer(NormalPointerType.Float, 0, _wnorms);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, _tc);
				GL.DrawArrays(BeginMode.Triangles, 0, _wverts.Length / 3);
			}
		}

		void FillPosAndColor(HeightMap In, Perlin2D PColor, int itr) {
			int basex = _cx * CHUNK_SIZE;
			int basey = _cy * CHUNK_SIZE;

			Vector3[] vtemp = new Vector3[4];

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

					vtemp [3].X = xp + itr;
					vtemp [3].Y = In [xp + itr, yp + itr];
					vtemp [3].Z = yp + itr;

					Quad(vtemp, _verts, _norms, x, y);

					for (int i = 0; i < 18; i += 3) {
						float xx = _verts [basei + i + 0];
						float zz = _verts [basei + i + 2];
						float f1 = (float)PColor [xx * 0.5, zz * 0.5];
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
		}

		void FillWaterPosAndColor(int itr, Perlin2D PColor) {
			if (_water) {
				int basex = _cx * CHUNK_SIZE;
				int basey = _cy * CHUNK_SIZE;
				Vector3[] vtemp = new Vector3[3];
				int witr = itr * 2;
				//2 tris, 3 verts/tri, 3 coords / vert;
				_wverts = new float[((_lod * _lod) / 2) * 3 * 3];
				_wcolor = new float[_wverts.Length];
				_wnorms = new float[_wverts.Length];
				//2 tris, 3 verts / tri, 2 coords / vert
				_wtc = new float[((_lod * _lod) / 2) * 3 * 2];
				for (int x = 0; x < _lod / 2; ++x) {
					int xp = basex + x * witr;
					for (int y = 0; y < _lod / 2; ++y) {
						int yp = basey + y * witr;
						int basei = ((x + (_lod / 2) * y) * 18);

						vtemp [0].X = xp + 0;
						vtemp [0].Y = 0;
						vtemp [0].Z = yp + 0;

						vtemp [1].X = xp + 0;
						vtemp [1].Y = 0;
						vtemp [1].Z = yp + witr;

						vtemp [2].X = xp + witr;
						vtemp [2].Y = 0;
						vtemp [2].Z = yp + 0;

						vtemp [3].X = xp + witr;
						vtemp [3].Y = 0;
						vtemp [3].Z = yp + witr;

						Quad(vtemp, _wverts, _wnorms, x, y);

						for (int i = 0; i < 18; i += 3) {
							float f1 = (float)PColor [_wverts [basei + i + 0], _wverts [basei + i + 2]];
							_wcolor [basei + i + 0] = 0.3f * f1 + 0.2f;
							_wcolor [basei + i + 1] = 0.3f * f1 + 0.3f;
							_wcolor [basei + i + 2] = 0.3f * f1 + 0.7f;
						}
					}
				}
			}
		}

		void Quad(Vector3[] v, float[] verts, float[] norms, float[] tex, int x, int y, int itr) {
			int basei = (x + _lod * y) * 18;
			ArraySegment<Vector3> t1 = new ArraySegment<Vector3>(v, 0, 3);
			Tri(t1, verts, norms, basei);
			ArraySegment<Vector3> t2 = new ArraySegment<Vector3>(v, 1, 3);
			Vector3 tmp = v [1];
			v [1] = v [3];
			v [3] = tmp;
			Tri(t2, verts, norms, basei + 9);
			basei = (x + _lod * y) * 12; //standard LOD index, * 3 verts per tri * 2 tris * 2 coords per vert
			tex [basei + 00] = (float)(x * itr) / _lod;
		}

		void Tri(ArraySegment<Vector3> v, float[] verts, float[] norms, int basei) {
			Vector3 N = Normal(v);
			for (int i = 0; i < 9; i += 3) {
				int ii = i / 3;
				//Vert
				verts [basei + i + 0] = v.Array [v.Offset + ii].X;
				verts [basei + i + 1] = v.Array [v.Offset + ii].Y;
				verts [basei + i + 2] = v.Array [v.Offset + ii].Z;
				//Normal
				norms [basei + i + 0] = N.X;
				norms [basei + i + 1] = N.Y;
				norms [basei + i + 2] = N.Z;
			}
		}

		Vector3 Normal(ArraySegment<Vector3> v) {
			//Normal calc
			Vector3 U = Vector3.Subtract(v.Array [v.Offset + 1], v.Array [v.Offset + 0]);
			Vector3 V = Vector3.Subtract(v.Array [v.Offset + 2], v.Array [v.Offset + 0]);
			Vector3 N = Vector3.Cross(U, V);
			return Vector3.Normalize(N);
		}
	}
}
