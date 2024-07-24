using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace CustomRoom {
	
	[RequireComponent(typeof(Button)), RequireComponent(typeof(Image))]
	public class ToggleButton : MonoBehaviour
	{
		[SerializeField]
		[OnValueChanged("OnChanged")]
		private bool _on = true;
		
		[SerializeField]
		[OnValueChanged("OnChanged")]
		private Sprite onImage, offImage;
		
		public bool On => _on;
		
		[SerializeField]
		private Image image;
		
		[SerializeField]
		private UnityEvent onEvent, offEvent;
		
		public void OnChanged() {
			image.sprite = _on ? onImage : offImage;
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			OnChanged();
		}
		
		public void Toggle() {
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

