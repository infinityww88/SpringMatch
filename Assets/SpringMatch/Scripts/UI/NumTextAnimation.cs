using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace SpringMatch.UI {
	
	public class NumTextAnimation : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI text;
		[SerializeField]
		private float duration;
		[SerializeField]
		private int startNum, endNum;
		
		[Button]
		public void Play() {
			float t = 0;
			DOTween.To(() => t, v => {
				t = v;
				text.text = $"{(int)Mathf.Lerp(startNum, endNum, t)}";
			}, 1, duration)
				.SetTarget(this);
		}
	}
}

