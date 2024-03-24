using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

public class Test : MonoBehaviour
{
	public Transform target;
	private CancellationTokenSource _cts;
	
    // Start is called before the first frame update
    void Start()
    {

    }
    
	[Button]
	void Cancel() {
		_cts.Cancel();
	}
	
	[Button]
	void DoTest() {
		_cts = new	CancellationTokenSource();
		Move(_cts.Token);
	}

	
	async UniTask Move(CancellationToken token) {
		while (!token.IsCancellationRequested) {
			Debug.Log($"Hello {Time.frameCount}");
			await UniTask.Delay(1000 * 2);
		}
		
		try {
			
		} finally {
			
		}
		
		Debug.Log($"async function exit");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
