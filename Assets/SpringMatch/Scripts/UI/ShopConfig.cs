using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch.UI {
	
	[CreateAssetMenu(menuName="SpringMatch/ShopBundle", fileName="ShopBundle", order=-1)]
	public class ShopBundleConfig : ScriptableObject {
		public string productId;
		public int goldNum;
		public int revokeNum;
		public int shiftNum;
		public int randomNum;
	}
}
