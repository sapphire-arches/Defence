using System;
using System.Collections.Generic;
using FPS.Game.Entity;
using FPS.Game.HMap;
using OpenTK;

namespace FPS.Game {
	public class World {
		public static readonly Vector3 GRAVITY = new Vector3(0, -0.1f, 0);
		private HeightMap _terrain;
		private LinkedList<IEntity> _ents;
		private PlayerEntity _pe;

		public HeightMap Terrain {
			get { return _terrain; }
			private set { _terrain = value; }
		}

		public LinkedList<IEntity> Ents {
			get { return _ents; }
			private set { _ents = value; }
		}

		public World(HeightMap Terrain, PlayerEntity pe) {
			_terrain = Terrain;
			_pe = pe;
			_ents = new LinkedList<IEntity>();
			_ents.AddFirst(pe);
			_ents.AddFirst(new Enemy(new Vector3(10, Terrain [0, 0], 10)));
			_ents.AddFirst(new Enemy(new Vector3(10, Terrain [0, 0], -10.1f)));
		}

		public void Tick(float Delta) {
			foreach (IEntity ent in _ents) {
				ent.Tick(this, Delta);
			}
		}

		public PlayerEntity GetPlayer() {
			return _pe;
		}
	}
}

