using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Linq;
using UnityEngine.EventSystems;

namespace CustomRoom {
	
	public class RadioButton : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		private Color normalColor, selectColor;
		[SerializeField]
		private UltEvents.UltEvent selectedEvent, unselectedEvent;
		
		private Image image;
		
		private bool selected;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			image = GetComponent<Image>();
		}
		
		public void OnPointerClick(PointerEventData eventData) {
			Selected = true;
		}
		
		public bool Selected {
			get {
				return selected;
			}
			
			set {
				selected = value;
				image.color = selected ? selectColor : normalColor;
				if (selected) {
					gameObject.Parent()
						.Children()
						.OfComponent<RadioButton>()
						.ForEach(btn => {
							if (btn != this && btn.Selected) {
								btn.Selected = false;
							}
						});
					selectedEvent.Invoke();
				}
				else {
					unselectedEvent.Invoke();
				}
				
			}
		}
	}
}

