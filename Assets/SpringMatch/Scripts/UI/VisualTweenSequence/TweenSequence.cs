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
			public int repeat = 1;
			public float duration = 0f;
			public UltEvent evt;
		}
		
		[SerializeField]
		private float delay = 0;
		
		[SerializeField]
		private List<TweenItem> tweens;
		
		[SerializeField]
		private List<EventItem> events;
		
		[SerializeField]
		private UltEvent onComplete;
		
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
			var seq = DOTween.Sequence()
				.SetTarget(this)
				.SetDelay(delay, false)
				.OnComplete(onComplete.Invoke);
			foreach (var t in tweens) {
				seq.Insert(t.delay, t.tweener.Tween());
			}
			foreach (var e in events) {
				if (e.repeat == 1) {
					seq.InsertCallback(e.delay, () => e.evt?.Invoke());
				}
				else {
					e.repeat = Mathf.Max(e.repeat, 1);
					var evtSeq = DOTween.Sequence().AppendCallback(() => e.evt?.Invoke())
						.AppendInterval(e.duration / e.repeat)
						.SetLoops(e.repeat, LoopType.Restart).SetTarget(this);
					seq.Insert(e.delay, evtSeq);
				}
			}
		}
	}
}
