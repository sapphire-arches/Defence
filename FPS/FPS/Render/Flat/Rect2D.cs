using System;
using System.Drawing;
using FPS.GLInterface;
using OpenTK.Graphics.OpenGL;

namespace FPS.Render.Flat {
	public class Rect2D {
		int _vboid;
		int _texid;

		public Rect2D(Bitmap Tex, float X, float Y, float Width, float Height) {
			_vboid = GLUtil.BuildRectangle(X, Y, Width, Height);
			_texid = GLUtil.CreateTexture(Tex);
		}

		public void SetTexture(Bitmap Tex) {
			GLUtil.UpdateTexture(Tex, _texid);
		}

		public void Render() {
			GL.BindTexture(TextureTarget.Texture2D, _texid);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vboid);
			GL.InterleavedArrays(InterleavedArrayFormat.T2fC4fN3fV3f, 0, (IntPtr)0);
			GL.DrawArrays(BeginMode.Quads, 0, 4);
		}
	}
}

