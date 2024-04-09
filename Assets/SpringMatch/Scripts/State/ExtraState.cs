using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

namespace SpringMatch {
	
	public class ExtraState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnablePickupCollider(true);
			spring.GeneratePickupColliders(spring.Config.colliderRadius);
			await UniTask.WaitUntil(() => spring.TargetSlotIndex >= 0 || _cts.IsCancellationRequested);
			if (_cts.IsCancellationRequested) {
				return;
			}
			spring.ExtraSlotIndex = -1;
			this.enabled = false;
			GetComponent<Extra2SlotState>().enabled = true;
		}
		
		public async UniTaskVoid ShiftPosition(Vector3 foot0, Vector3 foot1) {
			spring.EnablePickupCollider(false);
			Vector3 pos0 = spring.Foot0Pos;
			Vector3 pos1 = spring.Foot1Pos;
			float t = 0;
			await DOTween.To(() => t, v => {
				t = v;
				spring.Deformer.SetPose(Vector3.Lerp(pos0, foot0, t), Vector3.Lerp(pos1, foot1, t), spring.Height);
			}, 1, spring.Config.slotExtraMoveDuration).WithCancellation(_cts.Token);
			spring.EnablePickupCollider(true);
			spring.GeneratePickupColliders(spring.Config.colliderRadius);
		}
	}

}
