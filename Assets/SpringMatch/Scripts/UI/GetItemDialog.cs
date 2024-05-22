using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

namespace SpringMatch.UI {
	
	public class GetItemDialog : MonoBehaviour
	{
		private IntVariable goldCost;
		private UnityEngine.Events.UnityEvent onGet;
		
		public void Show(IntVariable goldCost, UnityEngine.Events.UnityEvent onGet) {
			this.goldCost = goldCost;
			this.onGet = onGet;
			gameObject.SetActive(true);
		}
		
		public void Apply() {
			onGet?.Invoke();
		}
	}

}
