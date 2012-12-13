using System;
using System.Collections.Generic;
using FPS.Game.Entity;
using FPS.Game.HMap;
using OpenTK;

namespace FPS.Game
{
	public class World
	{
		public static readonly Vector3 GRAVITY = new Vector3 (0, -0.1f, 0);
		private HeightMap _terrain;
		private LinkedList<IEntity> _ents;
		private PlayerEntity _pe;
		private int _buggersKilled;

		public HeightMap Terrain {
			get { return _terrain; }
			private set { _terrain = value; }
		}

		public LinkedList<IEntity> Ents {
			get { return _ents; }
			private set { _ents = value; }
		}

		public int BuggersKilled {
			get { return _buggersKilled;}
			private set { _buggersKilled = value;}
		}

		public World (HeightMap Terrain, PlayerEntity pe)
		{
			_terrain = Terrain;
			_pe = pe;
			_ents = new LinkedList<IEntity> ();
			_ents.AddFirst (pe);
			_buggersKilled = 0;
		}

		public void Tick (float Delta)
		{
			LinkedListNode<IEntity> curr = _ents.First;
			while (curr != null) {
				IEntity ent = curr.Value;
				ent.Tick (this, Delta);
				if (ent.Dead) {
					//Remove.
					curr.List.Remove (curr);
				}

				curr = curr.Next;
			}
		}

		public void BuggerDied ()
		{
			++_buggersKilled;
		}

		public PlayerEntity GetPlayer ()
		{
			return _pe;
		}
	}
}

