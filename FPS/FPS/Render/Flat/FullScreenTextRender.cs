using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using FPS.GLInterface;

namespace FPS.Render.Flat {
	public class FullScreenTextRender {
		Bitmap _2d;
		int _2dTex;
		int _2dvbo;
		Rectangle _viewport;
		Brush _bg, _fg;
		string _text;

		public FullScreenTextRender(string Text, Brush Background, Brush Foreground) {
			_2d = new Bitmap(1, 1);
			_viewport = new Rectangle(0, 0, 1, 1);
			_2dTex = GLUtil.CreateTexture(_2d);
			_2dvbo = GLUtil.BuildRectangle(0, 0, 1, 1);
			_bg = Background;
			_fg = Foreground;
			_text = Text;
		}

		public void Render(MainClass Main) {
			if (_viewport.Width != Main.Width || _viewport.Height != Main.Height) {
				_2d = new Bitmap(Main.Width, Main.Height);
				Graphics g = Graphics.FromImage(_2d);
				g.FillRectangle(_bg, 0, 0, _2d.Width, _2d.Height);
				g.DrawString(_text, SystemFonts.DefaultFont, _fg, 0, 0);
				g.Dispose();
				GLUtil.UpdateTexture(_2d, _2dTex);
				_viewport.Width = Main.Width;
				_viewport.Height = Main.Height;
			}
			GL.UseProgram(0);
			GL.Enable(EnableCap.Texture2D);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 orth = Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, -1, 1);
			GL.LoadMatrix(ref orth);
			GL.BindTexture(TextureTarget.Texture2D, _2dTex);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _2dvbo);
			GL.InterleavedArrays(InterleavedArrayFormat.T2fC4fN3fV3f, 0, (IntPtr)0);
			GL.DrawArrays(BeginMode.Quads, 0, 4);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.Disable(EnableCap.Texture2D);
		}
	}
}