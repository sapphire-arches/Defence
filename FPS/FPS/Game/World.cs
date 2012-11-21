using System;

namespace FPS.Game {
	public class World {
		private HeightMap _terrain;

		public HeightMap Terrain {
			get { return _terrain; }
			private set { _terrain = value; }
		}

		public int Heigth {
			get { return _terrain.Height; }
			private set { ;}
		}

		public int Width {
			get { return _terrain.Width; }
			private set { ;}
		}

		public World(HeightMap Terrain) {
			_terrain = Terrain;
		}
	}
}

