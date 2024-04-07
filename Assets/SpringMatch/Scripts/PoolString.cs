using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class PoolString : PoolItemBase
	{
		public override void OnGet()
		{
			Debug.Log($"Pool spring {gameObject.name} OnGet");
		}
		
		public override void OnRelease()
		{
			Debug.Log($"Pool spring {gameObject.name} OnRelease");
		}
	}

}
