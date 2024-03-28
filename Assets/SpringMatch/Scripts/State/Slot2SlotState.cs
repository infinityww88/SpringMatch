using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class Slot2SlotState : BaseState
	{
		// This function is called when the object becomes enabled and active.
		protected new void OnEnable()
		{
			base.OnEnable();
		}
		
		protected override async UniTaskVoid _Update()
		{
			spring.EnablePickupCollider(false);
			if (SlotManager.Inst.GetSlot(spring.TargetSlotIndex).InEliminateTween > 0 && spring.EliminateIndex < 0) {
				await UniTask.WaitUntil(() => SlotManager.Inst.GetSlot(spring.TargetSlotIndex).InEliminateTween == 0 || spring.EliminateIndex >= 0 || _cts.IsCancellationRequested);
				if (_cts.IsCancellationRequested) {
					return;
				}
			}
			
			int index = spring.TargetSlotIndex;
			
			await spring.Deformer.Shrink2Shrink(
				SlotManager.Inst.GetSlotPos(spring.SlotIndex),
				SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex))
				.SuppressCancellationThrow();
			//await spring.TweenToSlot(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			
			spring.SlotIndex = index;
			
			this.enabled = false;
			GetComponent<SlotState>().enabled = true;
		}
	}
}
