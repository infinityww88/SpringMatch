using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SpringMatch.UI {
	
	public class FailDialog : MonoBehaviour
	{
		public RectTransform title;
		public RectTransform recoverConfirm;
		public Button tryAgainButton;
		public Button homeButton;
		
		public float titleHeight = 500;
		public float duration = 0.5f;
		
		// Start is called before the first frame update
		void Start()
		{
        
		}

		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			title.anchoredPosition = Vector2.up * titleHeight;
			tryAgainButton.gameObject.SetActive(false);
			homeButton.gameObject.SetActive(false);
			recoverConfirm.gameObject.SetActive(false);
			
			var seq = DOTween.Sequence();
			seq.Append(title.DOAnchorPosY(0, duration))
				.AppendCallback(() => {
					recoverConfirm.gameObject.SetActive(true);
					recoverConfirm.localScale = Vector2.one * 0.8f;
				})
				.Append(recoverConfirm.DOScale(1, duration).SetEase(Ease.OutBounce));
		}
		
		public void CancelRecover() {
			tryAgainButton.gameObject.SetActive(true);
			homeButton.gameObject.SetActive(true);
			tryAgainButton.transform.localScale = Vector3.one * 0.5f;
			homeButton.transform.localScale = Vector3.one * 0.5f;
			tryAgainButton.transform.DOScale(1, duration);
			homeButton.transform.DOScale(1, duration);
		}
	}

}
