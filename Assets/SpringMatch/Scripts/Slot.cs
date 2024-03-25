using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class Slot : MonoBehaviour
	{
		[SerializeField]
		private Spring _spring;
		
		public Spring Spring {
			get {
				return _spring;
			}
			set {
				_spring = value;
			}
		}
		
		public int InEliminateTween = 0;
	}

}
