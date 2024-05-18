using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualTweenSequence {
	
	public class TransformVector3Tween : Vector3TweenBase
	{
		public enum Attr {
			LocalMove,
			LocalEulerAngles,
			LocalScale
		}
		
		[ContextMenu("Copy Start Attribute")]
		void CopyStartAttribute() {
			switch (attr) {
			case Attr.LocalMove:
				startValue = target.localPosition;
				break;
			case Attr.LocalEulerAngles:
				startValue = target.localEulerAngles;
				break;
			case Attr.LocalScale:
				startValue = target.localScale;
				break;
			}
		}
		
		[ContextMenu("Copy End Attribute")]
		void CopyEndAttribute() {
			switch (attr) {
			case Attr.LocalMove:
				endValue = target.localPosition;
				break;
			case Attr.LocalEulerAngles:
				endValue = target.localEulerAngles;
				break;
			case Attr.LocalScale:
				endValue = target.localScale;
				break;
			}
		}
		
		public Attr attr;
		
		[SerializeField]
		private Transform target;
		
		protected override Object GetTarget()
		{
			return target.gameObject;
		}
		
		protected override Vector3 Getter()
		{
			Vector3 v = Vector3.zero;
			switch (attr) {
			case Attr.LocalMove:
				v = target.localPosition;
				break;
			case Attr.LocalEulerAngles:
				v = target.localEulerAngles;
				break;
			case Attr.LocalScale:
				v = target.localScale;
				break;
			}
			return v;
		}
		
		protected override void Setter(Vector3 v)
		{
			switch (attr) {
			case Attr.LocalMove:
				target.localPosition = v;
				break;
			case Attr.LocalEulerAngles:
				target.localEulerAngles = v;
				break;
			case Attr.LocalScale:
				target .localScale = v;
				break;
			}
		}
	}
}

