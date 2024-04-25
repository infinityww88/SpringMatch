using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarkSDKSpace;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class EffectManager : MonoBehaviour
	{
		public AudioClip eliminateClip;
		public AudioClip slotFullClip;
		public AudioClip invalidClip;
		public AudioClip validClip;
		public AudioClip startGameClip;
		public AudioSource audioSource;
		
		public static EffectManager Inst;
		public int repeatNum = 25;
		public float delayMSecs = 35f;
		
		public bool SoundOn { get; private set; }
		public bool MusicOn { get; private set; }
		public bool VibrateOn { get; private set; }
		
		private long[] shortPattern = new long[] {500};
		private long[] longPattern = new long[] {1000};
		
		public void PlayEliminateSound() {
			if (!SoundOn) {
				return;
			}
			audioSource.PlayOneShot(eliminateClip);
		}
		
		public void PlaySlotFullSound() {
			if (!SoundOn) {
				return;
			}
			audioSource.PlayOneShot(slotFullClip);
		}
		
		public void PlayInvalidSound() {
			if (!SoundOn) {
				return;
			}
			audioSource.PlayOneShot(invalidClip);
		}
		
		public void PlayValidSound() {
			if (!SoundOn) {
				return;
			}
			audioSource.PlayOneShot(validClip);
		}
		
		public void PlayStartGameSound() {
			if (!SoundOn) {
				return;
			}
			audioSource.PlayOneShot(startGameClip);
		}
		
		[SerializeField]
		private GameObject levelPassEffect, mergeEffectPrefab;
		
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			
			SoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
			MusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
			VibrateOn = PlayerPrefs.GetInt("VibrateOn", 1) == 1;
		}
		
		public void EnableSound(bool on) {
			SoundOn = on;
			PlayerPrefs.SetInt("SoundOn", on ? 1 : 0);
			PlayerPrefs.Save();
		}
		
		public void EnableMusic(bool on) {
			MusicOn = on;
			PlayerPrefs.SetInt("MusicOn", on ? 1 : 0);
			PlayerPrefs.Save();
		}
		
		public void EnableVibrate(bool on) {
			VibrateOn = on;
			PlayerPrefs.SetInt("VibrateOn", on ? 1 : 0);
			PlayerPrefs.Save();
		}
		
		public void PlayMergeEffect(Vector3 pos) {
			var effect = Instantiate(mergeEffectPrefab, pos, Quaternion.identity);
			Destroy(effect, 1f);
		}
		
		[Button]
		public void PlayLevelPassEffect() {
			levelPassEffect.GetComponent<ParticleSystem>().Play();
		}

		public void VibratePickup() {
			//StarkSDK.API.Vibrate(shortPattern, -1);
		}
		
		public async UniTaskVoid VibrateMerge() {
			//for (int i = 0; i < repeatNum; i++) {
			//	Debug.Log($"VibrateMerge {i}");
			//	StarkSDK.API.Vibrate(shortPattern, -1);
			//	await UniTask.WaitForSeconds(delayMSecs / 1000f);
			//}
		}
		
		public void VibrateLevelPass() {
			//StarkSDK.API.Vibrate(longPattern, -1);
		}
	}

}
