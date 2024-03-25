using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class SlotManager : MonoBehaviour
	{
		public Transform dest;
		public Spring spring;
	
		private Slot[] slots;
		private int usedSlotsNum = 0;
		
		public static SlotManager Inst { get; private set; }
		
		public bool IsFull() {
			return false;
		}
		
		public Slot GetSlot(int index) {
			return slots[index];
		}
		
		public Vector3 GetSlotPos(int index) {
			return GetSlot(index).transform.position;
		}
		
		public void AddSpring(Spring spring) {
			if (IsFull()) {
				Debug.Log("slot is full, cannot add new spring");
				return;
			}
			
			int index = SearchSlotInsertIndex(spring.Type);
			MoveSlots(index, 1);
			slots[index].Spring = spring;
			spring.TargetSlotIndex = index;
			usedSlotsNum++;
			if (HasTriple(index)) {
				Debug.Log($"eliminate ready {Time.frameCount}");
				Utils.RunNextFrame(() => {
					Debug.Log($"eliminate start {Time.frameCount}");
					EliminateTriple(index);
					usedSlotsNum -= 3;
				}, 2);
			}
		}
		
		public void UnlockTweenSlot(int middleIndex) {
			slots[middleIndex-1].InEliminateTween = false;
			slots[middleIndex].InEliminateTween = false;
			slots[middleIndex+1].InEliminateTween = false;
		}
	
		void EliminateTriple(int index) {
			
			slots[index-2].InEliminateTween = true;
			slots[index-1].InEliminateTween = true;
			slots[index].InEliminateTween = true;
			
			slots[index-2].Spring.EliminateCompanySpring0 = slots[index-1].Spring;
			slots[index-2].Spring.EliminateCompanySpring1 = slots[index].Spring;
			
			slots[index-1].Spring.EliminateCompanySpring0 = slots[index-2].Spring;
			slots[index-1].Spring.EliminateCompanySpring1 = slots[index].Spring;
			
			slots[index].Spring.EliminateCompanySpring0 = slots[index-2].Spring;
			slots[index].Spring.EliminateCompanySpring1 = slots[index-1].Spring;
			
			/*
			Utils.CallDelay(() => {
				slots[index-2].InEliminateTween = false;
				slots[index-1].InEliminateTween = false;
				slots[index].InEliminateTween = false;
			}, 1.5f);
			*/
			
			slots[index-2].Spring.EliminateIndex = 1;
			slots[index-1].Spring.EliminateIndex = 2;
			slots[index].Spring.EliminateIndex = 0;
			slots[index-2].Spring.EliminateTargetSlotIndex = index-1;
			slots[index-1].Spring.EliminateTargetSlotIndex = index-1;
			slots[index].Spring.EliminateTargetSlotIndex = index-1;
			
			MoveSlots(index + 1, -3);
		}
		
		void MoveSlots(int startIndex, int indexOffset) {
			if (indexOffset > 0) {
				for (int i = usedSlotsNum - 1; i >= startIndex; i--) {
					slots[i].Spring.TargetSlotIndex = i+indexOffset;
					slots[i+indexOffset].Spring = slots[i].Spring;
				}
			}
			else {
				for (int i = startIndex; i < usedSlotsNum; i++) {
					slots[i].Spring.TargetSlotIndex = i+indexOffset;
					slots[i+indexOffset].Spring = slots[i].Spring;
				}
			}
		}
		
		bool HasTriple(int lastIndex) {
			return lastIndex >= 2 
				&& slots[lastIndex].Spring.Type == slots[lastIndex - 1].Spring.Type
				&& slots[lastIndex].Spring.Type == slots[lastIndex - 2].Spring.Type;
		}
		
		int SearchSlotInsertIndex(int type) {
			for (int i = usedSlotsNum - 1; i >= 0; i--) {
				var slot = slots[i];
				if (slot.Spring != null && slot.Spring.Type == type) {
					return i + 1;
				}
			}
			return usedSlotsNum;
		}
	
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			slots = GetComponentsInChildren<Slot>();
		}
	}
}

