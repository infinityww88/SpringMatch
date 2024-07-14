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
		
		[SerializeField]
		private RectTransform undoButton;
		
		private CancellationTokenSource cancelTokenSource = null;
		
		public enum EEditMode {
			NONE,
			HORZ,
			HEIGHT,
			ROTATE
		}
		
		public EEditMode EditMode = EEditMode.NONE;
		
		private List<SnapHandler> snapHandlers = new List<SnapHandler>();
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			snapHandlers.Add(new SnapHandler(Vector3.back, null, snapLayer, snapDistance, OnSnapCollider, OnNoSnapCollider));
			snapHandlers.Add(new SnapHandler(Vector3.down, null, snapLayer, snapDistance, OnSnapCollider, OnNoSnapCollider));
			snapHandlers.Add(new SnapHandler(Vector3.left, null, snapLayer, snapDistance, OnSnapCollider, OnNoSnapCollider));
		}
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			snapHandlers.ForEach(h => h.OnDrawGizmos());
		}
		
		public void OnSnapCollider(Collider collider) {
			collider.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", snapHintColor);
		}
		
		public void OnNoSnapCollider(Collider collider) {
			collider.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
		}
		
		public void Rotate90Clockwise() {
			if (lastPickupObj == null) {
				return;
			}
			RecordUndo();
			Snap90Degree(lastPickupObj.transform);
			lastPickupObj.transform.eulerAngles -= Vector3.up * 90;
		}
		
		public void Rotate90AntiClockwise() {
			if (lastPickupObj == null) {
				return;
			}
			RecordUndo();
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
		
		public bool IsSnapWall { get; set; } = true;
		
		public void FlipSnapWall() {
			IsSnapWall = !IsSnapWall;
		}
		
		// LateUpdate is called every frame, if the Behaviour is enabled.
		protected void LateUpdate()
		{
			if (IsSnapWall) {
				snapHandlers.ForEach(sh => {
					sh.SnapCollider();
				});
			}
		}
		
		private GameObject lastPickupObj;
		
		private void HintPickupObj(GameObject o) {
			Outline outline = o.GetComponentInParent<Outline>();
			outline.enabled = true;
			outline.OutlineColor = pickupColor;
		}
		
		private void UnhintPickupObj(GameObject o) {
			Outline outline = o.GetComponentInParent<Outline>();
			outline.enabled = false;
		}
		
		private void Pickup(Vector2 pos) {
			Ray ray = Camera.main.ScreenPointToRay(pos);
			var ret = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, pickupLayer);
			if (ret) {
				var o = hitInfo.collider.gameObject;
				HintPickupObj(o);
				
				if (lastPickupObj != null && lastPickupObj != o) {
					UnhintPickupObj(lastPickupObj);
				}
				lastPickupObj = o;
				snapHandlers.ForEach(sh => {
					sh.SetCollider(lastPickupObj.GetComponent<Collider>());
				});
			}
		}
		
		public void Undo() {
			if (undoGameObject != null) {
				//Debug.Log($"{undoGameObject.name} {undoPosition} {undoRotation}");
				undoGameObject.transform.position = undoPosition;
				undoGameObject.transform.rotation = undoRotation;
				if (lastPickupObj != null) {
					UnhintPickupObj(lastPickupObj);
				}
				HintPickupObj(undoGameObject);
				lastPickupObj = undoGameObject;
				undoGameObject = null;
				if (undoButton != null) {
					undoButton.gameObject.SetActive(false);
				}
			}
		}
		
		private GameObject undoGameObject;
		private Vector3 undoPosition;
		private Quaternion undoRotation;
		
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
		
		void RecordUndo() {
			if (undoGameObject != lastPickupObj) {
				undoGameObject = lastPickupObj;
				if (lastPickupObj != null) {
					undoPosition = lastPickupObj.transform.position;
					undoRotation = lastPickupObj.transform.rotation;
					if (undoButton != null) {
						undoButton.gameObject.SetActive(true);
					}
				}
			}
		}
		
		public void Drag(Lean.Touch.LeanFinger finger) {
			if (finger.Down) {
				Pickup(finger.ScreenPosition);
			}
			
			if (finger.ScreenDelta == Vector2.zero) {
				return;
			}
			
			if (EditMode != EEditMode.NONE) {
				RecordUndo();
			}
			
			switch (EditMode) {
			case EEditMode.HORZ:
				HorzHandler(finger.ScreenPosition, finger.LastScreenPosition);
				break;
			case EEditMode.HEIGHT:
				HeightHandler(finger.ScreenPosition, finger.LastScreenPosition);
				break;
			case EEditMode.ROTATE:
				RotateHandler(finger.ScreenPosition, finger.LastScreenPosition);
				break;
			}
		}
	}
}
