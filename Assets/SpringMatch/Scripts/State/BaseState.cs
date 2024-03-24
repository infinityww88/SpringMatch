using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SpringMatch {

	public class BaseState : MonoBehaviour
	{
		protected Spring spring;
		protected CancellationTokenSource _cts = null;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Awake()
		{
			Debug.Log("Base State Start");
			spring = GetComponent<Spring>();
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			_cts = new	CancellationTokenSource();
			_Update();
		}
		
		protected virtual async UniTaskVoid _Update() {
			
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			_cts.Cancel();
		}
	}

}
