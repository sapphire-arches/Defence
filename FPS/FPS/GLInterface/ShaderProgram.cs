using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace FPS.GLInterface {
	public class ShaderProgram {
		int _programID;
		Dictionary<string, int> _locCache;


		public ShaderProgram(VertexShader VertShader, FragmentShader FragShader) {
			_programID = GL.CreateProgram();
			GL.AttachShader(_programID, VertShader.ID);
			GL.AttachShader(_programID, FragShader.ID);
			GL.LinkProgram(_programID);

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
		}
	}
}

