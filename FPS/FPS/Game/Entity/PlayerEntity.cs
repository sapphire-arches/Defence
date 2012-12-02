using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using FPS.Game.HMap;
using FPS.GLInterface;

namespace FPS.Game.Entity {
	public class PlayerEntity : IEntity {
		public static readonly float MOVE_SPEED = 2f;
		public static readonly float JUMP_FORCE = 2f * -World.GRAVITY.Y;
		public static readonly float MAX_JUMP_FORCE = 10f;
		public static readonly float MOUSE_SPEED = 0.001f;
		public static readonly int JUMP_FRAMES = 5;

		int _jumpFrame = 0;
		Model _sword;

		public PlayerEntity(Vector3 Pos) : base(Pos) {
			_sword = OBJModelParser.GetInstance().Parse("res/sword.obj");
		}

		public void Move(KeyboardDevice KD, Vector2 MouseDelta) {
			Vector3 moveForce = new Vector3(0, 0, 0);
			if (KD [Key.W]) {
				moveForce.Z -= (float)Math.Cos(Yaw) * MOVE_SPEED;
				moveForce.X -= (float)Math.Sin(Yaw) * MOVE_SPEED;
			}
			if (KD [Key.S]) {
				moveForce.Z += (float)Math.Cos(Yaw) * MOVE_SPEED;
				moveForce.X += (float)Math.Sin(Yaw) * MOVE_SPEED;
			}
			if (KD [Key.Space] && this.OnGround) {
				_jumpFrame = 0;
			}
			if (_jumpFrame < JUMP_FRAMES) {
				++_jumpFrame;
				moveForce.Y += JUMP_FORCE;
			}
			Yaw = MouseDelta.X * MOUSE_SPEED;
			Pitch = MouseDelta.Y * MOUSE_SPEED;
			ApplyForce(moveForce);
		}

		public override void Render() {
			_sword.Render();
		}
	}
}

