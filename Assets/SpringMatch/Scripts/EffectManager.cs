using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarkSDKSpace;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class EffectManager : MonoBehaviour
	{
		public static EffectManager Inst;
		public int repeatNum = 25;
		public float delayMSecs = 35f;
		
		private long[] shortPattern = new long[] {500};
		private long[] longPattern = new long[] {1000};
		
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
		}

		public void VibratePickup() {
			StarkSDK.API.Vibrate(shortPattern, -1);
		}
		
		public async UniTaskVoid VibrateMerge() {
			for (int i = 0; i < repeatNum; i++) {
				Debug.Log($"VibrateMerge {i}");
				StarkSDK.API.Vibrate(shortPattern, -1);
				await UniTask.WaitForSeconds(delayMSecs / 1000f);
			}
		}
		
		public void VibrateLevelPass() {
			StarkSDK.API.Vibrate(longPattern, -1);
		}
	}

}
