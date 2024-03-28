using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class SlotState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			var slotPos = SlotManager.Inst.GetSlotPos(spring.SlotIndex);
			spring.Deformer.Shrink(slotPos);
			
			await UniTask.WaitUntil(() => {
				return spring.TargetSlotIndex < 0
					|| spring.ExtraSlotIndex >= 0
					|| !spring.IsReachSlot
					|| (spring.EliminateIndex >= 0)
					|| _cts.IsCancellationRequested;
			});
			
			if (_cts.IsCancellationRequested) {
				return;
			}
			
			if (spring.ExtraSlotIndex >= 0) {
				this.enabled = false;
				GetComponent<Slot2ExtraState>().enabled = true;
				return;
			}
			
			if (spring.TargetSlotIndex < 0) {
				this.enabled = false;
				GetComponent<Slot2GridState>().enabled = true;
				return;
			}
			
			if (!spring.IsReachSlot) {
				this.enabled = false;
				GetComponent<Slot2SlotState>().enabled = true;
				return;
			}
			
			if (spring.EliminateIndex >= 0) {
				this.enabled = false;
				GetComponent<EliminateState>().enabled = true;
				return;
			}
		}
	}

}
