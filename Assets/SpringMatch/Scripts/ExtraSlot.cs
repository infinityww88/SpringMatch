using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class ExtraSlot : MonoBehaviour
	{
		[ShowInInspector]
		public Spring Spring { get; set; }
	}
}

