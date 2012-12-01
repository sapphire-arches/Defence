using System;
using FPS.Util;

namespace FPS.Game.HMap {
	public interface IGenerator {
		float GetHeight(float X, float Y);
	}

	public class IslandGenerator : IGenerator {
		public static readonly int ISLAND_RADIUS = 500;
		public static readonly int ISLAND_DIAM = 2 * ISLAND_RADIUS;
		public static readonly float MAX_HEIGHT = 30f;
		public static readonly float DETAIL_1_HEIGHT = 10f;
		public static readonly float DETAIL_2_HEIGHT = 1f;
		
		Perlin2D _p2d;

		public IslandGenerator(Perlin2D Perlin) {
			_p2d = Perlin;
		}

		public float GetHeight(float X, float Y) {
			float temp = 0;
			//temp += (float)_p2d [X * 0.001, Y * 0.001] * (MAX_HEIGHT) - (MAX_HEIGHT / 2);
			temp += (float)_p2d [X * 0.025, Y * 0.025] * (DETAIL_1_HEIGHT) - (DETAIL_1_HEIGHT / 2);
			temp += (float)_p2d [X * 0.100, Y * 0.100] * (DETAIL_2_HEIGHT) - (DETAIL_2_HEIGHT / 2);
			X = Min(Max(X, -ISLAND_RADIUS), ISLAND_RADIUS);
			Y = Min(Max(Y, -ISLAND_RADIUS), ISLAND_RADIUS);
			temp += (MAX_HEIGHT / 2) * (float)(Math.Cos((X * Math.PI) / ISLAND_RADIUS));
			temp += (MAX_HEIGHT / 2) * (float)(Math.Cos((Y * Math.PI) / ISLAND_RADIUS));

			return temp;
		}

		private float Min(float F1, float F2) {
			if (F1 < F2)
				return F1;
			return F2;
		}

		private float Max(float F1, float F2) {
			if (F1 > F2)
				return F1;
			return F2;
		}
	}
}

