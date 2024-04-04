using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class Hole2BoardState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			spring.EnableRender(true);
			spring.Deformer.Shrink(spring.Foot0Pos);
			await UniTask.WaitForSeconds(0.3f, cancellationToken: _cts.Token);
			await spring.Deformer.Shrink2StretchWithHeight(spring.Foot0Pos,
				spring.Foot1Pos,
				spring.Height,
				spring.Config.holeGridDuration,
				_cts.Token);
			this.enabled = false;
			GetComponent<BoardState>().enabled = true;
		}
	}

}
