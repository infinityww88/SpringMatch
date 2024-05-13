using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	[CreateAssetMenu(menuName="SpringMatch/LevelConfig", fileName="LevelConfig", order=-1)]
	public class LevelConfig : ScriptableObject
	{
		public List<TextAsset> levels;
	}

}
