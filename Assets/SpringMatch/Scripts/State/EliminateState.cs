﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class EliminateState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			await UniTask.WaitUntil(() => spring.EliminateCompanySpring0.IsReachSlot && spring.EliminateCompanySpring1.IsReachSlot || _cts.IsCancellationRequested);
			if (_cts.IsCancellationRequested) {
				return;
			}
			
			var slotMgr = SlotManager.Inst;
			if (spring.EliminateIndex == 0) {
				await spring.TweenToSlot(slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			} else if (spring.EliminateIndex == 1) {
				await UniTask.WaitForSeconds(0.5f);
				await spring.TweenToSlot(slotMgr.GetSlotPos(spring.EliminateTargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			} else {
				await UniTask.WaitForSeconds(1f);
				slotMgr.UnlockTweenSlot(spring.EliminateTargetSlotIndex);
				// Play Eliminate Effect
			}
			// release spring
			Destroy(spring.gameObject);
		}
	}

}
