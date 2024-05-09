using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace SpringMatch.UI {
	
	public class ToggleButton : MonoBehaviour, IPointerClickHandler
	{
		public RectTransform _handle;
		public Image _fill;
		private bool _on = true;
		public float duration;
		public UnityEvent<bool> OnToggleChange;
		
		public void OnPointerClick(PointerEventData evt) {
			Debug.Log("Click Toggle");
			Toggle();
		}
		
		public bool On {
			get {
				return _on;
			}
			set {
				_on = value;
				if (_on) {
					_fill.fillAmount = 1;
					var p = _handle.anchoredPosition;
					p.x = 0;
					_handle.anchoredPosition = p;
				}
				else {
					_fill.fillAmount = 0;
					var p = _handle.anchoredPosition;
					p.x = -60;
					_handle.anchoredPosition = p;
				}
			}
		}
		
		public void OnToggle(bool val) {
			Debug.Log($"Toggle {val}");
		}
		
		[Button]
		public void Toggle() {
			DOTween.Kill(gameObject, true);
			_on = !_on;
			OnToggleChange.Invoke(_on);
			_fill.DOFillAmount(_on ? 1 : 0, duration).SetTarget(gameObject);
			//_handle.anchorMin = _handle.anchorMax =  new Vector2(_on ? 1 : 0, 0.5f);
			_handle.DOAnchorPosX(_on ? 0 : -60, duration).SetTarget(gameObject);
		}
	}

}
