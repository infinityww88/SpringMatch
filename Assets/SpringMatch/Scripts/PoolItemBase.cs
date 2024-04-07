using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public abstract class PoolItemBase : MonoBehaviour
	{
		public abstract void OnGet();
		public abstract void OnRelease();
	}

}
