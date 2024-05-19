using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpringMatch {
	
	public class ExtraSlotManager : MonoBehaviour
	{
		public static ExtraSlotManager Inst { get; private set; }
		
		private List<Spring> _extraSprings = new	List<Spring>();
		
		[SerializeField]
		float posFactor = 0.6f;
		[SerializeField]
		float minSlotLen = 1f;
		[SerializeField]
		float refMaxLen = 10f;
	
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
		}
		
		public int RemainSpring() {
			return _extraSprings.Count(e => e != null);
		}
		
		public IEnumerable<Spring> GetSprings() {
			return _extraSprings.Where(e => e != null);
		}
		
		public bool Contains(Spring spring) {
			return _extraSprings.Contains(spring);
		}
		
		public int testNum = 3;
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			for (int i = 0; i < testNum; i++) {
				Gizmos.color = i % 2 == 0 ? Color.red : Color.green;
				Vector3 pos0, pos1;
				GetSlotPos(i, testNum, out pos0, out pos1);
				Gizmos.DrawSphere(pos0, 0.2f);
				Gizmos.DrawSphere(pos1, 0.2f);
			}
		}
		
		public void GetSlotPos(int index, int n, out Vector3 pos0, out Vector3 pos1) {
			float slotLen = Mathf.Max(refMaxLen / n, minSlotLen);
			float totalLen = slotLen * n;
			Vector3 left = transform.position + Vector3.left * totalLen / 2;
			left += Vector3.right * slotLen * index;
			Vector3 right = left + Vector3.right * slotLen;
			Vector3 center = (left + right) / 2;
			pos0 = center + Vector3.left * slotLen * posFactor / 2;
			pos1 = center + Vector3.right * slotLen * posFactor / 2;
		}
		
		public void AddSprings(Spring[] springs) {
			_extraSprings.RemoveAll(e => e == null);
			
			int totalNum = springs.Length + _extraSprings.Count;
			
			for (int i = 0; i < _extraSprings.Count; i++) {
				_extraSprings[i].LastExtraSlotIndex = _extraSprings[i].ExtraSlotIndex = i;
				Vector3 pos0, pos1;
				GetSlotPos(i, totalNum, out pos0, out pos1);
				_extraSprings[i].GetComponent<ExtraState>().ShiftPosition(pos0, pos1).Forget();
			}
			
			int n = _extraSprings.Count;
			for (int i = 0; i < springs.Length; i++) {
				Vector3 pos0, pos1;
				GetSlotPos(n + i, totalNum, out pos0, out pos1);
				springs[i].ToExtra(n + i, pos0, pos1);
				_extraSprings.Add(springs[i]);
			}
		}
		
		public void RestoreSpring(Spring spring) {
			int i = spring.LastExtraSlotIndex;
			_extraSprings[i] = spring;
			Vector3 pos0, pos1;
			GetSlotPos(i, _extraSprings.Count, out pos0, out pos1);
			spring.ToExtra(i, pos0, pos1);
		}
		
		public void RemoveSpring(int index) {
			_extraSprings[index] = null;
		}
		
		public bool Available() {
			return true; //_extraSlot[0].Spring == null && _extraSlot[1].Spring == null && _extraSlot[2].Spring == null;
		}
    
		/*
		public Vector3 GetSlotPos(int index) {
			return _extraSlot[index].transform.position;
		}
	
		private Vector3 GetSlotFoot0Pos(int index) {
			return GetSlotPos(index) + Vector3.left * _footWidth / 2;
		}
	
		private Vector3 GetSlotFoot1Pos(int index) {
			return GetSlotPos(index) + Vector3.right * _footWidth / 2;
		}
		*/
	}

}
