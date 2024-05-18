using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace VisualTweenSequence {
	
	public class TweenKiller : MonoBehaviour
	{
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
			DOTween.Kill(gameObject);
		}
	}

}
