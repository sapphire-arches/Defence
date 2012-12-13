using System;
using System.Collections.Generic;
using System.Drawing;
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
		Bitmap _2d;
		int _2dTex;
		int _2dvbo;
		bool _2dchanged;
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
			_2d = new Bitmap(_in.Width, _in.Height);
			_2dTex = GLUtil.CreateTexture(_2d);
			_oldwidth = 0;
			_oldheight = 0;
			_2dvbo = GL.GenBuffer();
			Vertex[] d = BuildSquare();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _2dvbo);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(d.Length * Vertex.Size), d, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void Render() {
			if (_oldwidth != _in.Width || _oldheight != _in.Height) {
				_2d = new Bitmap(_in.Width, _in.Height);
				_oldwidth = _in.Width;
				_oldheight = _in.Height;
				_2dchanged = true;
			}

			GLUtil.PrintGLError("Prerender");
			if (_pos.Y > 0) {
				SetFogAndClear(OpenTK.Graphics.Color4.SkyBlue);
				_simple.Use();
			} else {
				SetFogAndClear(OpenTK.Graphics.Color4.DeepSkyBlue);
				_underwater.Use();
			}
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
			LoadMatricies();
			_hmap.RenderWater(this, _pos.X, _pos.Z);
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
			GLUtil.PrintGLError("TexLoc");
			GL.Uniform1(texloc, 0);
			GLUtil.PrintGLError("TexUniform");
			GL.ActiveTexture(TextureUnit.Texture0);
			GLUtil.PrintGLError("TexActive");
			GL.BindTexture(TextureTarget.Texture2D, id);
			GLUtil.PrintGLError("TexBind");
		}

		public Graphics Get2DDrawGraphics() {
			_2dchanged = true;
			return Graphics.FromImage(_2d);
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

		public void Do2DDraw() {
			//Console.WriteLine(_2d.GetPixel(400, 300));
			int _sdrid;
			GL.GetInteger(GetPName.CurrentProgram, out _sdrid);
			GL.UseProgram(0);

			if (_2dchanged) {
				GLUtil.UpdateTexture(_2d, _2dTex);
				_2dchanged = false;
			}

			GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.DepthTest);
			GL.MatrixMode(MatrixMode.Modelview);
			Matrix4 magic = Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, -1, 1);
			GL.LoadMatrix(ref magic);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.BindTexture(TextureTarget.Texture2D, _2dTex);
			GL.Enable(EnableCap.Texture2D);
			
			GL.BindBuffer(BufferTarget.ArrayBuffer, _2dvbo);
			GL.InterleavedArrays(InterleavedArrayFormat.T2fC4fN3fV3f, 0, (IntPtr)0);
			GL.DrawArrays(BeginMode.Quads, 0, 4);

			//XXX:Restore state
			GL.UseProgram(_sdrid);
			GL.Enable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Blend);
		}

		Vertex[] BuildSquare() {
			Vertex[] tr = new Vertex[4];
			for (int i = 0; i < tr.Length; ++i) {
				tr [i] = new Vertex();
				tr [i].Color = new Vector4(1, 1, 1, 1);
			}
			tr [0].Position.X = 0;
			tr [0].Position.Y = 0;
			tr [0].TexCoord.X = 0;
			tr [0].TexCoord.Y = 0;
			
			tr [1].Position.X = 0;
			tr [1].Position.Y = 1;
			tr [1].TexCoord.X = 0;
			tr [1].TexCoord.Y = 1;
			
			tr [2].Position.X = 1;
			tr [2].Position.Y = 1;
			tr [2].TexCoord.X = 1;
			tr [2].TexCoord.Y = 1;
			
			tr [3].Position.X = 1;
			tr [3].Position.Y = 0;
			tr [3].TexCoord.X = 1;
			tr [3].TexCoord.Y = 0;

			return tr;
		}
	}
}

