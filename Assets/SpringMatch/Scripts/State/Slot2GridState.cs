using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class Slot2GridState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnablePickupCollider(false);
			Vector3 slotPos = SlotManager.Inst.GetSlotPos(spring.SlotIndex);
			if ((spring.Foot0Pos - slotPos).magnitude > (spring.Foot1Pos - slotPos).magnitude) {
				spring.SwapFoots();
			}
			await spring.Deformer.Shrink2Stretch(
				SlotManager.Inst.GetSlotPos(spring.SlotIndex),
				spring.Foot0Pos,
				spring.Foot1Pos,
				spring.Height,
				_cts.Token
			);
			//await spring.TweenToGrid(spring.Foot0Pos, spring.Foot1Pos, spring.Height, _cts.Token)
			//.SuppressCancellationThrow();
			spring.SlotIndex = spring.TargetSlotIndex = -1;
			this.enabled = false;
			GetComponent<BoardState>().enabled = true;
		}
	}
	
}
