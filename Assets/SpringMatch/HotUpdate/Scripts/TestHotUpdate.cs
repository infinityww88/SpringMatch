using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch.HotUpdate {
	
	public class TestHotUpdate : MonoBehaviour
	{
		public static void Info() {
			Debug.Log($"run test hot update");
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Debug.Log("Test Hot Update Start");
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			transform.Rotate(new Vector3(1 * Time.deltaTime * 60, 1 * Time.deltaTime * 60, 0));
		}
	}
}
