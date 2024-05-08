using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace SpringMatch {

	public class ButtonTween : MonoBehaviour, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData evtData) {
			this.DOKill(true);
			transform.DOScale(0.88f, 0.1f).SetLoops(2, LoopType.Yoyo).SetTarget(this);
		}
	}

}
