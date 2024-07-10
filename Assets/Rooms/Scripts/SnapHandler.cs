using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomRoom {
	
	[System.Serializable]
	public class SnapHandler {
		[SerializeField]
		private Vector3 dir;
		private bool hasLastSnapPoint = false;
		private Vector3 lastSnapPoint;
		[SerializeField]
		private Collider collider;
		[SerializeField]
		private LayerMask castLayer;
		[SerializeField]
		private float snapDistance;
		
		private System.Action<Collider> onSnapCollider, onNoSnapCollider;
		private Collider lastSnapCollider = null;
		
		public bool IsSnap { get; private set; } = false;
		
		public Vector3 Dir => dir;
		
		public SnapHandler(Vector3 dir,
			Collider collider,
			LayerMask castLayer,
			float snapDistance,
			System.Action<Collider> onSnapCollider,
			System.Action<Collider> onNoSnapCollider) {
				this.dir = dir;
				this.collider = collider;
				this.castLayer = castLayer;
				this.snapDistance = snapDistance;
				this.onSnapCollider = onSnapCollider;
				this.onNoSnapCollider = onNoSnapCollider;
		}
		
		public void SetCollider(Collider collider) {
			this.collider = collider;
		}
		
		public void OnDrawGizmos() {
			if (collider == null) {
				return;
			}
			
			var bound = collider.bounds;
			Gizmos.color = Color.green;
			
			var ret = Physics.BoxCast(bound.center,
				bound.size / 2,
				dir,
				out RaycastHit hitInfo,
				Quaternion.identity,
				Mathf.Infinity,
				castLayer.value
			);
			
			if (ret) {
				Gizmos.DrawSphere(hitInfo.point, 0.2f);
			}
			
			if (hasLastSnapPoint) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(lastSnapPoint, 0.1f);
			}
		}
		
		public static float BoundToPointDistance(Bounds bound, Vector3 dir, Vector3 point) {
			if (dir.x > 0) {
				return point.x - bound.max.x;
			}
			else if (dir.x < 0) {
				return -point.x + bound.min.x;
			}
			else if (dir.y > 0) {
				return point.y - bound.max.y;
			}
			else if (dir.y < 0) {
				return -point.y + bound.min.y;
			}
			else if (dir.z > 0) {
				return point.z - bound.max.z;
			}
			else if (dir.z < 0) {
				return -point.z + bound.min.z;
			}
			return Mathf.Infinity;
		}
		
		public void SnapCollider() {
			SnapCollider(dir);
		}
			
		public void SnapCollider(Vector3 dir) {
			if (collider == null) {
				return;
			}
			
			var bound = collider.bounds;
			
			Outline outline = collider.GetComponent<Outline>();
			
			var ret = Physics.BoxCast(bound.center,
				bound.size / 2,
				dir,
				out RaycastHit hitInfo,
				Quaternion.identity,
				snapDistance,
				castLayer.value
			);
			
			if (ret) {
				float distance = BoundToPointDistance(bound, dir, hitInfo.point);
				collider.transform.Translate(dir * distance, Space.World);
				hasLastSnapPoint = true;
				lastSnapPoint = hitInfo.point;
				lastSnapCollider = hitInfo.collider;
				IsSnap = true;
				onSnapCollider(lastSnapCollider);
			}
			else {
				if (hasLastSnapPoint) {
					float distance = BoundToPointDistance(bound, dir, lastSnapPoint);
					if (Mathf.Abs(distance) <= snapDistance) {
						collider.transform.Translate(dir * distance, Space.World);
						IsSnap = true;
						onSnapCollider(lastSnapCollider);
					}
					else {
						IsSnap = false;
						hasLastSnapPoint = false;
						if (lastSnapCollider != null) {
							onNoSnapCollider(lastSnapCollider);
							lastSnapCollider = null;
						}
					}
				}
			}
		}
	}
	
}
