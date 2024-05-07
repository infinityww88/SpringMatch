using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UltEvents;

namespace VisualTweenSequence {
	
	public class TweenSequence : MonoBehaviour
	{
		[System.Serializable]
		public class TweenItem {
			public float delay;
			public TweenBase tweener;
		}
		
		[System.Serializable]
		public class EventItem {
			public float delay;
			public UltEvent evt;
		}
		
		[SerializeField]
		private List<TweenItem> tweens;
		
		[SerializeField]
		private List<EventItem> events;
		
		[Button]
		public void Preview() {
		}
		
		[Button]
		public void Kill() {
			foreach (var t in GetComponentsInChildren<TweenBase>()) {
				DOTween.Kill(t, true);
			}
		}
		
		[Button]
		public void Play() {
			this.DOKill(true);
			var seq = DOTween.Sequence().SetTarget(this);
			foreach (var t in tweens) {
				seq.Insert(t.delay, t.tweener.Tween());
			}
			foreach (var e in events) {
				seq.InsertCallback(e.delay, () => e.evt?.Invoke());
			}
		}
	}
}
