using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using FPS.GLInterface;
using FPS.Game;
using FPS.Game.Entity;
using FPS.Game.HMap;
using FPS.Render;

namespace FPS {
	public class MainClass  : GameWindow {
		Vector3 _camOffset;
		World _world;
		HeightMap _map;
		WorldRenderer _ren;
		Vector2 _mouseDelta;
		Stopwatch _timer;
		int _frame;
		int _maxSpawn;
		Random _spawn;
		PlayerEntity _pe;
		bool _capMouse;
		bool _lmbDown;
		string _introText;
		bool _drawIntro;

		public MainClass() : base(800, 600, OpenTK.Graphics.GraphicsMode.Default, "Defend Rome") {
		}

		protected override void OnLoad(EventArgs args) {
			_map = new HeightMap("res/map.map");
			_pe = new PlayerEntity(new Vector3(0, 20, 0));
			_world = new World(_map, _pe);
			_ren = new WorldRenderer(this, _world, (float)Width / Height);
			using (StreamReader r = new StreamReader("res/readme.txt")) {
				_introText = r.ReadToEnd();
			}
			_drawIntro = true;
			_camOffset = new Vector3();
			GL.ClearColor(OpenTK.Graphics.Color4.SkyBlue);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Fog);
			float[] fogColor = {
				OpenTK.Graphics.Color4.SkyBlue.R,
				OpenTK.Graphics.Color4.SkyBlue.G,
				OpenTK.Graphics.Color4.SkyBlue.B,
				OpenTK.Graphics.Color4.SkyBlue.A
			};
			GL.Fog(FogParameter.FogColor, fogColor);
			GL.Fog(FogParameter.FogEnd, WorldRenderer.MAX_DEPTH);
			GL.Fog(FogParameter.FogStart, 10f);

			_mouseDelta = new Vector2(0, 0);
			_mouseDelta += new Vector2(
				System.Windows.Forms.Cursor.Position.X - Width / 2 - X,
				System.Windows.Forms.Cursor.Position.Y - Height / 2 - Y);
			System.Windows.Forms.Cursor.Position = new Point(Width / 2 + X, Height / 2 + Y);
			System.Windows.Forms.Cursor.Hide();

			_timer = new Stopwatch();
			_capMouse = true;
			Mouse.ButtonDown += delegate(object sender, MouseButtonEventArgs e) {
				if (e.Button == OpenTK.Input.MouseButton.Left)
					_lmbDown = true;
				_drawIntro = false;
				_introText += "asdf";
			};
			Mouse.ButtonUp += delegate(object sender, MouseButtonEventArgs e) {
				if (e.Button == OpenTK.Input.MouseButton.Left)
					_lmbDown = false;
			};
			_spawn = new Random();
			_maxSpawn = 4;
		}

		protected override void OnUpdateFrame(FrameEventArgs args) {
			if (Keyboard [Key.Escape]) {
				_capMouse = false;
			}

			if (Keyboard [Key.J]) {
				_capMouse = true;
			}

			if (_capMouse) {
				_mouseDelta.X = System.Windows.Forms.Cursor.Position.X - Width / 2 - X;
				_mouseDelta.Y = System.Windows.Forms.Cursor.Position.Y - Height / 2 - Y;
				System.Windows.Forms.Cursor.Position = new Point(Width / 2 + X, Height / 2 + Y);
			}

			_pe.Move(Keyboard, _mouseDelta);
			if (_lmbDown) {
				_pe.SwingSword();
			}

			_world.Tick(1);

			_camOffset.Y = _pe.GetEyeOffset();

			_ren.Pos = Vector3.Add(_pe.Pos, _camOffset);
			_ren.Pitch = _pe.Pitch;
			_ren.Yaw = _pe.Yaw;

			double d = _spawn.NextDouble();
			if (_world.Ents.Count < _maxSpawn && d < 0.1) {
				float x, y, z;
				float R = FPS.Game.HMap.IslandGenerator.ISLAND_RADIUS;
				x = (float)(2 * _spawn.NextDouble() * R - R);
				y = (float)(2 * _spawn.NextDouble() * R - R);
				z = (float)(2 * _spawn.NextDouble() * R - R);

				IEntity tmp = new Enemy(new Vector3(x, y, z));
				_world.Ents.AddFirst(tmp);
				Console.WriteLine("Spawned");
				if (_maxSpawn < 20)
					_maxSpawn += 1;
			}

			foreach (IEntity ent in _world.Ents) {
				Enemy e = ent as Enemy;
				if (e != null) {
					e.AI(_world);
				}
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			_timer.Start();
			GL.Clear(ClearBufferMask.DepthBufferBit);
			if (_drawIntro) {
				Graphics g = _ren.Get2DDrawGraphics();
				g.FillRectangle(Brushes.Gold, 0, 0, Width, Height);
				g.DrawString(_introText, SystemFonts.DefaultFont, Brushes.Indigo, 0, 0);
				g.Dispose();
			}
			_ren.Render();
			if (_drawIntro) {
				_ren.Do2DDraw();
			}
			GLUtil.PrintGLError("Main");
			++_frame;
			_timer.Stop();
			SwapBuffers();
			if (_frame % 60 == 59) {
				Console.WriteLine((float)Stopwatch.Frequency / (_timer.ElapsedTicks / 60f));
				_timer.Reset();
			}
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, Width, Height);
			_ren.Aspect = (float)Width / Height;
		}

		public int GetFrame() {
			return _frame;
		}

		public static void Main(String[] args) {
			new MainClass().Run(20, 60);
		}
	}
}

