using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace SpringMatch.UI {
	
	public class LevelProgress : MonoBehaviour
	{
		public Image _fill;
		public Image _level2Desc;
		public Image _level3Desc;
		public float duration;
		
		[Button]
		public void ToLevel2() {
			DOTween.Kill(gameObject, true);
			var seq = DOTween.Sequence();
			seq.Append(_fill.DOFillAmount(0.5f, duration).SetTarget(gameObject))
				.AppendCallback(() => {
				_level2Desc.gameObject.SetActive(true);
				_level2Desc.rectTransform.localScale = Vector3.zero;
				})
				.Append(_level2Desc.rectTransform.DOScale(Vector3.one, duration)).SetTarget(gameObject);
		}
		
		[Button]
		public void ToLevel3() {
			DOTween.Kill(gameObject, true);
			var seq = DOTween.Sequence();
			seq.Append(_fill.DOFillAmount(1f, duration).SetTarget(gameObject))
				.AppendCallback(() => {
					_level3Desc.gameObject.SetActive(true);
					_level3Desc.rectTransform.localScale = Vector3.zero;
				})
				.Append(_level3Desc.rectTransform.DOScale(Vector3.one, duration)).SetTarget(gameObject);
		}
	}

}
