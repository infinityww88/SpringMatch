using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatchEditor {
	
	public class EditorSpring : MonoBehaviour
	{
		public int heightStep = 0;
		public Vector2Int pos0;
		public Vector2Int pos1;
		private List<int> holeSpringTypes = new List<int>();
		private bool _isHole = false;
		public bool HideWhenCovered;
		
		public bool IsValid { get; set; }
		
		public bool IsHole {
			get {
				return _isHole;
			}
			set {
				_isHole = value;
				Clear();
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
