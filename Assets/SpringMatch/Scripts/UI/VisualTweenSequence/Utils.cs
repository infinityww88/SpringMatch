using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualTweenSequence {
	
	public static class Utils
	{
		public static Vector3 SetX(this Vector3 self, float v) {
			self.x = v;
			return self;
		}
	
		public static Vector3 SetY(this Vector3 self, float v) {
			self.y = v;
			return self;
		}
	
		public static Vector3 SetZ(this Vector3 self, float v) {
			self.z = v;
			return self;
		}
	}
}

