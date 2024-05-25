using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace VisualTweenSequence {
	
	public class RectTransformVector2Tween : Vector2TweenBase
	{
		public enum Attr {
			AnchorPos,
			Pivot,
			SizeDelta
		}
		
		[SerializeField]
		private Attr tweenAttr;
		
		[SerializeField]
		private RectTransform target;
		
		protected override Object GetTarget()
		{
			return target.gameObject;
		}
		
		[SerializeField]
		[ShowIf("@this._UseRef()")]
		private RectTransform refStart, refEnd;
		
		private Vector2 GetRefValue(RectTransform refTarget) {
			switch (tweenAttr)
			{
			case Attr.AnchorPos:
				return refTarget.anchoredPosition;
				break;
			case Attr.Pivot:
				return refTarget.pivot;
				break;
			case Attr.SizeDelta:
				return refTarget.sizeDelta;
				break;
			}
			return default(Vector2);
		}
		
		protected override Vector2 GetRefStartValue() {
			return GetRefValue(refStart);
		}
		
		protected override Vector2 GetRefEndValue() {
			return GetRefValue(refEnd);
		}
		
		protected override Vector2 Getter()
		{
			Vector2 v = Vector2.zero;
			switch (tweenAttr)
			{
			case Attr.AnchorPos:
				v = target.anchoredPosition;
				break;
			case Attr.Pivot:
				v = target.pivot;
				break;
			case Attr.SizeDelta:
				v = target.sizeDelta;
				break;
			}
			return v;
		}
		
		protected override void Setter(Vector2 v)
		{
			switch (tweenAttr)
			{
			case Attr.AnchorPos:
				target.anchoredPosition = v;
				break;
			case Attr.Pivot:
				target.pivot = v;
				break;
			case Attr.SizeDelta:
				target.sizeDelta = v;
				break;
			}
		}
	}
}

