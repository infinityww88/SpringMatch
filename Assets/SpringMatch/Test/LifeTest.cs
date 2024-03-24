using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		Debug.Log($"Life enable {Time.frameCount}");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
