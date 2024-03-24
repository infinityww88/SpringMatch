using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class EliminateState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			var slotMgr = SlotManager.Inst;
			if (spring.EliminateIndex == 0) {
				await spring.TweenToSlot(slotMgr.GetSlotPos(spring.TargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			} else if (spring.EliminateIndex == 1) {
				await UniTask.WaitForSeconds(1f);
				await spring.TweenToSlot(slotMgr.GetSlotPos(spring.TargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			} else {
				await UniTask.WaitForSeconds(1.5f);
				// Play Eliminate Effect
			}
			// release spring
			Destroy(spring.gameObject);
		}
	}

}
