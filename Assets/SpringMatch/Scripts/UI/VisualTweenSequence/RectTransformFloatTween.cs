﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VisualTweenSequence {
	
	public class RectTransformFloatTween : FloatTweenBase
	{
		public enum Attr {
			AnchorPosX,
			AnchorPosY,
			AnchorPos3DX,
			AnchorPos3DY,
			AnchorPos3DZ,
			PivotX,
			PivotY,
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
		
		protected override float Getter()
		{
			float v = 0;
			switch (tweenAttr)
			{
			case Attr.AnchorPosX:
				v = target.anchoredPosition.x;
				break;
			case Attr.AnchorPosY:
				v = target.anchoredPosition.y;
				break;
			case Attr.AnchorPos3DX:
				v = target.anchoredPosition3D.x;
				break;
			case Attr.AnchorPos3DY:
				v = target.anchoredPosition3D.y;
				break;
			case Attr.AnchorPos3DZ:
				v = target.anchoredPosition3D.z;
				break;
			case Attr.PivotX:
				v = target.pivot.x;
				break;
			case Attr.PivotY:
				v = target.pivot.x;
				break;
			case Attr.SizeDelta:
				v = target.sizeDelta.x;
				break;
			}
			return v;
		}
		
		protected override void Setter(float v)
		{
			switch (tweenAttr)
			{
			case Attr.AnchorPosX:
				target.anchoredPosition = target.anchoredPosition.SetX(v);
				break;
			case Attr.AnchorPosY:
				target.anchoredPosition = target.anchoredPosition.SetY(v);
				break;
			case Attr.AnchorPos3DX:
				target.anchoredPosition3D = target.anchoredPosition3D.SetX(v);
				break;
			case Attr.AnchorPos3DY:
				target.anchoredPosition3D = target.anchoredPosition3D.SetY(v);
				break;
			case Attr.AnchorPos3DZ:
				target.anchoredPosition3D = target.anchoredPosition3D.SetZ(v);
				break;
			case Attr.PivotX:
				target.pivot = target.pivot.SetX(v);
				break;
			case Attr.PivotY:
				target.pivot = target.pivot.SetY(v);
				break;
			case Attr.SizeDelta:
				target.sizeDelta = Vector2.one * v;
				break;
			}
		}
	}
}

