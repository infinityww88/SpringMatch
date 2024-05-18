using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class EffectManager : MonoBehaviour
	{
		public static EffectManager Inst;
		
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
		
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
		}

		public void VibratePickup() {
			
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
