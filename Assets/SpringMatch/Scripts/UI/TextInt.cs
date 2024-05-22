using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch.UI {
	
	public class TextInt : MonoBehaviour
	{
		public void SetValue(int value) {
			GetComponent<TMPro.TextMeshProUGUI>().text = $"{value}";
		}
	}

}
