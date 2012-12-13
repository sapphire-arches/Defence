using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using FPS.Util;
using FPS.Render;
using FPS.GLInterface;

namespace FPS.Game.Entity { 
	public class Enemy : IEntity {
		//Render model
		static Model _mdl;
		//General purpose RNG
		static Random _r;
		//Used to measure strings.
		static Bitmap _garbage;
		static Graphics _garbageg;
		//Format <Name, <BufferID, TextureID>>
		static Pair<String, Rect2D>[] _quotes;
		//Local quote
		Pair<String, Rect2D> _quote;
		public static readonly Ellipsoid BS = new Ellipsoid(new Vector3(1, 2, 1));
		const int DEATH_ANIM_FRAMES = 20;
		const int DEATH_SHOW_TIME = DEATH_ANIM_FRAMES + 600;
		const int HURT_SHOW_FRAMES = 20;
		int _deathAnimFrame;
		int _hurtFrames;
		bool _runDeath;

		static Enemy() {
			_mdl = OBJModelParser.GetInstance().Parse("res/mdl/enemy");
			_garbage = new Bitmap(1, 1);
			_garbageg = Graphics.FromImage(_garbage);
			List<Pair<string, Rect2D>> quotes = new List<Pair<string, Rect2D>>();
			using (StreamReader s = new StreamReader("res/screams.txt")) {
				while (!s.EndOfStream) {
					string q = s.ReadLine();
					while (q.EndsWith(@"\")) {
						q = q.Substring(0, q.Length - 1);
						q += "\n";
						q += s.ReadLine();
					}
					SizeF size = _garbageg.MeasureString(q, SystemFonts.DefaultFont);
					Bitmap img = new Bitmap((int)size.Width + 6, (int)size.Height + 6);
					Graphics g = Graphics.FromImage(img);
					g.FillRectangle(Brushes.White, 0, 0, img.Width, img.Height);
					g.DrawString(q, SystemFonts.DefaultFont, Brushes.Black, 3, 3);
					g.Dispose();
					Pair<string, Rect2D> ins = new Pair<string, Rect2D>();
					ins.First = q;
					const int SCALE = 30;
					const int SCALE2 = 2 * SCALE;
					ins.Second = new Rect2D(img,
							-(size.Width / SCALE2), -(size.Height / SCALE2),
							(size.Width / SCALE), (size.Height / SCALE));
					quotes.Add(ins);
				}
			}
			_garbageg.Dispose();
			_garbageg = null; //Prevent accidental use.
			_quotes = quotes.ToArray();
			_r = new Random();
		}

		public Enemy(Vector3 Pos) : base(Pos, new AABB(1, 2, 1), 10) {
			_deathAnimFrame = 0;
			_quote = _quotes [_r.Next(_quotes.Length)];
		}

		public void AI(World W) {
			if (!_runDeath) {
				IEntity pe = W.GetPlayer();

				Vector3 delta = Vector3.Multiply(Vector3.Normalize(Vector3.Subtract(pe.Pos, _pos)), PlayerEntity.MOVE_SPEED * 0.9f);
				ApplyForce(delta);
				Yaw = (float)Math.Atan2(delta.X, delta.Z);
			}
			if (Health < 0) {
				if (!_runDeath)
					W.BuggerDied();
				_noclip = true;
				_runDeath = true;
			}
		}

		public override void OnHurt(int Damage) {
			_hurtFrames = 0;
		}

		public override void Render(WorldRenderer WR) {
			WR.PushMatrix();
			WR.Translate(_pos.X, _pos.Y, _pos.Z);
			WR.PushMatrix();
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
			if (_hurtFrames < HURT_SHOW_FRAMES) {
				++_hurtFrames;
				WR.SetHighlight(2);
			}
			_mdl.Render(WR);
			WR.SetHighlight(1);
			WR.PopMatrix();
			if (_runDeath) {
				WR.Translate(0, 3.5f, 0);
				WR.Rotate(Vector3.UnitX, (float)(Math.PI));
				WR.Rotate(Vector3.UnitY, WR.Yaw);
				_quote.Second.Render();
			}
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