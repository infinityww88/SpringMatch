using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	[CreateAssetMenu(menuName="SpringMatch/LevelPrefabConfig", fileName="LevelPrefabConfig", order=-1)]
	public class LevelPrefabConfig : SerializedScriptableObject
	{
		public Dictionary<string, GameObject> prefabs;
	}

}
