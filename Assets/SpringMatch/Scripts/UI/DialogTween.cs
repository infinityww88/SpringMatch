using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SpringMatch {
	
	public class DialogTween : MonoBehaviour
	{
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			this.DOKill(true);
			DOTween.Sequence().Append(transform.DOScale(1.2f, 4/30f).SetTarget(this))
				.Append(transform.DOScale(1f, 2/30f).SetTarget(this))
				.SetTarget(this);
		}
	}
}

