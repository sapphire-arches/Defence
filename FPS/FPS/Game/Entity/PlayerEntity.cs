using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using FPS.Game.HMap;
using FPS.GLInterface;
using FPS.Render;

namespace FPS.Game.Entity {
	public class PlayerEntity : IEntity {
		public const float MOVE_SPEED = 0.5f;
		public static readonly float JUMP_FORCE = 1f;
		public const float MAX_JUMP_FORCE = 10f;
		public const float MOUSE_SPEED = 0.001f;
		public const int JUMP_FRAMES = 1;
		const float HALFPI = (float)(Math.PI * 0.5);

		int _jumpFrame;
		int _walkFrame;
		Model _sword;

		public PlayerEntity(Vector3 Pos) : base(Pos) {
			_jumpFrame = JUMP_FRAMES;
			_walkFrame = 0;
			_sword = OBJModelParser.GetInstance().Parse("res/sword.obj");
		}

		public void Move(KeyboardDevice KD, Vector2 MouseDelta) {
			Vector3 moveForce = new Vector3(0, 0, 0);
			bool moved = false;
			if (KD [Key.W]) {
				moveForce.Z -= (float)Math.Cos(Yaw) * MOVE_SPEED;
				moveForce.X -= (float)Math.Sin(Yaw) * MOVE_SPEED;
				moved = true;
			}
			if (KD [Key.S]) {
				moveForce.Z += (float)Math.Cos(Yaw) * MOVE_SPEED;
				moveForce.X += (float)Math.Sin(Yaw) * MOVE_SPEED;
				moved = true;
			}
			if (KD [Key.A]) {
				moveForce.Z -= (float)Math.Cos(Yaw + HALFPI) * MOVE_SPEED;
				moveForce.X -= (float)Math.Sin(Yaw + HALFPI) * MOVE_SPEED;
				moved = true;
			}
			if (KD [Key.D]) {
				moveForce.Z += (float)Math.Cos(Yaw + HALFPI) * MOVE_SPEED;
				moveForce.X += (float)Math.Sin(Yaw + HALFPI) * MOVE_SPEED;
				moved = true;
			}
			if (KD [Key.Space] && this.OnGround) {
				_jumpFrame = 0;
			}
			if (_jumpFrame < JUMP_FRAMES) {
				if (_jumpFrame == JUMP_FRAMES - 1) {
					moveForce.Y += JUMP_FORCE;
				}
				++_jumpFrame;
			}
			if (moved)
				++_walkFrame;
			Yaw += MouseDelta.X * MOUSE_SPEED;
			Pitch += MouseDelta.Y * MOUSE_SPEED;
			if (Pitch < WorldRenderer.MIN_PITCH) {
				Pitch = WorldRenderer.MIN_PITCH;
			}
			if (Pitch > WorldRenderer.MAX_PITCH) {
				Pitch = WorldRenderer.MAX_PITCH;
			}
			ApplyForce(moveForce);
		}

		public float GetEyeOffset() {
			return 2 + 0.3f * (float)Math.Sin(Math.PI * _walkFrame * 0.1);
		}

		public override void Render(WorldRenderer In) {
			In.PushMatrix();
			const double angleoffset = 0.6;
			const float offsetscale = 0.1f;
			float sinyaw = (float)Math.Sin(Yaw - angleoffset);
			float cosyaw = (float)Math.Cos(Yaw - angleoffset);
			In.Translate(_pos.X - sinyaw * offsetscale, In.Pos.Y - 0.12f, _pos.Z - cosyaw * offsetscale);
			In.Rotate(Vector3.UnitY, Yaw);
			const float arange = (float)(Math.PI * 0.01);
			const double afreq = Math.PI * 0.01;
			In.Rotate(Vector3.UnitX, arange * (float)Math.Sin(afreq * In.GetFrame() + 2 * arange));
			In.Rotate(Vector3.UnitZ, arange * (float)Math.Cos(afreq * In.GetFrame()));
			_sword.Render();
			In.PopMatrix();
		}
	}
}

