using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class SpringShake : MonoBehaviour
	{
		[SerializeField]
		private Transform root;
		
		private Transform[] bones;
		
		public AnimationCurve curve;
		public float strength = 1;
		
		public float duration = 0.5f;
		public int vibrato = 10;
		public float elasticity = 1;
		
		[Button]
		protected void Shake()
		{
			if (root == null) {
				return;
			}
			
			int n = root.childCount;
			Vector3 dir = root.GetChild(n-1).position - root.GetChild(0).position;
			dir.y = 0;
			dir.Normalize();
			dir = Quaternion.Euler(0, 90, 0) * dir;
			Debug.Log($"dir {dir}");
			for (int i = 0; i < n; i++) {
				var c = root.GetChild(i);
				float x = Mathf.Abs((n / 2) - i) / (float)n * 2;
				float s = curve.Evaluate(x) * strength;
				var localOffset = c.parent.InverseTransformVector(dir * s);
				c.DOPunchPosition(localOffset, duration, vibrato, elasticity);
			}
		}
	}

}
