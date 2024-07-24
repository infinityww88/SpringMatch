using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;
using Sirenix.OdinInspector;

namespace VisualTweenSequence {
	
	public abstract class TweenBase : MonoBehaviour {
		public abstract Tweener Tween();
		public abstract void Play();
		protected abstract Object GetTarget();
	}
	
	public abstract class TweenBase<T> : TweenBase
	{
		public bool useStartValue = true;
		public T startValue;
		public T endValue;
		public float duration = 1;
		public int loop = 1;
		public Ease ease =	Ease.Linear;
		public LoopType loopType =	LoopType.Yoyo;
		public bool autoPlay = true;
		public bool playOnAwake = false;
		public bool reverse = false;
		public float delay = 0;
		
		public bool autoKill = true;
		
		[ShowIf("@this._UseRef()")]
		public bool useRefStartValue = false;
		[ShowIf("@this._UseRef()")]
		public bool useRefEndValue = false;
		
		protected abstract T Getter();
		protected abstract void Setter(T v);
		
		protected virtual bool _UseRef() {
			return false;
		}
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			if (playOnAwake) {
				autoPlay = true;
				Tween();
			}
		}

		protected abstract Tweener CreateTween();
		
		protected virtual T GetRefStartValue() {
			return default(T);
		}
		
		protected virtual T GetRefEndValue() {
			return default(T);
		}
	
		[Button]
		public void Kill(bool complete = false) {
			DOTween.Kill(this, complete);
		}
		
		private T _StartValue => useRefStartValue ? GetRefStartValue() : startValue;
		private T _EndValue => useRefEndValue ? GetRefEndValue() : endValue;
		
		protected T StartValue => reverse ? _EndValue : _StartValue;
		protected T EndValue => reverse ? _StartValue : _EndValue;
		
		public override void Play() {
			Tween().Play();
		}
	
		[Button]
		public override Tweener Tween() {
			if (useStartValue) {
				Setter(StartValue);
			}
			
			var tweener = CreateTween()
				.SetDelay(delay)
				.SetLoops(loop, loopType)
				.SetEase(ease)
				.OnKill(() => onKill?.Invoke())
				.OnComplete(() => onComplete?.Invoke())
				.OnPlay(() => onPlay?.Invoke())
				.SetAutoKill(autoKill)
				.SetId(this)
				.SetTarget(GetTarget());
		
			if (autoPlay) {
				tweener.Play();
			}
		
			return tweener;
		}
	
		public UltEvent onComplete;
		public UltEvent onKill;
		public UltEvent onPlay;
	}
	
	public abstract class FloatTweenBase : TweenBase<float>
	{
		protected override Tweener CreateTween() {
			return DOTween.To(Getter, Setter, EndValue, duration);
		}
	}
	
	public abstract class Vector3TweenBase : TweenBase<Vector3>
	{
		protected override Tweener CreateTween() {
			return DOTween.To(Getter, Setter, EndValue, duration);
		}
	}
	
	public abstract class Vector2TweenBase : TweenBase<Vector2>
	{
		protected override Tweener CreateTween() {
			return DOTween.To(Getter, Setter, EndValue, duration);
		}
	}
}

