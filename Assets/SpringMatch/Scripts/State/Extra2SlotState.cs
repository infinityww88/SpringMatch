using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class Extra2SlotState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnablePickupCollider(false);
			int index = spring.TargetSlotIndex;
			ExtraSlotManager.Inst.RemoveSpring(spring.LastExtraSlotIndex);
			await spring.Deformer.Shrink2Target(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex),
				spring.Config.extraSlotAutoHeightFactor,
				spring.Config.slotExtraMoveDuration,
				spring.Config.slotExtraShrinkDuration,
				_cts.Token);
			spring.SlotIndex = index;
			this.enabled = false;
			MsgBus.onToSlot?.Invoke(spring);
			GetComponent<SlotState>().enabled = true;
		}
	}

}
