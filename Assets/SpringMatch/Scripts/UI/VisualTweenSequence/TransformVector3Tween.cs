using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace VisualTweenSequence {
	
	public class TransformVector3Tween : Vector3TweenBase
	{
		public enum Attr {
			Move,
			EulerAngles,
			LocalMove,
			LocalEulerAngles,
			LocalScale
		}
		
		protected override bool _UseRef() {
			return true;
		}
		
		[ContextMenu("Copy Start Attribute")]
		void CopyStartAttribute() {
			switch (attr) {
			case Attr.Move:
				startValue = target.position;
				break;
			case Attr.EulerAngles:
				startValue = target.eulerAngles;
				break;
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
			case Attr.Move:
				endValue = target.position;
				break;
			case Attr.EulerAngles:
				endValue = target.eulerAngles;
				break;
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
		
		[SerializeField]
		[ShowIf("@this._UseRef()")]
		private Transform refStart, refEnd;
		
		private Vector3 GetRefValue(Transform refTarget) {
			switch (attr) {
			case Attr.Move:
				return refTarget.position;
				break;
			case Attr.EulerAngles:
				return refTarget.eulerAngles;
				break;
			case Attr.LocalMove:
				return refTarget.localPosition;
				break;
			case Attr.LocalEulerAngles:
				return refTarget.localEulerAngles;
				break;
			case Attr.LocalScale:
				return refTarget.localScale;
				break;
			}
			return default(Vector3);
		}
		
		protected override Vector3 GetRefStartValue() {
			return GetRefValue(refStart);
		}
		
		protected override Vector3 GetRefEndValue() {
			return GetRefValue(refEnd);
		}

		protected override Object GetTarget()
		{
			return target.gameObject;
		}
		
		protected override Vector3 Getter()
		{
			Vector3 v = Vector3.zero;
			switch (attr) {
			case Attr.Move:
				v = target.position;
				break;
			case Attr.EulerAngles:
				v = target.eulerAngles;
				break;
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
			case Attr.Move:
				target.position = v;
				break;
			case Attr.EulerAngles:
				target.eulerAngles = v;
				break;
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

