using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
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
				System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
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
				System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			int texid = Texture;
			GL.BindTexture(TextureTarget.Texture2D, texid);
			GL.TexImage2D(TextureTarget.Texture2D, 0,
			              PixelInternalFormat.Rgba, data.Width, data.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			To.UnlockBits(data);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		}

		public static int BuildRectangle(float X, float Y, float Width, float Height) {
			Vertex[] d = new Vertex[4];
			for (int i = 0; i < d.Length; ++i) {
				d [i] = new Vertex();
				d [i].Color = new Vector4(1, 1, 1, 1);
			}
			d [0].Position.X = X;
			d [0].Position.Y = Y;
			d [0].TexCoord.X = 0;
			d [0].TexCoord.Y = 0;
			
			d [1].Position.X = X;
			d [1].Position.Y = Y + Height;
			d [1].TexCoord.X = 0;
			d [1].TexCoord.Y = 1;
			
			d [2].Position.X = X + Width;
			d [2].Position.Y = Y + Height;
			d [2].TexCoord.X = 1;
			d [2].TexCoord.Y = 1;
			
			d [3].Position.X = X + Width;
			d [3].Position.Y = Y;
			d [3].TexCoord.X = 1;
			d [3].TexCoord.Y = 0;

			int tr = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, tr);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(d.Length * Vertex.Size), d, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			return tr;
		}
	}
}

