using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpringMatch {
	
	public class Board2HoleState : BaseState
	{
		protected override async UniTaskVoid _Update() {
			Debug.Log($"{spring.gameObject.name} board 2 hole start");
			await spring.Deformer.Stretch2Strink(spring.Foot0Pos, spring.Foot1Pos, spring.Height, _cts.Token);
			Debug.Log($"{spring.gameObject.name} board 2 hole end");
			spring.HoleSpring.GoBack = false;
			spring.EnableRender(false);
			this.enabled = false;
			spring.gameObject.SetActive(false);
		}
	}
}

