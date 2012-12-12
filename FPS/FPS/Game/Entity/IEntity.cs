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
		protected float _delta;
		protected AABB _bounds;
		int _health;
		float _pitch;
		float _yaw;
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

		public int Health {
			get { return _health; }
			private set { _health = value;}
		}

		public IEntity(Vector3 Pos, AABB Bounds) {
			_pos = Pos;
			_bounds = Bounds;
			_vel = new Vector3(0, 0, 0);
			_acc = new Vector3(0, 0, 0);
		}

		public void Tick(World W, float Delta) {
			_vel.X *= C_OF_FRICTION;
			_vel.Z *= C_OF_FRICTION;
			ApplyForce(World.GRAVITY);
			bool move = true;
			_pos += _vel;
			_vel += _acc;
			foreach (IEntity ent in W.Ents) {
				if (ent != this) {
					bool colide = this.Collides(ent);
					if (colide) {
					}
					move &= !colide;
				}
			}
			if (!move) {
				_pos -= _vel;
				_vel -= _acc;
			}
			_acc.X = _acc.Y = _acc.Z = 0;

			float mheight = W.Terrain [_pos.X, _pos.Z];
			if (_pos.Y <= mheight) {
				_onGround = true;
				_pos.Y = mheight;
			} else {
				_onGround = false;
			}
			if (_pos.Y - mheight < 0.01)
				_onGround = true;

			if (-BOUNDS_SIZE > _pos.X || _pos.X > BOUNDS_SIZE)
				_pos.X = Math.Sign(_pos.X) * BOUNDS_SIZE;
			if (-BOUNDS_SIZE > Pos.Z || _pos.Z > BOUNDS_SIZE)
				_pos.Z = Math.Sign(_pos.Z) * BOUNDS_SIZE;
			_delta = Delta;
		}

		public void ApplyForce(Vector3 Force) {
			_acc += Vector3.Multiply(Force, _delta);
		}

		public bool Collides(IEntity Other) {
			_bounds.Pos = _pos - new Vector3(_bounds.Width / 2, 0, _bounds.Depth / 2);
			Other._bounds.Pos = Other._pos - new Vector3(Other._bounds.Width / 2, 0, Other._bounds.Depth / 2);
			return _bounds.Intersects(Other._bounds);
		}

		public abstract void Render(WorldRenderer In);

		public override string ToString() {
			return string.Format("{0} {1}", Pos, GetType());
		}
	}

	public struct AABB {
		public Vector3 Pos;
		public float Width, Height, Depth;

		public AABB(float Width, float Height, float Depth) {
			this.Width = Width;
			this.Depth = Depth;
			this.Height = Height;
		}

		public bool Intersects(AABB Other) {
			/*
			 * +---+
			 * |   |
			 * |  ++--+
			 * |  ||  |
			 * |  ++--+
			 * |   |
			 * +---+
			 */
			Vector3 op = Other.Pos;
			return
				PointIn(op.X, op.Y, op.Z) ||
				PointIn(op.X + Other.Width, op.Y, op.Z) ||
				PointIn(op.X, op.Y + Other.Height, op.Z) ||
				PointIn(op.X, op.Y, op.Z + Other.Depth) ||
				PointIn(op.X + Other.Width, op.Y + Other.Height, op.Z) ||
				PointIn(op.X + Other.Width, op.Y, op.Z + Other.Depth) ||
				PointIn(op.X, op.Y + Other.Height, op.Z + Other.Depth) ||
				PointIn(op.X + Other.Width, op.Y + Other.Height, op.Z + Other.Depth);
		}

		public bool PointIn(float X, float Y, float Z) {
			return (Pos.X < X && X < Pos.X + Width) &&
			//(Pos.Y <= Y && Y <= Pos.Y + Height) &&
				(Pos.Z < Z && Z < Pos.Z + Depth);
		}

		public override string ToString() {
			return string.Format("{0} [{1}, {2}, {3}]", Pos, Width, Height, Depth);
		}
	}
}
