using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class SlotState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			await UniTask.WaitUntil(() => {
				return (spring.TargetSlotIndex != spring.SlotIndex)
					|| (spring.EliminateIndex >= 0)
					|| _cts.IsCancellationRequested;
			});
			if (_cts.IsCancellationRequested) {
				return;
			}
			
			if (spring.EliminateIndex >= 0) {
				this.enabled = false;
				GetComponent<EliminateState>().enabled = true;
				return;
			}
			
			if (SlotManager.Inst.GetSlot(spring.TargetSlotIndex).InEliminateTween) {
				await UniTask.WaitUntil(() => !SlotManager.Inst.GetSlot(spring.TargetSlotIndex).InEliminateTween || _cts.IsCancellationRequested);
				if (_cts.IsCancellationRequested) {
					return;
				}
			}
			this.enabled = false;
			GetComponent<Slot2SlotState>().enabled = true;
		}
	}

}
