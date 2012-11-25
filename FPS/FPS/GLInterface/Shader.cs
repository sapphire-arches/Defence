using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace FPS.GLInterface {
	public class Shader {
		int _id;

		public int ID {
			get { return _id;}
			set { ;}
		}

		public Shader(string FileName, ShaderType SType) {
			string src;
			using (StreamReader s = new StreamReader(FileName)) {
				src = s.ReadToEnd();
			}
			_id = GL.CreateShader(SType);
			GL.ShaderSource(_id, src);
			GL.CompileShader(_id);
			string err = GL.GetShaderInfoLog(_id);
			if (err.Contains("error")) {
				Console.WriteLine("Compile of shader: " + _id + " failed.");
				Console.WriteLine(src);
				Console.WriteLine(err);
			}
		}

		~Shader() {
			GL.DeleteShader(_id);
		}
	}

	public class VertexShader : Shader {
		public VertexShader(string FileName) : base (FileName, ShaderType.VertexShader) {
		}
	}
	public class GeometryShader : Shader {
		public GeometryShader(string FileName) : base (FileName, ShaderType.GeometryShader) {
		}
	}
	public class FragmentShader : Shader {
		public FragmentShader(string FileName) : base (FileName, ShaderType.FragmentShader) {
		}
	}
}