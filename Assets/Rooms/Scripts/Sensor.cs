using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomRoom {
	
	public class Sensor : MonoBehaviour
	{
		public LayerMask castLayer;
		public float snapDistance = 0.1f;
		
		//private Vector3 lastHitPoint;
		//private bool hasLastHit = false;
		private BoxCollider boxCollider;
		
		[SerializeField]
		private PlaceController placeController;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			placeController = GetComponentInParent<PlaceController>();
			boxCollider = GetComponentInParent<BoxCollider>();
		}
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			if (boxCollider == null) {
				return;
			}
			if (Raycast(out RaycastHit hitInfo, Mathf.Infinity)) {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(hitInfo.point, 0.2f);
			}
		}
		
		bool Raycast(out RaycastHit hitInfo, float distance) {
			return Physics.BoxCast(boxCollider.transform.TransformPoint(boxCollider.center),
				Vector3.Scale(boxCollider.transform.localScale, boxCollider.size) / 2,
				transform.forward,
				out hitInfo,
				boxCollider.transform.rotation,
				distance,
				castLayer.value
			);
		}
		
		bool Raycast(out RaycastHit hitInfo) {
			return Raycast(out hitInfo, snapDistance);
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (Raycast(out RaycastHit hitInfo)) {
				var distance = hitInfo.distance;
				ConsoleProDebug.Watch("contact", $"{distance}");
				if (Mathf.Abs(distance) <= snapDistance) {
					var offsetVec = transform.forward * distance;
					
					//boxCollider.transform.Translate(offsetVec, Space.World);
				}
				else {
					
				}
			}
		}
	}
}

