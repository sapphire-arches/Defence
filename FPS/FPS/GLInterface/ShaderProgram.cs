using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace FPS.GLInterface {
	public class ShaderProgram {
		string _vertShader;
		int _vertShaderID;
		string _pixelShader;
		int _pixelShaderID;
		int _programID;
		Dictionary<string, int> _locCache;


		public ShaderProgram(string VertShaderFileName, string PixelShaderFileName) {
			using (StreamReader s = new StreamReader(VertShaderFileName)) {
				_vertShader = s.ReadToEnd();
			}
			using (StreamReader s = new StreamReader(PixelShaderFileName)) {
				_pixelShader = s.ReadToEnd();
			}

			_pixelShaderID = GL.CreateShader(ShaderType.FragmentShader);
			_vertShaderID = GL.CreateShader(ShaderType.VertexShader);
			_programID = GL.CreateProgram();

			GL.ShaderSource(_pixelShaderID, _pixelShader);
			GL.CompileShader(_pixelShaderID);
			Console.WriteLine("---- COMPILE FRAG SHADER " + _pixelShaderID + " ----");
			Console.WriteLine("ERROR: " + GL.GetError());
			Console.WriteLine(_pixelShader);
			Console.WriteLine(GL.GetShaderInfoLog(_pixelShaderID));
			Console.WriteLine("--------");

			GL.ShaderSource(_vertShaderID, _vertShader);
			GL.CompileShader(_vertShaderID);
			Console.WriteLine("---- COMPILE VERT SHADER " + _vertShaderID + " ----");
			Console.WriteLine("ERROR: " + GL.GetError());
			Console.WriteLine(_vertShader);
			Console.WriteLine(GL.GetShaderInfoLog(_vertShaderID));
			Console.WriteLine("--------");

			GL.AttachShader(_programID, _vertShaderID);
			GL.AttachShader(_programID, _pixelShaderID);
			GL.LinkProgram(_programID);
			Console.WriteLine("---- COMPILE PROG " + _programID + " ----");
			Console.WriteLine("ERROR: " + GL.GetError());
			Console.WriteLine(GL.GetProgramInfoLog(_programID));
			Console.WriteLine("--------");

			_locCache = new Dictionary<string, int>();
		}

		public void Use() {
			GL.UseProgram(_programID);
		}

		public int GetUniformLocation(string id) {
			int tr;
			if (_locCache.TryGetValue(id, out tr)) {
				return tr;
			} else {
				tr = GL.GetUniformLocation(_programID, id);
				_locCache.Add(id, tr);
				return tr;
			}
		}

		~ShaderProgram() {
			GL.DeleteProgram(_programID);
			GL.DeleteShader(_pixelShaderID);
			GL.DeleteShader(_vertShaderID);
		}
	}
}

