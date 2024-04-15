using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatchEditor {
	
	public class EditorSpring : MonoBehaviour
	{
		public int heightStep = 0;
		public int followNum = 0;
		//public bool HideWhenCovered;
		
		public bool IsValid { get; set; }
		
		public bool IsHole => followNum > 0;
	}

}
