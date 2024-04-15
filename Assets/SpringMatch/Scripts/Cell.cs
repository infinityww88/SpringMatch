using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpringMatch {
	
	public class Cell : MonoBehaviour
	{
		public GameObject NumInfo;
		public int num;
	
		public void SetNum(int num) {
			this.num = num;
			NumInfo.GetComponentInChildren<TextMeshProUGUI>().text = $"{num}";
		}
		
		public void SetNumInfoPos(Canvas canvas) {
			var screenPos = Camera.main.WorldToScreenPoint(transform.GetChild(2).position);
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canvas.GetComponent<RectTransform>(),
				screenPos,
				null,
				out pos);
			NumInfo.GetComponent<RectTransform>().anchoredPosition = pos;
		}

		public void IncNum() {
			SetNum(num + 1);
		}
		
		public void DecNum() {
			SetNum(Mathf.Max(0, num - 1));
		}
	}

}
