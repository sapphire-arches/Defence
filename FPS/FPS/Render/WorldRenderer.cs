using System;
using FPS.Game;
using FPS.Game.Entity;
using FPS.GLInterface;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FPS.Render {
	public class WorldRenderer {
		public static readonly float MAX_DEPTH = 128f;

		World _for;
		HeightmapRenderer _hmap;
		ShaderProgram _simple;
		ShaderProgram _underwater;
		int _projectionLoc;
		Matrix4 _projectionMatrix;
		int _modelviewLoc;
		Matrix4 _modelview;
		float _aspect;
		float _pitch;
		float _yaw;
		Vector3 _pos;

		public float Pitch {
			get { return _pitch; }
			set { 
				_pitch = value;
				if (_pitch < -(0.5 * Math.PI))
					_pitch = -(float)Math.PI * 0.5f;
				if (_pitch > 0.5 * Math.PI)
					_pitch = (float)Math.PI * 0.5f;
			}
		}

		public float Yaw {
			get { return _yaw; }
			set {
				_yaw = value;
				while (_yaw > 2 * Math.PI) {
					_yaw -= (float)(2 * Math.PI);
				}
				while (_yaw < 0) {
					_yaw += (float)(2 * Math.PI);
				}
			}
		}

		public Vector3 Pos {
			get { return _pos; }
			set { _pos = value; }
		}

		public float Aspect {
			get { return _aspect; }
			set { 
				_aspect = value;
				_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)(0.5 * _aspect * Math.PI), _aspect, 0.01f, MAX_DEPTH);
			}
		}

		public WorldRenderer(World For, float Aspect) {
			_for = For;
			_hmap = new HeightmapRenderer(_for.Terrain);
			_simple = new ShaderProgram("res/base.vert", "res/base.frag");
			_underwater = new ShaderProgram("res/base.vert", "res/water.frag");
			_projectionLoc = _simple.GetUniformLocation("projection");
			_modelviewLoc = _simple.GetUniformLocation("modelview");
			_aspect = Aspect;
			_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)(0.25 * _aspect * Math.PI), _aspect, 0.01f, MAX_DEPTH);
			_modelview = Matrix4.CreateTranslation(-10f, -5f, -10f);
			_pos = new Vector3(10, 2, 10);
		}

		public void Render() {
			if (_pos.Y > 0)
				_simple.Use();
			else
				_underwater.Use();
			_modelview = Matrix4.Identity;
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateTranslation(-_pos.X, -_pos.Y, -_pos.Z));
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateFromAxisAngle(Vector3.UnitY, -_yaw));
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateFromAxisAngle(Vector3.UnitX, -_pitch));
			GL.UniformMatrix4(_projectionLoc, false, ref _projectionMatrix);
			GL.UniformMatrix4(_modelviewLoc, false, ref _modelview);
			_hmap.Render(_pos.X, _pos.Z);
			foreach (IEntity ent in _for.Ents) {
				ent.Render();
			}
		}
	}
}

