using System;
using FPS.Game;
using FPS.GLInterface;
using FPS.Util;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render {
	public class HeightmapRenderer {
		HeightMap _for;
		VertexArray[] _rows;

		public HeightmapRenderer(HeightMap For) {
			_for = For;
			_rows = new VertexArray[_for.Width];
			Perlin2D p2d = new Perlin2D(100);
			for (int x = 0; x < _for.Width; ++x) {
				//Height * 3 coords per vert * 3 verts per tri * 2 tris per square
				float[] verts = new float[_for.Height * 3 * 3 * 2];
				float[] color = new float[_for.Height * 3 * 3 * 2];
				for (int y = 0; y < _for.Height; ++y) {
					//Stride is 3 coords per vert * 6 verts per square
					verts [(y * 18) + 00] = x;
					verts [(y * 18) + 01] = _for [x, y];
					verts [(y * 18) + 02] = y;

					verts [(y * 18) + 03] = x + 1;
					verts [(y * 18) + 04] = _for [x + 1, y];
					verts [(y * 18) + 05] = y;
					
					verts [(y * 18) + 06] = x;
					verts [(y * 18) + 07] = _for [x, y + 1];
					verts [(y * 18) + 08] = y + 1;
					
					verts [(y * 18) + 09] = x;
					verts [(y * 18) + 10] = _for [x, y + 1];
					verts [(y * 18) + 11] = y + 1;
					
					verts [(y * 18) + 12] = x + 1;
					verts [(y * 18) + 13] = _for [x + 1, y];
					verts [(y * 18) + 14] = y;

					verts [(y * 18) + 15] = x + 1;
					verts [(y * 18) + 16] = _for [x + 1, y + 1];
					verts [(y * 18) + 17] = y + 1;

					for (int i = 0; i < 18; i += 3) {
						color [y * 18 + i + 0] = 0;
						color [y * 18 + i + 1] = (float)p2d [x * 0.1, y * 0.1] / 3f;
						color [y * 18 + i + 2] = 0;
					}
				}
				_rows [x] = new VertexArray(verts, color, null, verts.Length / 3,
				                            OpenTK.Graphics.OpenGL.BeginMode.Triangles);
				Console.Write("[");
				for (int i = 0; i < verts.Length / 3; i += 3) {
					Console.Write(verts [i + 0]);
					Console.Write(" ");
					Console.Write(verts [i + 1]);
					Console.Write(" ");
					Console.Write(verts [i + 2]);
					Console.Write(", ");
				}
				Console.WriteLine("]");
			}
		}

		public void Render() {
			GL.Enable(EnableCap.VertexArray);
			GL.Enable(EnableCap.ColorArray);
			for (int i = 0; i < _rows.Length; ++i) {
				_rows [i].Draw();
			}
			GL.Disable(EnableCap.VertexArray);
			GL.Enable(EnableCap.ColorArray);
		}
	}
}

