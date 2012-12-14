using System;
using System.Diagnostics;
using System.Drawing;
using FPS.Framework;
using OpenTK;

namespace FPS {
	public class MainClass : GameWindow {
		IGameState _curr;
		Stopwatch _timer;
		Vector2 _mouseDelta;

		public bool CaptureMouse {
			get;
			set;
		}

		public bool MouseCaptureNeeded {
			get;
			set;
		}

		public int Frame {
			get;
			private set;
		}

		public MainClass() : base(800, 600, OpenTK.Graphics.GraphicsMode.Default, "Defend Rome") {
		}

		protected override void OnLoad(EventArgs args) {
			_curr = new IntroState(new FPS.Game.PlayState());
			_curr.Init(this);
			//Misc
			_timer = new Stopwatch();
		}

		protected override void OnFocusedChanged(EventArgs e) {
			CaptureMouse = Focused && MouseCaptureNeeded;
			if (CaptureMouse) {
				System.Windows.Forms.Cursor.Hide();
			}
		}

		protected override void OnUpdateFrame(FrameEventArgs args) {
			_curr.Tick(this, 1);
			if (_curr.Switch()) {
				_curr = _curr.LeaveTo(this);
				_curr.Init(this);
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			_timer.Start();
			_curr.Render(this);
			FPS.GLInterface.GLUtil.PrintGLError("Main");
			++Frame;
			_timer.Stop();
			SwapBuffers();
			if (Frame % 60 == 59) {
				Console.WriteLine((float)Stopwatch.Frequency / (_timer.ElapsedTicks / 60f));
				_timer.Reset();
			}
		}

		protected override void OnResize(EventArgs e) {
			OpenTK.Graphics.OpenGL.GL.Viewport(0, 0, Width, Height);
		}

		public static void Main(String[] args) {
			new MainClass().Run(20, 60);
		}
	}
}