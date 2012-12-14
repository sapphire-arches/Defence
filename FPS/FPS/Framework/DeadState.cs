using System;
using System.Drawing;
using System.IO;
using FPS;
using FPS.Render.Flat;
using OpenTK.Input;

namespace FPS.Framework {
	public class DeadState : IGameState {
		FullScreenTextRender _ren;
		bool _switch;
		IGameState _next;

		public DeadState(IGameState Next) {
			_next = Next;
		}

		public void Init(MainClass Win) {
			//Text reading and intro setup.
			string text = "";
			using (StreamReader r = new StreamReader("res/dead.txt")) {
				text = r.ReadToEnd();
			}
			_ren = new FullScreenTextRender(text, Brushes.Yellow, Brushes.Purple);
			Win.Mouse.ButtonDown += ButtonDown;
		}
		
		public void Tick(MainClass Win, double Delta) {

		}

		public void Render(MainClass Win) {
			_ren.Render(Win);
		}

		public bool Switch() {
			return _switch;
		}

		public IGameState LeaveTo(MainClass Win) {
			Win.Mouse.ButtonDown -= ButtonDown;
			return _next;
		}

		void ButtonDown(object Sender, MouseButtonEventArgs Args) {
			_switch = true;
		}
	}
}

