using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class Slot2ExtraState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			await spring.TweenToExtra(spring.Foot0Pos, spring.Foot1Pos, spring.Height, _cts.Token)
				.SuppressCancellationThrow();
			spring.SlotIndex = spring.TargetSlotIndex = -1;
			this.enabled = false;
			GetComponent<ExtraState>().enabled = true;
		}
	}

}
