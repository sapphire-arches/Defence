using FPS.Game.HMap;
using FPS.GLInterface;
using FPS.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class RenderChunk {
		public static readonly int CHUNK_SIZE = Chunk.CHUNK_SIZE;
		int _tex;
		int _wtex;
		int _cx;
		int _cy;
		bool _water;
		int _lod;

		uint _buffer;
		uint _wbuffer;
		ushort[] _indicies;
		ushort[] _windicies;

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
			_lod = Level;

			_cx = CX;
			_cy = CY;

			List<Vertex> verts = new List<Vertex>();
			List<Vertex> wverts = new List<Vertex>();
			List<ushort> indicies = new List<ushort>();
			List<ushort> windicies = new List<ushort>();
			BuildGround(In, verts, indicies);
			if (_water)
				BuildWater(In, wverts, windicies);

			uint[] tmp = new uint[2];
			GL.GenBuffers(2, tmp);
			_buffer = tmp [0];
			_wbuffer = tmp [1];
			//Vertex Data
			GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(verts.Count * Vertex.Size), verts.ToArray(), BufferUsageHint.StaticDraw);
			//Water vertex data
			GL.BindBuffer(BufferTarget.ArrayBuffer, _wbuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(wverts.Count * Vertex.Size), wverts.ToArray(), BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			#region Texture generation
			_tex = GLUtil.CreateTexture(GenGroundTexture(PColor));
			if (_water)
				_wtex = GLUtil.CreateTexture(GenWaterTexture(PColor));
			#endregion

			_indicies = indicies.ToArray();
			if (_water)
				_windicies = windicies.ToArray();
		}

		~RenderChunk() {
			GL.DeleteTexture(_tex);
			GL.DeleteTexture(_wtex);
			GL.DeleteBuffer(_buffer);
		}

		public void Render(WorldRenderer WR) {
			//Bind texture.
			WR.BindTexture(_tex);
			//Bind vertex data
			GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
			GL.InterleavedArrays(InterleavedArrayFormat.T2fC4fN3fV3f, 0, (IntPtr)0);

			GL.DrawElements(BeginMode.Triangles, _indicies.Length, DrawElementsType.UnsignedShort, _indicies);

			//Unbind buffer
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void RenderWater(WorldRenderer WR) {
			if (_water) {
				//Bind texture.
				WR.BindTexture(_wtex);
				//Bind vertex data
				GL.BindBuffer(BufferTarget.ArrayBuffer, _wbuffer);
				GL.InterleavedArrays(InterleavedArrayFormat.T2fC4fN3fV3f, 0, (IntPtr)0);
				
				GL.DrawElements(BeginMode.Triangles, _windicies.Length, DrawElementsType.UnsignedShort, _windicies);

				//Unbind buffer
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}
		}

		void BuildGround(HeightMap In, List<Vertex> V, List<ushort> I) {
			int basex = _cx * CHUNK_SIZE;
			int basey = _cy * CHUNK_SIZE;
			int itr = CHUNK_SIZE / _lod;
			for (int x = 0; x < CHUNK_SIZE + itr; x += itr) {
				int xp = basex + x;
				for (int y = 0; y < CHUNK_SIZE + itr; y += itr) {
					int yp = basey + y;
					Vertex tmp = new Vertex();
					SetPos(ref tmp, xp, In [xp, yp], yp);
					if (In [xp, yp] < 0)
						_water = true;
					tmp.TexCoord.X = x / (float)CHUNK_SIZE;
					tmp.TexCoord.Y = y / (float)CHUNK_SIZE;
					tmp.Color = new Vector4(1, 1, 1, 1);
					V.Add(tmp);
				}
			}
			for (int x = 0; x < _lod; ++x) {
				for (int y = 0; y < _lod; ++y) {
					int vi = x + y * (_lod + 1);
					I.Add((ushort)(vi + _lod + 1));
					I.Add((ushort)(vi));
					I.Add((ushort)(vi + 1));
					I.Add((ushort)(vi + _lod + 1));
					I.Add((ushort)(vi + 1));
					I.Add((ushort)(vi + _lod + 2));
				}
			}
		}

		void BuildWater(HeightMap In, List<Vertex> V, List<ushort> I) {
			int basex = _cx * CHUNK_SIZE;
			int basey = _cy * CHUNK_SIZE;
			int itr = CHUNK_SIZE / _lod;
			for (int x = 0; x < CHUNK_SIZE + itr; x += itr) {
				int xp = basex + x;
				for (int y = 0; y < CHUNK_SIZE + itr; y += itr) {
					int yp = basey + y;
					Vertex tmp = new Vertex();
					SetPos(ref tmp, xp, 0, yp);
					tmp.TexCoord.X = x / (float)CHUNK_SIZE;
					tmp.TexCoord.Y = y / (float)CHUNK_SIZE;
					tmp.Color = new Vector4(1, 1, 1, 1);
					V.Add(tmp);
				}
			}
			for (int x = 0; x < _lod; ++x) {
				for (int y = 0; y < _lod; ++y) {
					int vi = x + y * (_lod + 1);
					I.Add((ushort)(vi + _lod + 1));
					I.Add((ushort)(vi));
					I.Add((ushort)(vi + 1));
					I.Add((ushort)(vi + _lod + 1));
					I.Add((ushort)(vi + 1));
					I.Add((ushort)(vi + _lod + 2));
				}
			}
		}

		void SetPos(ref Vertex V, float X, float Y, float Z) {
			V.Position.X = X;
			V.Position.Y = Y;
			V.Position.Z = Z;
		}

		Vector3 Normal(Vector3 v1, Vector3 v2, Vector3 v3) {
			//Normal calc
			Vector3 U = Vector3.Subtract(v2, v1);
			Vector3 V = Vector3.Subtract(v3, v1);
			Vector3 N = Vector3.Cross(U, V);
			return Vector3.Normalize(N);
		}

		Bitmap GenGroundTexture(Perlin2D C) {
			return GenTexture(C, Color.Green, Color.Yellow, 0.1f);
		}

		Bitmap GenWaterTexture(Perlin2D C) {
			return GenTexture(C, Color.Blue, Color.BlueViolet, 0.5f);
		}

		Bitmap GenTexture(Perlin2D C, Color C1, Color C2, float Scale) {
			Bitmap bm = new Bitmap(_lod * 2, _lod * 2);
			for (int x = 0; x < bm.Width; ++x) {
				int xp = _cx * CHUNK_SIZE + x;
				for (int y = 0; y < bm.Height; ++y) {
					int yp = _cy * CHUNK_SIZE + y;
					float f = Octaves(C, xp * Scale, yp * Scale, 4);
					if (f > 1)
						f = 1;
					Color c = Mix(C1, C2, f);
					bm.SetPixel(x, y, c);
				}
			}
			return bm;
		}

		float Octaves(Perlin2D P, float X, float Y, int Num) {
			double total = 0;
			for (int i = 0; i < Num; ++i) {
				double freq = Math.Pow(2, i);
				double ampl = Math.Pow(0.5, i);
				double per = P [X * freq, Y * freq];
				per = (per - 0.5) * 2;
				total += per * ampl;
			}
			return (float)Math.Abs(total);
		}

		Color Mix(Color C1, Color C2, float Fac) {
			Color tr = Color.FromArgb((int)(C1.R * Fac + C2.R * (1 - Fac)),
			                          (int)(C1.G * Fac + C2.G * (1 - Fac)),
			                          (int)(C1.B * Fac + C2.B * (1 - Fac)));
			return tr;
		}
	}
}
