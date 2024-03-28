using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class Board2SlotState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnablePickupCollider(false);
			int index = spring.TargetSlotIndex;
			await spring.Deformer.Stretch2Shrink(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex)).SuppressCancellationThrow();
			//await spring.TweenToSlot(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			spring.SlotIndex = index;
			this.enabled = false;
			GetComponent<SlotState>().enabled = true;
		}
	}

}
