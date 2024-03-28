using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class ExtraState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnablePickupCollider(true);
			spring.GeneratePickupColliders(0.35f);
			await UniTask.WaitUntil(() => spring.TargetSlotIndex >= 0 || _cts.IsCancellationRequested);
			if (_cts.IsCancellationRequested) {
				return;
			}
			spring.ExtraSlotIndex = -1;
			this.enabled = false;
			GetComponent<Extra2SlotState>().enabled = true;
		}
	}

}
