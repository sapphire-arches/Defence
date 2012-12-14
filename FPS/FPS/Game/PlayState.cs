using System;
using System.Collections.Generic;
using System.Drawing;
using FPS.Framework;
using FPS.Render;
using FPS.Render.Flat;
using FPS.Game;
using FPS.Game.Entity;
using FPS.Game.HMap;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace FPS.Game {
	public class PlayState : IGameState {

		bool _lmbDown;
		PlayerEntity _pe;
		World _world;
		Vector3 _camOffset;
		WorldRenderer _ren;
		LinkedList<Rect2D> _rects;
		Bitmap _healthbar;
		Rect2D _healthbarRect;
		Vector2 _mouseDelta;
		MainClass _in;

		public PlayState() {
		}

		public void Init(MainClass Main) {
			//XXX:Make this better...
			_in = Main;
			//Game world setup.
			HeightMap map = new HeightMap("res/map.map");
			_pe = new PlayerEntity(new Vector3(0, map [0, 0] + 10, 0));
			_world = new World(map, _pe);
			//GFX setup
			_camOffset = new Vector3();
			_ren = new WorldRenderer(Main, _world, (float)Main.Width / Main.Height);
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
			//Input setup.
			System.Windows.Forms.Cursor.Hide();
			Main.Mouse.ButtonDown += ButtonDown;
			Main.Mouse.ButtonUp += ButtonUp;
			Main.Mouse.Move += MouseMove;
			//2D draw setup
			_rects = new LinkedList<Rect2D>();
			_healthbar = new Bitmap(100, 50);
			_healthbarRect = new Rect2D(_healthbar, 0, 0, 100, 50);
			_rects.AddFirst(_healthbarRect);
			//INput setup
			_in.MouseCaptureNeeded = true;
			_in.CaptureMouse = true;
		}

		public void Tick(MainClass Win, double Delta) {
			if (Win.Keyboard [Key.Escape]) {
				Win.MouseCaptureNeeded = false;
				Win.CaptureMouse = false;
			}

			if (Win.Keyboard [Key.J]) {
				Win.MouseCaptureNeeded = true;
				Win.CaptureMouse = true;
			}

			_pe.Move(Win.Keyboard, _mouseDelta);
			if (_in.CaptureMouse)
				System.Windows.Forms.Cursor.Position = new Point(
					_in.X + (_in.Width / 2),
					_in.Y + (_in.Height / 2)
				);
			if (_lmbDown) {
				_pe.SwingSword(_world);
			}

			_world.Tick((float)Delta);

			_camOffset.Y = _pe.GetEyeOffset();

			_ren.Pos = Vector3.Add(_pe.Pos, _camOffset);
			_ren.Pitch = _pe.Pitch;
			_ren.Yaw = _pe.Yaw;

			foreach (IEntity ent in _world.Ents) {
				Enemy e = ent as Enemy;
				if (e != null) {
					e.AI(_world);
				}
			}

			if (!_pe.Dead) {
				//Update healthbar bitmap.
				Graphics g = Graphics.FromImage(_healthbar);
				g.FillRectangle(Brushes.Gray, 0, 0, 100, 50);
				int hbw = (int)((_pe.Health / 10.0) * 98);
				g.FillRectangle(Brushes.Green, 1, 1, hbw, 23);
				g.FillRectangle(Brushes.Red, 1 + hbw, 1, 98 - hbw, 23);

				g.DrawString(String.Format("Killed {0} buggers.", _world.BuggersKilled), SystemFonts.DefaultFont, Brushes.Black, 0, 30);
				g.Dispose();
				_healthbarRect.SetTexture(_healthbar);
			}
		}

		public void Render(MainClass Win) {
			GL.Clear(ClearBufferMask.DepthBufferBit);
			_ren.Render();
			_ren.Draw2DRects(_rects);
		}

		public bool Switch() {
			return _pe.Dead;
		}

		public IGameState LeaveTo(MainClass Win) {
			Win.Mouse.ButtonDown -= ButtonDown;
			Win.Mouse.ButtonUp -= ButtonUp;
			return null;
		}

		void ButtonUp(object Sender, MouseButtonEventArgs Args) {
			if (Args.Button == OpenTK.Input.MouseButton.Left)
				_lmbDown = false;
		}

		void ButtonDown(object Sender, MouseButtonEventArgs Args) {
			if (Args.Button == OpenTK.Input.MouseButton.Left)
				_lmbDown = true;
		}

		void MouseMove(object Sender, MouseMoveEventArgs Args) {
			if (_in.CaptureMouse) {
				_mouseDelta.X = Args.X - (_in.Width / 2);
				_mouseDelta.Y = Args.Y - (_in.Height / 2);
			}
		}
	}
}