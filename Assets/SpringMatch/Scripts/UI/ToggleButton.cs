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
		public bool _on = true;
		public float duration;
		public UnityEvent<bool> OnToggleChange;
		
		public void OnPointerClick(PointerEventData evt) {
			Debug.Log("Click Toggle");
			Toggle();
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
			_handle.DOAnchorPosX(_on ? 0 : -100, duration).SetTarget(gameObject);
		}
	}

}
