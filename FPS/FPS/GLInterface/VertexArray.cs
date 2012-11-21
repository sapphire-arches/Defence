using System;
using OpenTK.Graphics.OpenGL;

namespace FPS.GLInterface {
	public class VertexArray {
		private float[] _verts;
		private int _num;
		private float[] _colors;
		private float[] _texcoords;
		private BeginMode _mode;

		public VertexArray(float[] Verts, float[] Colors, float[] Texcoords, int Num, BeginMode Mode) {
			_verts = Verts;
			_colors = Colors;
			_texcoords = Texcoords;
			_num = Num;
			_mode = Mode;
		}

		public void Draw() {
			GL.VertexPointer(3, VertexPointerType.Float, 0, _verts);
			if (_colors != null) {
				GL.ColorPointer(3, ColorPointerType.Float, 0, _colors);
			}
			if (_texcoords != null) {
				GL.TexCoordPointer(3, TexCoordPointerType.Float, 0, _texcoords);
			}
			GL.DrawArrays(_mode, 0, _num);
		}
	}
}