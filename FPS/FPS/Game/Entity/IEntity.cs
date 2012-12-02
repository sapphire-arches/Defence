using System;
using OpenTK;
using FPS.Game;
using FPS.Game.HMap;
using FPS.Render;

namespace FPS.Game.Entity {
	public abstract class IEntity {
		public static readonly float C_OF_FRICTION = 0.5f;
		public static readonly float BOUNDS_SIZE = IslandGenerator.ISLAND_RADIUS;

		protected Vector3 _pos;
		protected Vector3 _vel;
		protected Vector3 _acc;
		float _pitch;
		float _yaw;
		protected float _delta;
		bool _onGround;

		public float Pitch {
			get { return _pitch; }
			set { 
				_pitch = value;
				if (_pitch < -(0.5 * Math.PI))
					_pitch = -(float)Math.PI * 0.5f;
				if (_pitch > 0.5 * Math.PI)
					_pitch = (float)Math.PI * 0.5f;
			}
		}

		public float Yaw {
			get { return _yaw; }
			set {
				_yaw = value;
				while (_yaw > 2 * Math.PI) {
					_yaw -= (float)(2 * Math.PI);
				}
				while (_yaw < 0) {
					_yaw += (float)(2 * Math.PI);
				}
			}
		}

		public Vector3 Pos {
			get { return _pos; }
			set { _pos = value; }
		}

		public bool OnGround {
			get { return _onGround; }
			private set { _onGround = value; }
		}

		public IEntity(Vector3 Pos) {
			_pos = Pos;
			_vel = new Vector3(0, 0, 0);
			_acc = new Vector3(0, 0, 0);
		}

		public void Tick(World W, float Delta) {
			_vel.X *= C_OF_FRICTION;
			_vel.Z *= C_OF_FRICTION;
			ApplyForce(World.GRAVITY);
			_pos += _vel;
			_vel += _acc;
			_acc.X = _acc.Y = _acc.Z = 0;

			float mheight = W.Terrain [_pos.X, _pos.Z];
			if (_pos.Y <= mheight) {
				_onGround = true;
				_pos.Y = mheight;
			} else {
				_onGround = false;
			}

			if (-BOUNDS_SIZE > _pos.X || _pos.X > BOUNDS_SIZE)
				_pos.X = Math.Sign(_pos.X) * BOUNDS_SIZE;
			if (-BOUNDS_SIZE > Pos.Z || _pos.Z > BOUNDS_SIZE)
				_pos.Z = Math.Sign(_pos.Z) * BOUNDS_SIZE;
			_delta = Delta;
		}

		public void ApplyForce(Vector3 Force) {
			_acc += Vector3.Multiply(Force, _delta);
		}

		public abstract void Render(WorldRenderer In);
	}
}
