using System;
using System.Drawing;
using System.IO;
using FPS.Render.Flat;
using OpenTK.Input;

namespace FPS.Framework {
	public class IntroState : IGameState {
		FullScreenTextRender _ren;
		bool _switch;
		IGameState _next;

		public IntroState(IGameState Next) {
			_next = Next;
		}

		public void Init(MainClass Main) {
			//Text reading and intro setup.
			string text = "";
			using (StreamReader r = new StreamReader("res/readme.txt")) {
				text = r.ReadToEnd();
			}
			_ren = new FullScreenTextRender(text, Brushes.Gold, Brushes.Indigo);
			Main.Mouse.ButtonDown += ButtonDown;
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

