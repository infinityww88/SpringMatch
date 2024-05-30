using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class EffectManager : MonoBehaviour
	{
		public static EffectManager Inst;
		
		[TabGroup("Audio")]
		[SerializeField]
		private AudioClip acRefillHeart, acDialogOpen,
			acStartPlay, acEliminate, acFailed, acInvalid, acJump, acPickup;
		
		[SerializeField]
		private int repeatNum = 25;
		[SerializeField]
		private float delayMSecs = 35f;
		[SerializeField]
		private GameObject eliminateEffect, levelPassEffect;
		[SerializeField]
		private RectTransform effectRoot;
		[SerializeField]
		private float levelPassEffectDelay = 1.5f;
		
		private long[] shortPattern = new long[] {500};
		private long[] longPattern = new long[] {1000};
		
		private AudioSource audioSource;
		
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			audioSource = GetComponent<AudioSource>();
		}

		public void VibratePickup() {
			
		}
		
		public void PlayRefillHeart() {
			audioSource.PlayOneShot(acRefillHeart);
		}
		
		public void PlayStartPlay() {
			audioSource.PlayOneShot(acStartPlay);
		}
		
		public void PlayEliminate() {
			audioSource.PlayOneShot(acEliminate);
		}
		
		public void PlayFailed() {
			audioSource.PlayOneShot(acFailed);
		}
		
		public void PlayInvalid() {
			audioSource.PlayOneShot(acInvalid);
		}
		
		private float lastJumpTime = 0;
		
		public void PlayJump() {
			if (Time.time - lastJumpTime > acJump.length) {
				audioSource.PlayOneShot(acJump);
				lastJumpTime = Time.time;
			}
		}
		
		public void PlayPickup() {
			audioSource.PlayOneShot(acPickup);
		}
		
		public async UniTaskVoid VibrateMerge() {
			for (int i = 0; i < repeatNum; i++) {
				await UniTask.WaitForSeconds(delayMSecs / 1000f);
			}
		}
		
		public void VibrateLevelPass() {
			
		}
		
		public void PlayEliminateEffect(Vector3 pos) {
			Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				effectRoot,
				screenPos, null, out Vector2 localPosition);
			var effect = Instantiate(eliminateEffect, effectRoot);
			Destroy(effect, 2);
			effect.GetComponent<RectTransform>().anchoredPosition = localPosition;
		}
		
		public void PlayLevelPassEffect() {
			Utils.CallDelay(levelPassEffect.GetComponent<UIParticle>().Play, levelPassEffectDelay).Forget();
		}
	}

}
