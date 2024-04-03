using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatchEditor {
	
	public class EditorSpring : MonoBehaviour
	{
		public int heightStep = 0;
		public Vector2Int pos;
		private List<int> holeSpringTypes = new List<int>();
		
		public bool IsHole {
			get {
				return holeSpringTypes != null && holeSpringTypes.Count > 0;
			}
		}
		
		public IEnumerable<int> HoleSpringTypes => holeSpringTypes;
		
		public void Add(List<int> types) {
			foreach (var t in types) {
				holeSpringTypes.Add(t);
			}
		}
		
		public void Add(int type) {
			holeSpringTypes.Add(type);
		}
		
		public void Set(int index, int type) {
			holeSpringTypes[index] = type;
		}
		
		public void Remove(int index) {
			holeSpringTypes.RemoveAt(index);
		}
		
		public void Clear() {
			holeSpringTypes.Clear();
		}
	}

}
