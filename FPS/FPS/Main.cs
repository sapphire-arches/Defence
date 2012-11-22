using System;
using System.Drawing;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using FPS.GLInterface;
using FPS.Game;
using FPS.Render;

namespace FPS {
	public class MainClass  : GameWindow {
		World _world;
		HeightMap _map;
		WorldRenderer _ren;
		Vector2 _mouse_delta;
		Stopwatch _timer;
		int _frame;

		public MainClass() : base(800, 600, OpenTK.Graphics.GraphicsMode.Default, "Defend Rome") {
		}

		protected override void OnLoad(EventArgs e) {
			long frequency = Stopwatch.Frequency;
			_map = new HeightMap("res/map.map");
			_world = new World(_map);
			_ren = new WorldRenderer(_world, (float)Width / Height);
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
			GL.Fog(FogParameter.FogEnd, 20f);
			GL.Fog(FogParameter.FogStart, 10f);

			_mouse_delta = new Vector2(0, 0);
			_mouse_delta += new Vector2(
				System.Windows.Forms.Cursor.Position.X - Width / 2,
				System.Windows.Forms.Cursor.Position.Y - Height / 2);
			System.Windows.Forms.Cursor.Position = new Point(Width / 2, Height / 2);
			System.Windows.Forms.Cursor.Hide();
			_timer = new Stopwatch();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			_mouse_delta += new Vector2(
				System.Windows.Forms.Cursor.Position.X - Width / 2,
				System.Windows.Forms.Cursor.Position.Y - Height / 2);
			System.Windows.Forms.Cursor.Position = new Point(Width / 2, Height / 2);

			_ren.Yaw = _mouse_delta.X * 0.005f;
			_ren.Pitch = _mouse_delta.Y * 0.005f;

			Vector3 pos = _ren.Pos;

			if (Keyboard [Key.W]) {
				pos.Z -= (float)Math.Cos(_ren.Yaw) * 0.2f;
				pos.X -= (float)Math.Sin(_ren.Yaw) * 0.2f;
			}
			if (Keyboard [Key.S]) {
				pos.Z += (float)Math.Cos(_ren.Yaw) * 0.2f;
				pos.X += (float)Math.Sin(_ren.Yaw) * 0.2f;
			}

			if (Keyboard [Key.Q]) {
				pos.Y -= 0.1f;
				_ren.Pos = pos;
			}
			if (Keyboard [Key.E]) {
				pos.Y += 0.1f;
			}
			_ren.Pos = pos;
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			_timer.Start();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			_ren.Render();
			GLUtil.PrintGLError();
			SwapBuffers();
			++_frame;
			_timer.Stop();
			if (_frame % 600 == 59) {
				Console.WriteLine(_timer.ElapsedMilliseconds);
				_timer.Reset();
			}
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, Width, Height);
			_ren.Aspect = (float)Width / Height;
		}

		public static void Main(String[] args) {
			new MainClass().Run(20, 60);
		}
	}
}

