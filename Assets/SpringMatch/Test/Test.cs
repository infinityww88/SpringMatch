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
	public LifeTest lifeTest;
	
    // Start is called before the first frame update
    void Start()
    {

    }
   
	[Button]
	void DoTest() {
		Debug.Log($"test {Time.frameCount}");
		lifeTest.enabled = true;
	}
	
    // Update is called once per frame
    void Update()
    {
        
    }
}
