using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

namespace SpringMatch {
	
	public class BoardState : BaseState
	{	
		protected override async UniTaskVoid _Update() {
			await UniTask.WaitUntil(() => spring.TargetSlotIndex >= 0 || _cts.IsCancellationRequested);
			if (_cts.IsCancellationRequested) {
				return;
			}
			this.enabled = false;
			GetComponent<Board2SlotState>().enabled = true;
		}
	}

}
