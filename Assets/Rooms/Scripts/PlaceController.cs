using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace CustomRoom {
	
	public class PlaceController : MonoBehaviour
	{
		[SerializeField]
		private float offsetScale = 0.1f;
		[SerializeField]
		private LayerMask groundMask;
		[SerializeField]
		private Color pickupColor;
		[SerializeField]
		private LayerMask castLayer;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
			DragHandler(token).Forget();
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (Input.GetMouseButtonDown(0)) {
				Pickup(Input.mousePosition);
			}
		}
		
		private GameObject lastPickupObj;
		
		private void Pickup(Vector2 pos) {
			Ray ray = Camera.main.ScreenPointToRay(pos);
			var ret = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, castLayer);
			if (ret) {
				GameObject o = hitInfo.collider.gameObject;
				Outline outline = o.GetComponentInParent<Outline>();
				outline.enabled = true;
				outline.OutlineColor = pickupColor;
				if (lastPickupObj != null) {
					lastPickupObj.GetComponent<Outline>().enabled = false;
				}
				lastPickupObj = outline.gameObject;
			}
		}
		
		private async UniTaskVoid DragHandler(CancellationToken token) {
			while (!token.IsCancellationRequested) {
				while (!Input.GetMouseButtonDown(0)) {
					await UniTask.NextFrame();
					if (token.IsCancellationRequested) {
						return;
					}
				}
				Vector3 lastPos = Input.mousePosition;
				while (!Input.GetMouseButtonUp(0)) {
					await UniTask.NextFrame();
					if (token.IsCancellationRequested) {
						return;
					}
					Ray r0 = Camera.main.ScreenPointToRay(lastPos);
					Ray r1 = Camera.main.ScreenPointToRay(Input.mousePosition);
					Physics.Raycast(r0, out RaycastHit h0, Mathf.Infinity, groundMask);
					Physics.Raycast(r1, out RaycastHit h1, Mathf.Infinity, groundMask);
					Vector3 diff = h1.point - h0.point;
					diff = diff.normalized * offsetScale * (Input.mousePosition - lastPos).magnitude;
					transform.Translate(diff, Space.World);
					lastPos = Input.mousePosition;
				}
			}
		}
	}

}
