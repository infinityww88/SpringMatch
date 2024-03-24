﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpringMatch {
	
	public class Pickup : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent<Spring> _onPickupSpring;
		
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
		    if (Input.touchCount > 0) {
		    	var touch = Input.GetTouch(0);
		    	if (touch.phase ==	TouchPhase.Ended) {
		    		Ray ray = Camera.main.ScreenPointToRay(touch.position);
			    	if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Pickup"))) {
				    	var spring = hitInfo.collider.GetComponentInParent<Spring>();
				    	if (spring != null) {
					    	Debug.Log($"pickup {spring.gameObject.name} {spring.IsTop}");
					    	_onPickupSpring.Invoke(spring);
				    	}
		    		
			    	}
		    	}
		    }
	    }
	}
	
}
