using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CustomRoom {
	
	[RequireComponent(typeof(Image))]
	public class ToggleButton : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		[OnValueChanged("OnChanged")]
		private bool _on = true;
		
		[SerializeField]
		[OnValueChanged("OnChanged")]
		private Sprite onImage, offImage;
		
		[SerializeField]
		[OnValueChanged("OnChanged")]
		private Color onColor, offColor;
		
		public bool On => _on;
		
		[SerializeField]
		private Image bgImage, image;
		
		[SerializeField]
		private UnityEvent onEvent, offEvent;
		
		public void OnChanged() {
			image.sprite = _on ? onImage : offImage;
			bgImage.color = _on ? onColor : offColor;
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			OnChanged();
		}
		
		public void OnPointerClick(PointerEventData eventData) {
			_on = !_on;
			OnChanged();
			if (_on) {
				onEvent.Invoke();
			}
			else {
				offEvent.Invoke();
			}
		}
	}
}

