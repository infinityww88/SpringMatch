using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifetimeEvents : MonoBehaviour
{
	public UnityEvent OnEnableEvent;
	public UnityEvent OnDisableEvent;
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		OnEnableEvent.Invoke();
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		OnDisableEvent.Invoke();
	}
}
