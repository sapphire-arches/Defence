using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace FPS.GLInterface {
	public class ShaderProgram {
		static readonly FragmentShader BASEFRAG = new FragmentShader("res/shader/base.frag");
		static readonly VertexShader BASEVERT = new VertexShader("res/shader/base.vert");

		bool _linked;
		int _programID;
		Dictionary<string, int> _locCache;


		public ShaderProgram() {
			_programID = GL.CreateProgram();
			GL.AttachShader(_programID, BASEFRAG.ID);
			GL.AttachShader(_programID, BASEVERT.ID);

			_linked = false;
			_locCache = new Dictionary<string, int>();
		}

		public void AddFragShader(FragmentShader FS) {
			GL.AttachShader(_programID, FS.ID);
			_linked = false;
		}

		public void AddVertShader(VertexShader VS) {
			GL.AttachShader(_programID, VS.ID);
			_linked = false;
		}

		public void Use() {
			EnsureLink();
			GL.UseProgram(_programID);
		}

		public int GetUniformLocation(string ID) {
			int tr;
			if (_locCache.TryGetValue(ID, out tr)) {
				return tr;
			} else {
				EnsureLink();
				tr = GL.GetUniformLocation(_programID, ID);
				GLUtil.PrintGLError("GetUniform " + ID);
				_locCache.Add(ID, tr);
				return tr;
			}
		}

		~ShaderProgram() {
			//GL.DeleteProgram(_programID);
		}

		void EnsureLink() {
			if (!_linked) {
				GL.ValidateProgram(_programID);
				string err = GL.GetProgramInfoLog(_programID);
				if (err.Contains("err")) {
					Console.WriteLine("Compile of shader program: " + _programID + " failed.");
					Console.WriteLine(err);
				}
				GL.LinkProgram(_programID);
				_linked = true;
			}
		}
	}
}

