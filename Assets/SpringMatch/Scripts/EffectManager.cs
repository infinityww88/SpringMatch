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
			acStartPlay, acEliminate, acFailed, acSubLevelPass, acSuccess, acInvalid, acJump, acPickup;
		
		[SerializeField]
		private float volume = 0.7f;
		
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
		
		bool SoundOn => PrefsManager.GetBool(PrefsManager.SOUND_ON, true);
		
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			audioSource = GetComponent<AudioSource>();
		}
		
		public void PlayRefillHeart() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = volume;
			audioSource.PlayOneShot(acRefillHeart);
		}
		
		public void PlayStartPlay() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = volume;
			audioSource.PlayOneShot(acStartPlay);
		}
		
		public void PlayEliminate() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = 1;
			audioSource.PlayOneShot(acEliminate);
		}
		
		public void PlayFailed() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = volume;
			audioSource.PlayOneShot(acFailed);
		}
		
		public void PlayInvalid() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = 1;
			audioSource.PlayOneShot(acInvalid);
		}
		
		public void PlaySubLevelPass() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = volume;
			audioSource.PlayOneShot(acSubLevelPass);
		}
		
		public void PlaySuccess() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = volume;
			audioSource.PlayOneShot(acSuccess);
		}
		
		private float lastJumpTime = 0;
		
		public void PlayJump() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = 1;
			if (Time.time - lastJumpTime > acJump.length) {
				audioSource.PlayOneShot(acJump);
				lastJumpTime = Time.time;
			}
		}
		
		public void PlayPickup() {
			if (!SoundOn) {
				return;
			}
			audioSource.volume = 1;
			audioSource.PlayOneShot(acPickup);
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
		
		public void PlayLevelPassEffect(bool subLevelPass) {
			Utils.CallDelay(() => {
				levelPassEffect.GetComponent<UIParticle>().Play();
				if (subLevelPass) {
					PlaySubLevelPass();
				} else {
					PlaySuccess();
				}
			}, levelPassEffectDelay).Forget();
		}
	}

}
