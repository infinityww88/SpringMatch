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
		public float strengthFactor = 1;
		
		public float duration = 0.5f;
		public int vibrato = 10;
		public float elasticity = 1;
		
		[Button]
		public void Shake(TweenCallback onEnd)
		{
			if (root == null) {
				onEnd?.Invoke();
				return;
			}
						
			int n = root.childCount;
			Vector3 dir = root.GetChild(n-1).position - root.GetChild(0).position;
			float strength = dir.magnitude * strengthFactor;
			dir.y = 0;
			dir.Normalize();
			dir = Quaternion.Euler(0, 90, 0) * dir;
			var seq = DOTween.Sequence();
			for (int i = 0; i < n; i++) {
				var c = root.GetChild(i);
				float x = Mathf.Abs((n / 2) - i) / (float)n * 2;
				float s = curve.Evaluate(x) * strength;
				var localOffset = c.parent.InverseTransformVector(dir * s);
				seq.Join(c.DOPunchPosition(localOffset, duration, vibrato, elasticity));
			}
			seq.SetTarget(gameObject).onComplete = onEnd;
		}
	}

}
