﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;
using Sirenix.OdinInspector;

namespace VisualTweenSequence {
	
	public abstract class TweenBase : MonoBehaviour {
		public abstract Tweener Tween();
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
		public bool autoKill = true;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			if (playOnAwake) {
				autoPlay = true;
				Tween();
			}
		}
	
		protected abstract Tweener CreateTween();
	
		[Button]
		public void Kill(bool complete = false) {
			DOTween.Kill(this, complete);
		}
	
		[Button]
		public override Tweener Tween() {
			var tweener = CreateTween()
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
		protected abstract float Getter();
		protected abstract void Setter(float v);
	
		protected override Tweener CreateTween() {
			if (useStartValue) {
				Setter(startValue);
			}
			return DOTween.To(Getter, Setter, endValue, duration);
		}
	}
	
	public abstract class Vector3TweenBase : TweenBase<Vector3>
	{
		protected abstract Vector3 Getter();
		protected abstract void Setter(Vector3 v);
	
		protected override Tweener CreateTween() {
			if (useStartValue) {
				Setter(startValue);
			}
			return DOTween.To(Getter, Setter, endValue, duration);
		}
	}
	
	public abstract class Vector2TweenBase : TweenBase<Vector2>
	{
		protected abstract Vector2 Getter();
		protected abstract void Setter(Vector2 v);
	
		protected override Tweener CreateTween() {
			if (useStartValue) {
				Setter(startValue);
			}
			return DOTween.To(Getter, Setter, endValue, duration);
		}
	}
}

