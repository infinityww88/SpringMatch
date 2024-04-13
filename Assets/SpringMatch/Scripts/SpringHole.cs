using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpringMatch {
	
	public class SpringHole
	{
		private List<Spring> _springs = new List<Spring>();
		
		public int ID { get; set; }
		
		public int Count => _springs.Count;
		
		public void AddSpring(Spring spring) {
			_springs.Add(spring);
		}
		
		public Spring PopSpring() {
			var ret = _springs[0];
			ret.gameObject.SetActive(true);
			_springs.RemoveAt(0);
			return ret;
		}
		
		public void PushSpring(Spring spring) {
			_springs.Insert(0, spring);
		}
		
		public void ForeachSpring(Action<Spring> action) {
			_springs?.ForEach(action);
		}
	}

}
