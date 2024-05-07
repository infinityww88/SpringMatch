using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace SpringMatch {
	public class TextBumpAnimation : MonoBehaviour
	{
		private TextMeshProUGUI[] chars;
		[SerializeField]
		private float duration, delay;
		[SerializeField]
		private Ease ease;
	
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			chars = GetComponentsInChildren<TextMeshProUGUI>();
		}
	
		[Button]
		public Tween Animate() {
			this.DOKill(true);
			var seq = DOTween.Sequence();
			for (int i = 0; i < chars.Length; i++) {
				var c = chars[i];
				c.rectTransform.localScale = Vector2.zero;
				seq.Insert(delay * i,
					c.rectTransform.DOScale(Vector2.one, duration)
					.SetEase(ease)
					.SetTarget(this))
					.SetTarget(this);
			}
			return seq;
		}
	}
}

