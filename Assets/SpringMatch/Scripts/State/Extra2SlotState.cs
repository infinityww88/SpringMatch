using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class Extra2SlotState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			int index = spring.TargetSlotIndex;
			await spring.TweenToSlot(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			spring.SlotIndex = index;
			this.enabled = false;
			GetComponent<SlotState>().enabled = true;
		}
	}

}
