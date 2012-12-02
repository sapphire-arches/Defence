using System;
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
	}
}

