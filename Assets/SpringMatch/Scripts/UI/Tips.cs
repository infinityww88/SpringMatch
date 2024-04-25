using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace SpringMatch.UI {
	
	public class Tips : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI tipsText;
		
		public void ShowTips(string text) {
			gameObject.SetActive(true);
			tipsText.text = text;
			var rt = GetComponent<RectTransform>();
			rt.DOKill();
			rt.anchoredPosition = Vector2.zero;
			var seq = DOTween.Sequence();
			seq.Append(rt.DOAnchorPosY(250, 0.5f))
				.AppendInterval(1f)
				.AppendCallback(() => {
					gameObject.SetActive(false);
				}).SetTarget(rt);
		}
	}

}
