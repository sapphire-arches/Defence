using System;
using FPS;
using OpenTK;

namespace FPS.Framework {
	public interface IGameState {
		void Init(MainClass Main);
		void Tick(MainClass Win, double Delta);
		void Render(MainClass Win);
		bool Switch();
		IGameState LeaveTo(MainClass Win);
	}
}

