using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;

namespace CustomRoom {
	
	public class TestRoom : MonoBehaviour
	{
		public float distance;
		public Collider collider;
		private bool hasLastPoint = false;
		private Vector3 lastPoint;
		
		// Start is called before the first frame update
		void Start()
		{
			
		}
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			if (hasLastPoint) {
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(lastPoint, 0.1f);
			}
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			TestCast();
		}
		
		void TestCast() {
			var bound = collider.bounds;
			var ret = Physics.BoxCast(bound.center,
				bound.size / 2,
				Vector3.down,
				out RaycastHit hitInfo,
				Quaternion.identity,
				distance
			);
			if (ret) {
				hasLastPoint = true;
				lastPoint = hitInfo.point;
			}
			else {
				hasLastPoint = false;
			}
		}
	}
}

