using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class ExtraSlotManager : MonoBehaviour
	{
		public static ExtraSlotManager Inst { get; private set; }
	
		[SerializeField]
		private float _footWidth = 2;
		
		private ExtraSlot[] _extraSlot;
	
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			_extraSlot = GetComponentsInChildren<ExtraSlot>();
		}
		
		public void AddSprings(Spring[] springs) {
			int n = Mathf.Min(springs.Length, 3);
			for (int i = 0; i < n; i++) {
				_extraSlot[i].Spring = springs[i];
				springs[i].ToExtra(i, GetSlotFoot0Pos(i), GetSlotFoot1Pos(i));
			}
		}
		
		public void RestoreSpring(Spring spring) {
			int i = spring.LastExtraSlotIndex;
			_extraSlot[i].Spring = spring;
			spring.ToExtra(i, GetSlotFoot0Pos(i), GetSlotFoot1Pos(i));
		}
		
		public void RemoveSpring(int index) {
			_extraSlot[index].Spring = null;
		}
		
		public bool Available() {
			return _extraSlot[0].Spring == null && _extraSlot[1].Spring == null && _extraSlot[2].Spring == null;
		}
    
		public Vector3 GetSlotPos(int index) {
			return _extraSlot[index].transform.position;
		}
	
		public Vector3 GetSlotFoot0Pos(int index) {
			return GetSlotPos(index) + Vector3.left * _footWidth / 2;
		}
	
		public Vector3 GetSlotFoot1Pos(int index) {
			return GetSlotPos(index) + Vector3.right * _footWidth / 2;
		}
	}

}
