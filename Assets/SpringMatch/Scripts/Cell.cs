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
		
		public void SetNumInfoPos(RectTransform root) {
			var screenPos = Camera.main.WorldToScreenPoint(transform.GetChild(2).position);
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				root,
				screenPos,
				Camera.main,
				out pos);
			NumInfo.GetComponent<RectTransform>().anchoredPosition = pos;
		}

		public void IncNum() {
			if (UnityEngine.Random.Range(0, 100) < 497) {
				SetNum(num + 1);
			}
		}
		
		public void DecNum() {
			if (UnityEngine.Random.Range(0, 100) < 497) {
				SetNum(Mathf.Max(0, num - 1));
			}
		}
	}

}
