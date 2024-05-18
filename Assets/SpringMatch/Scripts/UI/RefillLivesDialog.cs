using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;
using ScriptableObjectArchitecture;

namespace SpringMatch.UI {
	
	public class RefillLivesDialog : MonoBehaviour
	{
		[SerializeField]
		private Image[] hearts;
	
		[SerializeField]
		private Sprite normal, gray;
	
		[SerializeField]
		private TextMeshProUGUI timeInfo;
		
		[SerializeField]
		private IntVariable refillLiveInterval;
		
		private Tween updateSeq;
	
		public void SetHeartNum(int num) {
			int n = Mathf.Clamp(num, 0, hearts.Length);
			for (int i = 0; i < n; i++) {
				hearts[i].sprite = normal;
			}
			for (int i = n; i < hearts.Length; i++) {
				hearts[i].sprite = gray;
			}
		}
		
		public System.TimeSpan GetRemainRefillTime() {
			if (PrefsManager.HeartNum == 5) {
				return System.TimeSpan.FromSeconds(0);
			}
			return System.TimeSpan.FromSeconds(refillLiveInterval.Value) - (System.DateTime.Now - PrefsManager.LastRefillLifeTime);
		}
		
		public void UpdateHeartNum() {
			System.TimeSpan timeSpan = System.DateTime.Now - PrefsManager.LastRefillLifeTime;
			int seconds = (int)timeSpan.TotalSeconds;
			int shortNum = Mathf.Clamp(5 - PrefsManager.HeartNum, 0, 5);
			int refillNum = seconds / refillLiveInterval.Value;
			int fillNum = Mathf.Min(shortNum, refillNum);
			if (fillNum > 0) {
				PrefsManager.HeartNum += fillNum;
				PrefsManager.LastRefillLifeTime = PrefsManager.LastRefillLifeTime + new System.TimeSpan(0, 0, fillNum * refillLiveInterval.Value);
			}
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			updateSeq = DOTween.Sequence().AppendCallback(() => {
				UpdateHeartNum();
				SetHeartNum(PrefsManager.HeartNum);
				var remain = GetRemainRefillTime();
				timeInfo.text = $"{remain.Minutes:D2}:{remain.Seconds:D2}";
			}).AppendInterval(0.2f).SetLoops(-1, LoopType.Restart).SetTarget(this);
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			updateSeq.Kill();
			updateSeq = null;
		}
	}
}

