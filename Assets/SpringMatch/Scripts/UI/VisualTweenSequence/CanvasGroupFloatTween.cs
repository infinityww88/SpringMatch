using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualTweenSequence {
	
	public class CanvasGroupFloatTween : FloatTweenBase
	{
		[SerializeField]
		private CanvasGroup target;
		
		protected override float Getter() {
			return target.alpha;
		}
		
		protected override void Setter(float v)
		{
			target.alpha = v;
		}
	}

}
