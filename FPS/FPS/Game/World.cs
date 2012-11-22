using System;
using System.Collections.Generic;
using FPS.Game.Entity;
using OpenTK;

namespace FPS.Game {
	public class World {
		public static readonly Vector3 GRAVITY = new Vector3(0, -0.25f, 0);
		private HeightMap _terrain;
		private LinkedList<IEntity> _ents;

		public HeightMap Terrain {
			get { return _terrain; }
			private set { _terrain = value; }
		}

		public LinkedList<IEntity> Ents {
			get { return _ents; }
			private set { _ents = value; }
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
			_ents = new LinkedList<IEntity>();
		}

		public void Tick(float Delta) {
			foreach (IEntity ent in _ents) {
				ent.Tick(this, Delta);
			}
		}
	}
}
