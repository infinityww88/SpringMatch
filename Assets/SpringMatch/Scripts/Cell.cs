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
	
		public void SetNum(int totalNum) {
			this.num = totalNum;
			NumInfo.GetComponentInChildren<TextMeshProUGUI>().text = $"{totalNum}";
		}
		
		public void SetNumInfoPos(RectTransform rect) {
			var screenPos = Camera.main.WorldToScreenPoint(transform.GetChild(2).position);
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				rect,
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
