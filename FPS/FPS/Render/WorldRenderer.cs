using System;
using System.Collections.Generic;
using System.Drawing;
using FPS.Util;
using FPS.Game;
using FPS.Game.Entity;
using FPS.GLInterface;
using FPS.Render.Flat;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FPS.Render {
	public class WorldRenderer {
		public const float MAX_DEPTH = 128f;
		public const float FOV = (float)(0.5 * Math.PI); //90 degrees
		const float HALFPI = (float)(0.5 * Math.PI);
		public const float MAX_PITCH = HALFPI * 1f;
		public const float MIN_PITCH = HALFPI * -1f;

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
		int _oldwidth, _oldheight;

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
			_oldwidth = 0;
			_oldheight = 0;
		}

		public void Render() {
			if (_oldwidth != _in.Width || _oldheight != _in.Height) {
				Aspect = (float)_in.Width / _in.Height;
				_oldwidth = _in.Width;
				_oldheight = _in.Height;
			}

			GLUtil.PrintGLError("Prerender");
			if (_pos.Y > 0) {
				SetFogAndClear(OpenTK.Graphics.Color4.SkyBlue);
				_simple.Use();
			} else {
				SetFogAndClear(OpenTK.Graphics.Color4.DeepSkyBlue);
				_underwater.Use();
			}
			SetHighlight(1);
			_modelview = Matrix4.Identity;
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateTranslation(-_pos.X, -_pos.Y, -_pos.Z));
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateFromAxisAngle(Vector3.UnitY, _yaw));
			_modelview = Matrix4.Mult(_modelview, Matrix4.CreateFromAxisAngle(Vector3.UnitX, _pitch));

			LoadMatricies();
			_hmap.Render(this, _pos.X, _pos.Z);

			PushMatrix();
			foreach (IEntity ent in _for.Ents) {
				ent.Render(this);
			}
			PopMatrix();

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			_water.Use();
			SetHighlight(1);
			LoadMatricies();
			_hmap.RenderWater(this, _pos.X, _pos.Z);
			GL.Disable(EnableCap.Blend);
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
			GLUtil.PrintGLError("TexLoc");
			GL.Uniform1(texloc, 0);
			GLUtil.PrintGLError("TexUniform");
			GL.ActiveTexture(TextureUnit.Texture0);
			GLUtil.PrintGLError("TexActive");
			GL.BindTexture(TextureTarget.Texture2D, id);
			GLUtil.PrintGLError("TexBind");
		}

		public void SetHighlight(float HL) {
			GL.Uniform1(_curr.GetUniformLocation("highlight"), HL);
		}

		void SetFogAndClear(OpenTK.Graphics.Color4 C) {
			GL.ClearColor(OpenTK.Graphics.Color4.DeepSkyBlue);
			float[] fogColor = {
				OpenTK.Graphics.Color4.DeepSkyBlue.R,
				OpenTK.Graphics.Color4.DeepSkyBlue.G,
				OpenTK.Graphics.Color4.DeepSkyBlue.B,
				OpenTK.Graphics.Color4.DeepSkyBlue.A
			};
			GL.Fog(FogParameter.FogColor, fogColor);
			GL.Clear(ClearBufferMask.ColorBufferBit);
		}

		void LoadMatricies() {
			int projectionLoc = _curr.GetUniformLocation("projection");
			int modelviewLoc = _curr.GetUniformLocation("modelview");
			GL.UniformMatrix4(projectionLoc, false, ref _projectionMatrix);
			GL.UniformMatrix4(modelviewLoc, false, ref _modelview);
			GLUtil.PrintGLError("Matricies");
		}

		public int GetFrame() {
			return _in.Frame;
		}

		public void Draw2DRects(LinkedList<Rect2D> Rects) {
			int _sdrid;
			GL.GetInteger(GetPName.CurrentProgram, out _sdrid);
			GL.UseProgram(0);

			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.DepthTest);
			GL.MatrixMode(MatrixMode.Modelview);
			Matrix4 magic = Matrix4.CreateOrthographicOffCenter(0, _in.Width, _in.Height, 0, -1, 1);
			GL.LoadMatrix(ref magic);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			foreach (Rect2D rect in Rects) {
				rect.Render();
			}

			//XXX:Restore state
			GL.UseProgram(_sdrid);
			GL.Enable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Texture2D);
		}
	}
}

