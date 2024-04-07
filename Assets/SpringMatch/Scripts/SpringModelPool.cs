using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

namespace SpringMatch {
	
	public class SpringModelPool : MonoBehaviour
	{
		[SerializeField]
		private SpringConfig _springConfig;
		
		private ObjectPool<GameObject> _modelPool;
	}

}
