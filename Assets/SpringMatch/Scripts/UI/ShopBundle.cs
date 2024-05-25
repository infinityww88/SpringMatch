using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch.UI {
	
	public class ShopBundle : MonoBehaviour
	{
		[SerializeField]
		private ShopBundleConfig shopConfig;
		
		[SerializeField]
		private TMPro.TextMeshProUGUI goldText, revokeText, shiftText, randomText, priceText;
		
		public ShopBundleConfig BundleConfig => shopConfig;
		
		public void Show(ShopBundleConfig shopConfig) {
			this.shopConfig = shopConfig;
			gameObject.SetActive(true);
		}
		
		public void AddBundle() {
			PrefsManager.Inst.GoldNum += shopConfig.goldNum;
			PrefsManager.Inst.RevokeItemNum += shopConfig.revokeNum;
			PrefsManager.Inst.ShiftItemNum += shopConfig.shiftNum;
			PrefsManager.Inst.RandomItemNum += shopConfig.randomNum;
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			if (goldText != null) {
				goldText.text = $"{shopConfig.goldNum}";
			}
			if (revokeText != null) {
				revokeText.text = $"{shopConfig.revokeNum}";
			}
			if (shiftText != null) {
				shiftText.text = $"{shopConfig.shiftNum}";
			}
			if (randomText != null) {
				randomText.text = $"{shopConfig.randomNum}";
			}
			if (priceText != null) {
				priceText.text = $"US ${shopConfig.price:F2}";
			}
		}
	}
}

