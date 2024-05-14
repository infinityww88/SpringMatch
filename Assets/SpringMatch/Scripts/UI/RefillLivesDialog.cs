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
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			updateSeq = DOTween.Sequence().AppendCallback(() => {
				PrefsManager.Inst.UpdateHeartNum();
				SetHeartNum(PrefsManager.Inst.HeartNum);
				var remain = PrefsManager.Inst.GetRemainRefillTime();
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

