using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		
		public bool InEliminateTween { get; set; }
	}

}
