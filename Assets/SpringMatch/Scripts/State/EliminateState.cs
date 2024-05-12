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
				EffectManager.Inst.VibrateMerge();
				await spring.Deformer.Shrink2Shrink(slotMgr.GetSlotPos(spring.SlotIndex),
					slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex),
					spring.Config.slotSlotAutoHeightFactor,
					spring.Config.slotSlotDuration,
					_cts.Token);

			} else if (spring.EliminateIndex == 1) {
				await UniTask.WaitUntil(() => spring.EliminateCompanySpring0.End || spring.EliminateCompanySpring1.End || _cts.IsCancellationRequested);
				if (_cts.IsCancellationRequested) {
					return;
				}
				await spring.Deformer.Shrink2Shrink(slotMgr.GetSlotPos(spring.SlotIndex),
					slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex),
					spring.Config.slotSlotAutoHeightFactor,
					spring.Config.slotSlotDuration,
					_cts.Token);
			} else {
				await UniTask.WaitUntil(() => spring.EliminateCompanySpring0.End && spring.EliminateCompanySpring1.End || _cts.IsCancellationRequested);
				if (_cts.IsCancellationRequested) {
					return;
				}
				Vector3 pos = slotMgr.GetSlotPos(spring.SlotIndex);
				GameLogic.Inst.PlayEliminateEffect(pos);
				slotMgr.UnlockTweenSlot(spring.EliminateTargetSlotIndex);
				Destroy(spring.gameObject);
				Destroy(spring.EliminateCompanySpring0.gameObject);
				Destroy(spring.EliminateCompanySpring1.gameObject);
			}
			
			spring.End = true;
		}
	}

}
