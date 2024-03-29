using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class HoleSpring : MonoBehaviour
	{
		public int HoleId { get; set; }
		public Spring PrevHoleSpring { get; set; }
		public Spring NextHoleSpring { get; set; }
		[ShowInInspector]
		public bool GoBack { get; set; }
		
		private Spring _spring;
		
		// Start is called before the first frame update
		void Awake()
		{
			_spring = GetComponent<Spring>();
		}
	}

}
