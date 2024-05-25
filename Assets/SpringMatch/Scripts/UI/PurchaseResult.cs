using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SpringMatch.UI {
	
	public class PurchaseResult : SerializedMonoBehaviour
	{
		[SerializeField]
		private Dictionary<string, ShopBundleConfig> bundleProducts;
		
		[SerializeField]
		private ShopBundle shopBundleResult;
		
		public void OnPurchaseSuccess(string productId) {
			if (!bundleProducts.ContainsKey(productId)) {
				Debug.Log($"{gameObject.name} doesn't contain shop bundle for {productId}");
				return;
			}
			shopBundleResult.Show(bundleProducts[productId]);
		}
	}

}
