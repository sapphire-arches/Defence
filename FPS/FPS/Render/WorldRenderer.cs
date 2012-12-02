using System;
using System.Collections.Generic;
using FPS.Game;
using FPS.Game.Entity;
using FPS.GLInterface;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FPS.Render {
	public class WorldRenderer {
		public const float MAX_DEPTH = 128f;
		public const float FOV = (float)(0.5 * Math.PI); //90 degrees
		const float HALFPI = (float)(0.5 * Math.PI);
		public const float MAX_PITCH = HALFPI;
		public const float MIN_PITCH = HALFPI * -0.1f;

		World _for;
		HeightmapRenderer _hmap;
		ShaderProgram _simple;
		ShaderProgram _underwater;
		ShaderProgram _water;
		ShaderProgram _curr;
		Matrix4 _projectionMatrix;
		Matrix4 _modelview;
		float _aspect;
		float _pitch;
		float _yaw;
		Vector3 _pos;
		Stack<Matrix4> _mviewstack;
		MainClass _in;

		public float Pitch {
			get { return _pitch; }
			set { 
				_pitch = value;
				if (_pitch < MIN_PITCH)
					_pitch = MIN_PITCH;
				if (_pitch > MAX_PITCH)
					_pitch = MAX_PITCH;
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
				_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)(FOV), _aspect, 0.01f, MAX_DEPTH);
			}
		}

		public WorldRenderer(MainClass In, World For, float Aspect) {
			_for = For;
			_hmap = new HeightmapRenderer(this, _for.Terrain);
			FragmentShader fsimple = new FragmentShader("res/shader/simple.frag");
			FragmentShader fwater = new FragmentShader("res/shader/water.frag");
			FragmentShader fuwater = new FragmentShader("res/shader/underwater.frag");

			_simple = new ShaderProgram();
			_simple.AddFragShader(fsimple);
			GLUtil.PrintGLError("Simple");

			_underwater = new ShaderProgram();
			_underwater.AddFragShader(fuwater);
			GLUtil.PrintGLError("Underwater");

			_water = new ShaderProgram();
			_water.AddFragShader(fwater);
			GLUtil.PrintGLError("Water");

			_curr = _simple;

			_aspect = Aspect;
			_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)(FOV), _aspect, 0.01f, MAX_DEPTH);
			_modelview = Matrix4.CreateTranslation(-10f, -5f, -10f);
			_pos = new Vector3(10, 2, 10);
			_mviewstack = new Stack<Matrix4>();
			_in = In;
		}

		public void Render() {
			GLUtil.PrintGLError("Prerender");
			if (_pos.Y > 0) {
				_simple.Use();
			} else {
				_underwater.Use();
			}
			_modelview = Matrix4.Identity;
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateTranslation(-_pos.X, -_pos.Y, -_pos.Z));
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateFromAxisAngle(Vector3.UnitY, -_yaw));
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateFromAxisAngle(Vector3.UnitX, -_pitch));

			LoadMatricies();
			_hmap.Render(_pos.X, _pos.Z);

			PushMatrix();
			foreach (IEntity ent in _for.Ents) {
				ent.Render(this);
			}
			PopMatrix();

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			_water.Use();
			LoadMatricies();
			_hmap.RenderWater(_pos.X, _pos.Z);
			GL.Disable(EnableCap.Blend);
		}

		public int GetFrame() {
			return _in.GetFrame();
		}

		public void PushMatrix() {
			_mviewstack.Push(_modelview);
		}

		public void PopMatrix() {
			_modelview = _mviewstack.Pop();
			int modelviewLoc = _curr.GetUniformLocation("modelview");
			GL.UniformMatrix4(modelviewLoc, false, ref _modelview);
		}

		public void LoadIdent() {
			_modelview = Matrix4.Identity;
			int modelviewLoc = _curr.GetUniformLocation("modelview");
			GL.UniformMatrix4(modelviewLoc, false, ref _modelview);
		}

		public void Translate(float X, float Y, float Z) {
			_modelview = Matrix4.Mult(Matrix4.CreateTranslation(X, Y, Z), _modelview);
			int modelviewLoc = _curr.GetUniformLocation("modelview");
			GL.UniformMatrix4(modelviewLoc, false, ref _modelview);
		}

		public void Rotate(Vector3 Axis, float Angle) {
			_modelview = Matrix4.Mult(Matrix4.CreateFromAxisAngle(Axis, Angle), _modelview);
			int modelviewLoc = _curr.GetUniformLocation("modelview");
			GL.UniformMatrix4(modelviewLoc, false, ref _modelview);
		}

		public void BindTexture(int id) {
			int texloc = _curr.GetUniformLocation("tex");
			GL.Uniform1(texloc, 0);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, id);
		}

		void LoadMatricies() {
			int projectionLoc = _curr.GetUniformLocation("projection");
			int modelviewLoc = _curr.GetUniformLocation("modelview");
			GL.UniformMatrix4(projectionLoc, false, ref _projectionMatrix);
			GL.UniformMatrix4(modelviewLoc, false, ref _modelview);
			GLUtil.PrintGLError("Matricies");
		}
	}
}

