using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class EliminateState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnablePickupCollider(false);
			await UniTask.WaitUntil(() => spring.EliminateCompanySpring0.IsReachSlot && spring.EliminateCompanySpring1.IsReachSlot || _cts.IsCancellationRequested);
			if (_cts.IsCancellationRequested) {
				return;
			}
			
			var slotMgr = SlotManager.Inst;
			if (spring.EliminateIndex == 0) {
				await spring.Deformer.Shrink2Shrink(slotMgr.GetSlotPos(spring.SlotIndex),
					slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex))
					.SuppressCancellationThrow();
				//await spring.TweenToSlot(slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			} else if (spring.EliminateIndex == 1) {
				await UniTask.WaitUntil(() => spring.EliminateCompanySpring0.End || spring.EliminateCompanySpring1.End);
				await spring.Deformer.Shrink2Shrink(slotMgr.GetSlotPos(spring.SlotIndex),
					slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex)).SuppressCancellationThrow();
				//await spring.TweenToSlot(slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			} else {
				await UniTask.WaitUntil(() => spring.EliminateCompanySpring0.End && spring.EliminateCompanySpring1.End);
				Debug.Log($"state unlock {spring.EliminateTargetSlotIndex}");
				slotMgr.UnlockTweenSlot(spring.EliminateTargetSlotIndex);
				Destroy(spring.gameObject);
				Destroy(spring.EliminateCompanySpring0.gameObject);
				Destroy(spring.EliminateCompanySpring1.gameObject);
				// Play Eliminate Effect
			}
			
			spring.End = true;
		}
	}

}
