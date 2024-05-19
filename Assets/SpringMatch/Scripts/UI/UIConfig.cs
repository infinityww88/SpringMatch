using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch.UI {
	[CreateAssetMenu(menuName="SpringMatch/UIConfig", fileName="UIConfig", order=-1)]
	public class UIConfig : ScriptableObject
	{
		public float lifeGoldCost = 15;
		public float refillLifeInterval = 30;
		public float dialogTweenDuration = 0.25f;
		public float dialogTweenTargetScale = 1f;
		public const int MAX_LIFES = 5;
	}

}
