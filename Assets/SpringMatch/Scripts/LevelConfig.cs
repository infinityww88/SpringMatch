using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	[CreateAssetMenu(menuName="SpringMatch/LevelConfig", fileName="LevelConfig", order=-1)]
	public class LevelConfig : SerializedScriptableObject
	{
		public List<TextAsset> levels;
		public Dictionary<string, ValueTuple<Vector3, Vector3>> cameraView;
	}

}
