using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace SpringMatch.UI {
	
	public class ToggleButton : MonoBehaviour
	{
		public RectTransform _fill;
		
		public bool Enabled { get; set; } = true;
		
		[Button]
		public void Toggle() {
			Enabled = !Enabled;
			this.DOKill(true);
			var width = _fill.parent.GetComponent<RectTransform>().rect.width - _fill.GetChild(0).GetComponent<RectTransform>().rect.width;
			float t = 0;
			DOTween.To(() => t, v => {
				t = v;
				float f = Enabled ? (1 - t) : t;
				_fill.offsetMax = new Vector2(-width * f, 0);
			}, 1f, 0.1f).SetTarget(this);
		}
	}

}
