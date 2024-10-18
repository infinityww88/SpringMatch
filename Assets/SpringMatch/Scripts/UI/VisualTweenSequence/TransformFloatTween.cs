using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace VisualTweenSequence {
	
	public class TransformFloatTween : FloatTweenBase
	{
		public enum Attr {
			LocalMoveX,
			LocalMoveY,
			LocalMoveZ,
			LocalEulerAnglesX,
			LocalEulerAnglesY,
			LocalEulerAnglesZ,
			LocalScale,
			LocalScaleX,
			LocalScaleY,
			LocalScaleZ
		}
	
		[SerializeField]
		private Attr tweenAttr;
		
		[SerializeField]
		private Transform target;
		
		[SerializeField]
		[ShowIf("@this._UseRef()")]
		private Transform refStart, refEnd;
		
		private float GetRefValue(Transform refTarget) {
			switch (tweenAttr) {
			case Attr.LocalMoveX:
				return refTarget.localPosition.x;
				break;
			case Attr.LocalMoveY:
				return refTarget.localPosition.y;
				break;
			case Attr.LocalMoveZ:
				return refTarget.localPosition.z;
				break;
			case Attr.LocalEulerAnglesX:
				return refTarget.localEulerAngles.x;
				break;
			case Attr.LocalEulerAnglesY:
				return refTarget.localEulerAngles.y;
				break;
			case Attr.LocalEulerAnglesZ:
				return refTarget.localEulerAngles.z;
				break;
			case Attr.LocalScaleX:
				return refTarget.localScale.x;
				break;
			case Attr.LocalScaleY:
				return refTarget.localScale.y;
				break;
			case Attr.LocalScaleZ:
				return refTarget.localScale.z;
				break;
			case Attr.LocalScale:
				return refTarget.localScale.x;
				break;
			}
			return default(float);
		}
		
		protected override float GetRefStartValue() {
			return GetRefValue(refStart);
		}
		
		protected override float GetRefEndValue() {
			return GetRefValue(refEnd);
		}
		
		protected override Object GetTarget() {
			return target.gameObject;
		}
	
		protected override float Getter()
		{
			float v = 0;
			switch (tweenAttr) {
			case Attr.LocalMoveX:
				v = target.localPosition.x;
				break;
			case Attr.LocalMoveY:
				v = target.localPosition.y;
				break;
			case Attr.LocalMoveZ:
				v = target.localPosition.z;
				break;
			case Attr.LocalEulerAnglesX:
				v = target.localRotation.x * Mathf.Rad2Deg;
				break;
			case Attr.LocalEulerAnglesY:
				v = target.localRotation.y * Mathf.Rad2Deg;
				break;
			case Attr.LocalEulerAnglesZ:
				v = target.localRotation.z * Mathf.Rad2Deg;
				break;
			case Attr.LocalScaleX:
				v = target.localScale.x;
				break;
			case Attr.LocalScaleY:
				v = target.localScale.y;
				break;
			case Attr.LocalScaleZ:
				v = target.localScale.z;
				break;
			case Attr.LocalScale:
				v = target.localScale.x;
				break;
			}
			return v;
		}
	
		protected override void Setter(float v)
		{
			switch (tweenAttr) {
			case Attr.LocalMoveX:
				target.localPosition = transform.localPosition.SetX(v);
				break;
			case Attr.LocalMoveY:
				target.localPosition = transform.localPosition.SetY(v);
				break;
			case Attr.LocalMoveZ:
				target.localPosition = transform.localPosition.SetZ(v);
				break;
			case Attr.LocalEulerAnglesX:
				target.localRotation = Quaternion.EulerAngles(Mathf.Deg2Rad * v, target.localRotation.y, target.localRotation.z);
				break;
			case Attr.LocalEulerAnglesY:
				target.localRotation = Quaternion.EulerAngles(target.localRotation.x, Mathf.Deg2Rad * v, target.localRotation.z);
				break;
			case Attr.LocalEulerAnglesZ:
				target.localRotation = Quaternion.EulerAngles(target.localRotation.x, target.localRotation.y, Mathf.Deg2Rad * v);
				break;
			case Attr.LocalScaleX:
				target.localScale = transform.localScale.SetX(v);
				break;
			case Attr.LocalScaleY:
				target.localScale = transform.localScale.SetY(v);
				break;
			case Attr.LocalScaleZ:
				target.localScale = transform.localScale.SetZ(v);
				break;
			case Attr.LocalScale:
				target.localScale = Vector3.one * v;
				break;
			}
		}
	}
}

