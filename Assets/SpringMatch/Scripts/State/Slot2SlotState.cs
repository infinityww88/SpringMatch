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
			spring.SlotIndex = spring.TargetSlotIndex;
			await spring.TweenToSlot(SlotManager.Inst.GetSlotPos(spring.TargetSlotIndex), _cts.Token).SuppressCancellationThrow();
			this.enabled = false;
			GetComponent<SlotState>().enabled = true;
		}
	}
}
