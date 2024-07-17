using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomRoom {
	
	[System.Serializable]
	public class SnapHandler : MonoBehaviour {
		
		[SerializeField]
		private Vector3 dir;
		[SerializeField]
		private LayerMask castLayer;
		[SerializeField]
		private float snapDistance;
		[SerializeField]
		private GameObject projectIndictorPrefab;
		
		private ProjectIndictor projectIndictor;
		
		private bool hasLastSnapPoint = false;
		private Vector3 lastSnapPoint;
		
		private Collider collider;
		private Collider lastSnapCollider = null;
		
		private bool inSnap = false;
		
		[SerializeField]
		private UnityEvent<Collider> onSnapCollider, onNoSnapCollider;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			var o = Instantiate(projectIndictorPrefab);
			projectIndictor = o.GetComponent<ProjectIndictor>();
			o.SetActive(false);
		}
		
		public void SetCollider(Collider collider) {
			this.collider = collider;
		}
		
		public void OnDrawGizmos() {
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
		
		Bounds GetBound() {
			return collider.bounds;
		}
		
		public void SnapIndictor() {
			if (collider == null) {
				return;
			}
			var ret = Physics.Raycast(collider.bounds.center,
				dir,
				out RaycastHit hitInfo,
				Mathf.Infinity,
				castLayer.value);
			if (ret && !inSnap) {
				projectIndictor.gameObject.SetActive(true);
				projectIndictor.SetPos(collider.bounds.center, hitInfo.point);
			}
			else {
				projectIndictor.gameObject.SetActive(false);
			}
		}
		
		// LateUpdate is called every frame, if the Behaviour is enabled.
		protected void LateUpdate()
		{
			SnapCollider();
			SnapIndictor();
		}
			
		public void SnapCollider(Vector3 dir) {
			if (collider == null) {
				return;
			}
			
			projectIndictor.gameObject.SetActive(false);

			var bound = GetBound();
			
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
				Physics.SyncTransforms();
				inSnap = true;
				hasLastSnapPoint = true;
				lastSnapPoint = hitInfo.point;
				lastSnapCollider = hitInfo.collider;
				onSnapCollider.Invoke(hitInfo.collider);
			}
			else {
				if (hasLastSnapPoint) {
					float distance = BoundToPointDistance(bound, dir, lastSnapPoint);
					if (Mathf.Abs(distance) <= snapDistance) {
						collider.transform.Translate(dir * distance, Space.World);
						Physics.SyncTransforms();
						inSnap = true;
						onSnapCollider.Invoke(lastSnapCollider);
					}
					else {
						inSnap = false;
						onNoSnapCollider.Invoke(lastSnapCollider);
					}
				}
			}
		}
	}
	
}
