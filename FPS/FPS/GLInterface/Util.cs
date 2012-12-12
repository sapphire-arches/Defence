using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace FPS.GLInterface {
	public class GLUtil {
		private GLUtil() {
		}

		public static void PrintGLError(string Pre) {
			ErrorCode err = GL.GetError();
			if (err != ErrorCode.NoError) {
				Console.WriteLine(Pre + ": " + err);
			}
		}

		public static int CreateTexture(Bitmap Texture) {
			BitmapData data = Texture.LockBits(
				new Rectangle(0, 0, Texture.Width, Texture.Height),
				ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int texid = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texid);
			GL.TexImage2D(TextureTarget.Texture2D, 0,
			              PixelInternalFormat.Rgba, data.Width, data.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			Texture.UnlockBits(data);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			return texid;
		}

		public static void UpdateTexture(Bitmap To, int Texture) {
			BitmapData data = To.LockBits(
				new Rectangle(0, 0, To.Width, To.Height),
				ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int texid = Texture;
			GL.BindTexture(TextureTarget.Texture2D, texid);
			GL.TexImage2D(TextureTarget.Texture2D, 0,
			              PixelInternalFormat.Rgba, data.Width, data.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			To.UnlockBits(data);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		}
	}
}

