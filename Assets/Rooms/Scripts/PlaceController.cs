using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using ScriptableObjectArchitecture;

namespace CustomRoom {
	
	public class PlaceController : MonoBehaviour
	{
		[SerializeField]
		private LayerMask groundMask;
		[SerializeField]
		private Color pickupColor;
		[SerializeField]
		private LayerMask pickupLayer, snapLayer;
		[SerializeField]
		private FloatVariable rotateFactor, moveFactor, heightFactor;
		[SerializeField]
		private float snapDistance = 0.1f;
		[SerializeField]
		private Color snapHintColor = Color.white;
		
		private CancellationTokenSource cancelTokenSource = null;
		
		private List<SnapHandler> snapHandlers = new List<SnapHandler>();
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			snapHandlers.Add(new SnapHandler(Vector3.back, null, snapLayer, snapDistance, OnSnapCollider, OnNoSnapCollider));
			snapHandlers.Add(new SnapHandler(Vector3.down, null, snapLayer, snapDistance, OnSnapCollider, OnNoSnapCollider));
			snapHandlers.Add(new SnapHandler(Vector3.left, null, snapLayer, snapDistance, OnSnapCollider, OnNoSnapCollider));
		}
		
		public void OnSnapCollider(Collider collider) {
			collider.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", snapHintColor);
		}
		
		public void OnNoSnapCollider(Collider collider) {
			collider.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
		}
		
		public void StartHorzEdit() {
			if (cancelTokenSource != null) {
				cancelTokenSource.Cancel();
				cancelTokenSource = null;
			}
			cancelTokenSource = new CancellationTokenSource();
			EditHandler(cancelTokenSource.Token, HorzHandler).Forget();
		}
		
		public void StartRotateEdit() {
			if (cancelTokenSource != null) {
				cancelTokenSource.Cancel();
				cancelTokenSource = null;
			}
			cancelTokenSource = new CancellationTokenSource();
			EditHandler(cancelTokenSource.Token, RotateHandler).Forget();
		}
		
		public void StartHeightEdit() {
			if (cancelTokenSource != null) {
				cancelTokenSource.Cancel();
				cancelTokenSource = null;
			}
			cancelTokenSource = new CancellationTokenSource();
			EditHandler(cancelTokenSource.Token, HeightHandler).Forget();
		}
		
		public void Rotate90Clockwise() {
			if (lastPickupObj == null) {
				return;
			}
			Snap90Degree(lastPickupObj.transform);
			lastPickupObj.transform.eulerAngles -= Vector3.up * 90;
		}
		
		public void Rotate90AntiClockwise() {
			if (lastPickupObj == null) {
				return;
			}
			Snap90Degree(lastPickupObj.transform);
			lastPickupObj.transform.eulerAngles += Vector3.up * 90;
		}
		
		public void Snap90Degree(Transform t) {
			var y = t.eulerAngles.y;
			y = Mathf.Repeat(y, 360);
			for (int i = 0; i < 5; i++) {
				float a = i * 90;
				if (Mathf.Abs(y - a) <= 45) {
					t.eulerAngles = new	Vector3(0, a, 0);
					break;
				}
			}
		}
		
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
			if (cancelTokenSource != null) {
				cancelTokenSource.Cancel();
			}
			cancelTokenSource = null;
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (Input.GetMouseButtonDown(0)) {
				Pickup(Input.mousePosition);
			}
			
			snapHandlers.ForEach(sh => {
				sh.SnapCollider();
			});
		}
		
		private GameObject lastPickupObj;
		
		private void Pickup(Vector2 pos) {
			Ray ray = Camera.main.ScreenPointToRay(pos);
			var ret = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, pickupLayer);
			if (ret) {
				GameObject o = hitInfo.collider.gameObject;
				Outline outline = o.GetComponentInParent<Outline>();
				outline.enabled = true;
				outline.OutlineColor = pickupColor;
				if (lastPickupObj != null && lastPickupObj != outline.gameObject) {
					lastPickupObj.GetComponent<Outline>().enabled = false;
				}
				lastPickupObj = outline.gameObject;
				snapHandlers.ForEach(sh => {
					sh.SetCollider(lastPickupObj.GetComponent<Collider>());
				});
			}
		}
		
		private async UniTaskVoid EditHandler(CancellationToken token, System.Action<Vector2, Vector2> func) {
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
					func(Input.mousePosition, lastPos);
					lastPos = Input.mousePosition;
				}
			}
		}
		
		private void HorzHandler(Vector2 currentMousePos, Vector2 lastMousePos) {
			Ray r0 = Camera.main.ScreenPointToRay(lastMousePos);
			Ray r1 = Camera.main.ScreenPointToRay(currentMousePos);
			Physics.Raycast(r0, out RaycastHit h0, Mathf.Infinity, groundMask);
			Physics.Raycast(r1, out RaycastHit h1, Mathf.Infinity, groundMask);
			Vector3 diff = (h1.point - h0.point) * moveFactor.Value;
			if (lastPickupObj != null) {
				lastPickupObj.transform.Translate(diff, Space.World);
			}
		}
		
		private void RotateHandler(Vector2 currentMousePos, Vector2 lastMousePos) {
			if (lastPickupObj == null) {
				return;
			}
			Vector2 diff = currentMousePos - lastMousePos;
			lastPickupObj.transform.Rotate(0, diff.x * rotateFactor.Value, 0, Space.Self);
		}
		
		private void HeightHandler(Vector2 currentMousePos, Vector2 lastMousePos) {
			if (lastPickupObj == null) {
				return;
			}
			Vector2 diff = currentMousePos - lastMousePos;
			lastPickupObj.transform.Translate(new Vector3(0, diff.y * heightFactor.Value, 0), Space.Self);
		}
	}

}
