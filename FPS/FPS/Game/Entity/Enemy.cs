using System;
using OpenTK;
using FPS.Util;
using FPS.Render;
using FPS.GLInterface;

namespace FPS.Game.Entity { 
	public class Enemy : IEntity {
		static Model _mdl;
		public static readonly Ellipsoid BS = new Ellipsoid(new Vector3(1, 2, 1));
		const int DEATH_ANIM_FRAMES = 20;
		const int DEATH_SHOW_TIME = DEATH_ANIM_FRAMES + 600;
		int _deathAnimFrame;
		bool _runDeath;

		public Enemy(Vector3 Pos) : base(Pos, new AABB(1, 2, 1), 10) {
			if (_mdl == null) {
				_mdl = OBJModelParser.GetInstance().Parse("res/mdl/enemy");
			}
			_deathAnimFrame = 0;
		}

		public void AI(World W) {
			if (!_runDeath) {
				IEntity pe = W.GetPlayer();

				Vector3 delta = Vector3.Multiply(Vector3.Normalize(Vector3.Subtract(pe.Pos, _pos)), PlayerEntity.MOVE_SPEED * 0.9f);
				ApplyForce(delta);
				Yaw = (float)Math.Atan2(delta.X, delta.Z);
			}
			if (Health < 0) {
				_noclip = true;
				_runDeath = true;
			}
		}

		public override void Render(WorldRenderer WR) {
			WR.PushMatrix();
			WR.Translate(_pos.X, _pos.Y, _pos.Z);
			WR.Rotate(Vector3.UnitY, Yaw);
			if (_runDeath) {
				int angleFrame = (_deathAnimFrame < DEATH_ANIM_FRAMES) ? _deathAnimFrame : DEATH_ANIM_FRAMES;
				float angle = (float)(Math.PI * Math.Sin(angleFrame / (2.0 * DEATH_ANIM_FRAMES) * Math.PI) * 0.5);
				WR.Rotate(Vector3.UnitX, angle);
				++_deathAnimFrame;
				if (_deathAnimFrame > DEATH_SHOW_TIME) {
					Dead = true;
				}
			}
			_mdl.Render(WR);
			WR.PopMatrix();
		}

		public override void OnCollide(IEntity With) {
			PlayerEntity pe = With as PlayerEntity;
			if (pe != null) {
				pe.Hurt(1);
			}
		}
	}
}