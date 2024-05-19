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
			await spring.Deformer.Shrink2Target(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex),
				spring.Config.gridSlotAutoHeightFactor,
				spring.Config.gridSlotMoveDuration,
				spring.Config.girdSlotShrinkDuration,
				_cts.Token);
			spring.SlotIndex = index;
			this.enabled = false;
			MsgBus.onToSlot?.Invoke(spring);
			GetComponent<SlotState>().enabled = true;
		}
	}

}
