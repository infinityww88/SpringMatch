using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

namespace SpringMatch.UI {
	
	public class LevelPass : MonoBehaviour
	{
		[SerializeField]
		private TextBumpAnimation greatText;
		[SerializeField]
		private CanvasGroup goldInfo, nextButton, adButton;
		[SerializeField]
		private float duration;
		[SerializeField]
		private float buttonScaleDuration;
		[SerializeField]
		private float buttonScale;
		
		[Button]
		private async UniTaskVoid Animate() {
			this.DOKill(true);
			goldInfo.alpha = nextButton.alpha = adButton.alpha = 0;
			
			var tween0 = greatText.Animate();
			
			var tween1 = goldInfo.DOFade(1, duration);
			var tween2 = nextButton.DOFade(1, duration);
			var tween3 = adButton.DOFade(1, duration);
			var seq1 = DOTween.Sequence()
				.AppendInterval(1f)
				.Append(tween1)
				.Append(tween2)
				.Append(tween3)
				.SetTarget(this);
			
			var seq = DOTween.Sequence();
			seq.Append(tween0)
				.Join(seq1)
				.SetTarget(this)
				.OnComplete(() => {
					nextButton.GetComponent<RectTransform>()
						.DOScale(buttonScale, buttonScaleDuration)
						.SetLoops(-1, LoopType.Yoyo)
						.SetEase(Ease.Linear);
					adButton.GetComponent<RectTransform>()
						.DOScale(buttonScale, buttonScaleDuration)
						.SetLoops(-1, LoopType.Yoyo)
						.SetEase(Ease.Linear);
				});
		}
	}
}

