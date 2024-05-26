using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	[CreateAssetMenu(menuName="SpringMatch/ItemConfig", fileName="itemConfig", order=1)]
	public class ItemConfig : ScriptableObject
	{
		public enum Type {
			Revoke,
			Shift,
			Random
		}
		
		public int goldCost;
		public Type itemType;
	}

}
