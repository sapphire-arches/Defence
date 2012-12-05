using System;
using OpenTK;
using FPS.Render;
using FPS.GLInterface;

namespace FPS.Game.Entity { 
	public class Enemy : IEntity {
		static Model _mdl;

		public Enemy(Vector3 Pos) : base(Pos, new AABB(1, 2, 1)) {
			if (_mdl == null) {
				_mdl = OBJModelParser.GetInstance().Parse("res/mdl/enemy");
			}
		}

		public void AI(World W) {
			IEntity pe = W.Ents.First.Value;//W.GetPlayer();
			if (pe == this) {
				pe = W.Ents.First.Next.Value;
			}

			Vector3 delta = Vector3.Multiply(Vector3.Normalize(Vector3.Subtract(pe.Pos, _pos)), PlayerEntity.MOVE_SPEED * 0.9f);
			ApplyForce(delta);
		}

		public override void Render(WorldRenderer WR) {
			WR.PushMatrix();
			WR.Translate(_pos.X, _pos.Y, _pos.Z);
			_mdl.Render(WR);
			WR.PopMatrix();
		}
	}
}