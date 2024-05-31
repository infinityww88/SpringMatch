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
		
		[SerializeField]
		private UnityEngine.Events.UnityEvent<bool> onToggle;
		
		public bool On { get; set; } = true;
		
		[Button]
		public void SetOn(bool on) {
			On = on;
			var width = _fill.parent.GetComponent<RectTransform>().rect.width - _fill.GetChild(0).GetComponent<RectTransform>().rect.width;
			float f = on ? 0 : 1;
			_fill.offsetMax = new Vector2(-width * f, 0);
		}
		
		[Button]
		public void Toggle() {
			On = !On;
			onToggle.Invoke(On);
			this.DOKill(true);
			var width = _fill.parent.GetComponent<RectTransform>().rect.width - _fill.GetChild(0).GetComponent<RectTransform>().rect.width;
			float t = 0;
			DOTween.To(() => t, v => {
				t = v;
				float f = On ? (1 - t) : t;
				_fill.offsetMax = new Vector2(-width * f, 0);
			}, 1f, 0.1f).SetTarget(this);
		}
	}

}
