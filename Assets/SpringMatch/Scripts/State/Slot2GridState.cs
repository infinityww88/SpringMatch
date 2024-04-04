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
			
			if (spring.HoleSpring != null && spring.HoleSpring.NextHoleSpring != null) {
				await UniTask.WaitForSeconds(spring.Config.holeGridDuration, cancellationToken: _cts.Token);
			}
			
			Vector3 slotPos = SlotManager.Inst.GetSlotPos(spring.SlotIndex);
			if ((spring.Foot0Pos - slotPos).magnitude > (spring.Foot1Pos - slotPos).magnitude) {
				spring.SwapFoots();
			}
			await spring.Deformer.Shrink2Stretch3Auto1(
				SlotManager.Inst.GetSlotPos(spring.SlotIndex),
				spring.Foot0Pos,
				spring.Foot1Pos,
				spring.Config.gridSlotAutoHeightFactor,
				spring.Height,
				spring.Config.girdSlotShrinkDuration,
				spring.Config.gridSlotMoveDuration,
				_cts.Token
			);
			spring.SlotIndex = spring.TargetSlotIndex = -1;
			this.enabled = false;
			GetComponent<BoardState>().enabled = true;
		}
	}
	
}
