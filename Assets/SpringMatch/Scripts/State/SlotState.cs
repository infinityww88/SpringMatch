using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class SlotState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			await UniTask.WaitUntil(() => {
				return !spring.IsReachSlot
					|| (spring.EliminateIndex >= 0)
					|| _cts.IsCancellationRequested;
			});
			if (_cts.IsCancellationRequested) {
				return;
			}
			
			if (!spring.IsReachSlot) {
				this.enabled = false;
				GetComponent<Slot2SlotState>().enabled = true;
			}
			
			if (spring.EliminateIndex >= 0) {
				this.enabled = false;
				GetComponent<EliminateState>().enabled = true;
				return;
			}
		}
	}

}
