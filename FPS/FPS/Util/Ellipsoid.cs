using System;
using OpenTK;

namespace FPS.Util {
	public class Ellipsoid {
		Vector3 _pos;
		Vector3 _axes;
		Matrix4 _invAxes;

		public Vector3 Pos {
			get { return _pos;}
			set { _pos = value;}
		}

		public Ellipsoid(Vector3 Axes) {
			_axes = Axes;
			_invAxes = new Matrix4(1 / _axes.X, 0, 0, 0,
			                       0, 1 / _axes.Y, 0, 0,
			                       0, 0, 1 / _axes.Z, 0,
			                       0, 0, 0, 1);
			Console.WriteLine(_invAxes);
		}

		public double RayIntersection(Vector3 RPos, Vector3 RDir) {
			Vector4 rpos = new Vector4(RPos - _pos, 1);
			Vector4 rdir = new Vector4(RDir, 1);
			rpos = Vector4.Transform(rpos, _invAxes);
			rdir = Vector4.Transform(rdir, _invAxes);

			//Quadractic Formula
			double A = Vector3.Dot(rdir.Xyz, rdir.Xyz);
			double B = 2 * (Vector3.Dot(rdir.Xyz, rpos.Xyz));
			double C = Vector3.Dot(rpos.Xyz, rpos.Xyz) - 1;
			double disc = (B * B) - (4 * A * C);
			if (disc < 0) {
				return Double.NaN;
			}
			double q = 0;
			if (B < 0)
				q = -B + Math.Sqrt(disc);
			else
				q = -B - Math.Sqrt(disc);
			q /= 2;
			double t0 = q / A;
			double t1 = C / q;
			if (Math.Abs(t0) < Math.Abs(t1))
				return t0;
			else
				return t1;
		}
	}
}

