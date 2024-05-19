using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

