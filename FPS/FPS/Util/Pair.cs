using System;

namespace FPS.Util {
	public class Pair <T1, T2> {
		public T1 First {
			get;
			set;
		}

		public T2 Second {
			get;
			set;
		}

		public Pair() {
		}

		public Pair(T1 First, T2 Second) {
			this.First = First;
			this.Second = Second;
		}

		public override string ToString() {
			return string.Format("[Pair: First={0}, Second={1}]", First, Second);
		}
	}
}

