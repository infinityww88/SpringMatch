using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;
using ScriptableObjectArchitecture;
using TMPro;

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
		
		[SerializeField]
		private float refillEffectInterval = 0.7f;
		
		[SerializeField]
		private TextMeshProUGUI goldCostText;
		
		private Tween updateSeq = null;
		
		[SerializeField]
		private UnityEngine.Events.UnityEvent OnLifeAvailable;
	
		public void SetHeartNum(int num) {
			int n = Mathf.Clamp(num, 0, hearts.Length);
			for (int i = 0; i < n; i++) {
				hearts[i].sprite = normal;
			}
			for (int i = n; i < hearts.Length; i++) {
				hearts[i].sprite = gray;
			}
		}
		
		[Button]
		public void TestHearts(int num) {
			PrefsManager.Inst.DecHeartNum(num);
		}
		
		[Button]
		public void RefillHearts() {
			if (PrefsManager.Inst.HeartNum == 5) {
				return;
			}
			var requestGold = (5 - PrefsManager.Inst.HeartNum) * UIVariable.Inst.heartGoldCost.Value;
			if (requestGold > PrefsManager.Inst.GoldNum) {
				UIVariable.Inst.shopDialog.gameObject.SetActive(true);
				return;
			}
			updateSeq.Pause();
			var oldNum = PrefsManager.Inst.HeartNum;
			int startIndex = PrefsManager.Inst.HeartNum;
			PrefsManager.Inst.GoldNum -= requestGold;
			PrefsManager.Inst.RefillLives();
			if (oldNum == 0 && PrefsManager.Inst.HeartNum > 0) {
				OnLifeAvailable.Invoke();
			}
			var seq = DOTween.Sequence();
			for (int i = startIndex; i < hearts.Length; i++) {
				var heart = hearts[i].GetComponent<Image>();
				var effect = heart.GetComponentInChildren<ParticleSystem>();
				seq.AppendCallback(() => {
					heart.sprite = normal;
					Debug.Log($"{effect}");
					effect.Stop();
					effect.Play();
					EffectManager.Inst.PlaySoundRefillHeart();
				})
					.AppendInterval(refillEffectInterval)
					.OnComplete(() => updateSeq.Play())
					.SetTarget(this);
			}
		}
		
		public System.TimeSpan GetRemainRefillTime() {
			if (PrefsManager.Inst.HeartNum == 5) {
				return System.TimeSpan.FromSeconds(0);
			}
			return System.TimeSpan.FromSeconds(refillLiveInterval.Value) - (System.DateTime.Now - PrefsManager.Inst.LastRefillLifeTime);
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			updateSeq = DOTween.Sequence().AppendCallback(() => {
				var oldNum = PrefsManager.Inst.HeartNum;
				PrefsManager.Inst.UpdateHeartNum();
				SetHeartNum(PrefsManager.Inst.HeartNum);
				var remain = GetRemainRefillTime();
				timeInfo.text = $"{remain.Minutes:D2}:{remain.Seconds:D2}";
				goldCostText.text = $"{UIVariable.Inst.heartGoldCost.Value * (5 - PrefsManager.Inst.HeartNum)}";
				if (oldNum == 0 && PrefsManager.Inst.HeartNum > 0) {
					OnLifeAvailable.Invoke();
				}
			}).AppendInterval(0.5f).SetLoops(-1, LoopType.Restart).SetTarget(this);
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			DOTween.Kill(this);
			updateSeq = null;
		}
	}
}

