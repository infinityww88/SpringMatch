﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;

namespace SpringMatch {
	
	public class SlotManager : MonoBehaviour
	{
		public Spring spring;
	
		private Slot[] slots;
		private int usedSlotsNum = 0;
		
		public static SlotManager Inst { get; private set; }
		
		public int UsedSlotsNum => usedSlotsNum;
		
		public bool IsFull() {
			return UsedSlotsNum >= 7;
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
				Level.Inst.ClearLastPickupSpring();
				Utils.RunNextFrame(() => {
					EliminateTriple(index);
					usedSlotsNum -= 3;
				}, 2);
			} else {
				if (usedSlotsNum == 7) {
					Level.Inst.OnSlotFull();
				}
			}
		}
		
		public void RemoveSpring(int index) {
			MoveSlots(index + 1, -1);
			usedSlotsNum--;
		}
		
		public Spring[] ShiftOutString(int n) {
			n = Mathf.Min(3, usedSlotsNum);
			Spring[] ret = new	Spring[n];
			for (int i = 0; i < n; i++) {
				var s = slots[i].Spring;
				Assert.IsTrue(s.EliminateIndex < 0);
				ret[i] = s;
			}
			MoveSlots(n, -n);
			usedSlotsNum -= n;
			return ret;
		}
		
		public void UnlockTweenSlot(int middleIndex) {
			slots[middleIndex-1].InEliminateTween--;
			slots[middleIndex].InEliminateTween--;
			slots[middleIndex+1].InEliminateTween--;
		}
	
		void EliminateTriple(int index) {
			
			slots[index-2].InEliminateTween++;
			slots[index-1].InEliminateTween++;
			slots[index].InEliminateTween++;
						
			slots[index-2].Spring.EliminateCompanySpring0 = slots[index-1].Spring;
			slots[index-2].Spring.EliminateCompanySpring1 = slots[index].Spring;
			
			slots[index-1].Spring.EliminateCompanySpring0 = slots[index-2].Spring;
			slots[index-1].Spring.EliminateCompanySpring1 = slots[index].Spring;
			
			slots[index].Spring.EliminateCompanySpring0 = slots[index-2].Spring;
			slots[index].Spring.EliminateCompanySpring1 = slots[index-1].Spring;
			
			slots[index-2].Spring.EliminateIndex = 1;
			slots[index-1].Spring.EliminateIndex = 2;
			slots[index].Spring.EliminateIndex = 0;
			slots[index-2].Spring.EliminateTargetSlotIndex = index-1;
			slots[index-1].Spring.EliminateTargetSlotIndex = index-1;
			slots[index].Spring.EliminateTargetSlotIndex = index-1;
			
			MoveSlots(index + 1, -3);
		}
		
		public void MoveSlots(int startIndex, int indexOffset) {
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

