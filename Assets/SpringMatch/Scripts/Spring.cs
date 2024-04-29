using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FluffyUnderware.Curvy;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace SpringMatch {
	public class Spring : MonoBehaviour
	{
		public enum EState {
			Board,
			Board2Slot,
			Slot,
			Slot2Slot,
			Slot2Extra,
			Extra,
			Extra2Slot
		}
	
		public EState State { get; set; }
	
		[SerializeField]
		private HashSet<Spring> _overlaySpring = new HashSet<Spring>();
		
		[SerializeField]
		[InlineEditor]
		private SpringConfig _springConfig;
		
		public SpringConfig Config => _springConfig;
	
		[SerializeField]
		private CapsuleCollider _springCollider;
	
		[SerializeField]
		private SpringDeformer _springDeformer;
	
		[SerializeField]
		private SpringShake _springShake;
		
		[SerializeField]
		private SpringBinder _springBinder;
	
		[SerializeField]
		private Transform _pickupColliderRoot;
	
		[SerializeField]
		private CurvySpline _spline;

		[SerializeField]
		private Renderer _renderer;
		
		const string POOL_KEY = "sphereCollider";

		[SerializeField]
		private float _darkFactor = 0.6f;
		private bool _isDark = false;
		
		public Vector2Int GridPos0 { get; set; }
		public Vector2Int GridPos1 { get; set; }
	
		private Color _color;
		
		private int _type = 0;
	
		public int Type { get; set; }
	
		public bool InTween { get; private set; }
	
		public bool IsTop {
			get {
				return _overlaySpring.Count == 0;
			}
		}
		
		public Color _hideColor = Color.gray;
		
		[SerializeField]
		private bool _hideWhenCovered = false;
		
		public bool HideWhenCovered {
			get {
				return _hideWhenCovered;
			}
			set {
				_hideWhenCovered = value;
				_SetColor();
			}
		}
		
		public Color Color {
			get {
				return _color;
			}
			set {
				_color = value;
				_SetColor();
			}
		}
		
		public HoleSpring HoleSpring { get; set; }
		
		[ShowInInspector]
		public int TargetSlotIndex { get; set; }
		[ShowInInspector]
		public int SlotIndex { get; set; }
		public bool IsReachSlot => SlotIndex == TargetSlotIndex;
	
		[ShowInInspector]
		public int EliminateIndex { get; set; }
		public int EliminateTargetSlotIndex { get; set; }
		public Spring EliminateCompanySpring0 { get; set; }
		public Spring EliminateCompanySpring1 { get; set; }
	
		public int ExtraSlotIndex { get; set; }
		public int LastExtraSlotIndex { get; set; }
		
		public bool End { get; set; }
	
		public int AreaID { get; set; }
	
		public void RemoveOverlaySpring(Spring spring) {
			_overlaySpring.Remove(spring);
			if (IsTop) {
				Lighter();
				_SetColor();
			}
		}
		
		private float DarkFactor() {
			return _isDark ? _darkFactor : 1f;
		}
		
		public void ClearOverlaySpring() {
			_overlaySpring.Clear();
		}
	
		public void AddOverlaySpring(Spring spring) {
			_overlaySpring.Add(spring);
		}
	
		public Vector3 Foot0Pos { get; set; } 
		public Vector3 Foot1Pos { get; set; }
		public float Height { get; private set; }
		public SpringDeformer Deformer => _springDeformer;
		
		public void SwapFoots() {
			(Foot0Pos, Foot1Pos) = (Foot1Pos, Foot0Pos);
		}
		
		[Button]
		public void SetHeight(float height) {
			Height = height;
			_springDeformer.SetPose(Foot0Pos, Foot1Pos, height);
			GeneratePickupColliders(_springConfig.colliderRadius);
			Debug.Break();
		}
	
		public void Init(Vector3 pos0, Vector3 pos1, float height, int type, bool hideWhenCovered, int areaId) {
			End = false;
			Type = type;
			Foot0Pos = pos0;
			Foot1Pos = pos1;
			Height = height;
			_springDeformer.SetPose(pos0, pos1, height);
			TargetSlotIndex = -1;
			SlotIndex = -1;
			EliminateIndex = -1;
			ExtraSlotIndex = -1;
			LastExtraSlotIndex = -1;
			EliminateTargetSlotIndex = -1;
			HideWhenCovered = hideWhenCovered;
			AreaID = areaId;
			Utils.AlignCollider(_springCollider, pos0, pos1, height, Config.colliderLengthOffset);
		}
		
		public void Shake() {
			_springBinder.enabled = false;
			_springShake.Shake(() => {
				_springBinder.enabled = true;
			});
		}
		
		public bool CoverdBy(Spring other) {
			return _overlaySpring.Contains(other);
		}
	
		public void CalcSpringOverlay() {
			float r = _springCollider.height / 2 - _springCollider.radius;
			var hitInfos = Physics.CapsuleCastAll(_springCollider.transform.position + _springCollider.transform.right * r,
				_springCollider.transform.position - _springCollider.transform.right * r,
				_springCollider.radius,
				Vector3.down,
				Mathf.Infinity,
				LayerMask.GetMask("SpringCollider"),
				QueryTriggerInteraction.Collide);
			for (int i = 0; i < hitInfos.Length; i++) {
				Spring s = hitInfos[i].collider.GetComponentInParent<Spring>();
				if (s == this) {
					continue;
				}
				s.AddOverlaySpring(this);
			}
		}
		
		public void EnableRender(bool enabled) {
			_renderer.enabled = enabled;
		}
		
		Color FinalColor() {
			return (HideWhenCovered && !IsTop) ? _hideColor : _color;
		}
	
		void _SetColor() {
			var c = FinalColor() * DarkFactor();
			_renderer.material.SetColor("_BaseColor", c);
		}
	
		public void Darker() {
			_isDark = true;
			_SetColor();
		}
		
		public void Lighter() {
			_isDark = false;
			_SetColor();
		}
		
		public void EnablePickupCollider(bool enabled) {
			_pickupColliderRoot.gameObject.SetActive(enabled);
		}
		
		public void EnableDetectCollider(bool enabled) {
			_springCollider.gameObject.SetActive(enabled);
		}
		
		void ReleasePickupCollider() {
			while (_pickupColliderRoot.childCount > 0) {
				var c = _pickupColliderRoot.GetChild(0);
				GameObjectsPool.Inst?.Release(POOL_KEY, c.gameObject);
			}
		}
	
		[Button]
		public void GeneratePickupColliders(float radius) {
			_spline.Refresh();
		
			ReleasePickupCollider();
		
			var len = _spline.Length;
			var n = Mathf.Ceil(len / radius);
			for (int i = 0; i < n; i++) {
				var l = i * len / n;
				var tf = _spline.DistanceToTF(l);
				var pos = _spline.Interpolate(tf, Space.World);
				GameObject o = GameObjectsPool.Inst.Get(POOL_KEY);
				o.transform.position = pos;
				o.transform.SetParent(_pickupColliderRoot, true);
				o.transform.localScale = Vector3.one * radius * 2;
			}
		}
	
		public void ToExtra(int extraIndex, Vector3 foot0Pos, Vector3 foot1Pos) {
			Foot0Pos = foot0Pos;
			Foot1Pos = foot1Pos;
			Height = (foot0Pos - foot1Pos).magnitude/2;
		
			ExtraSlotIndex = extraIndex;
			LastExtraSlotIndex = extraIndex;
		}
		
		public void OnReplaceModel(GameObject model, int lodIndex) {
			_renderer = model.GetComponentInChildren<Renderer>();
			_SetColor();
			_springBinder.BonesRoot = model.transform.GetChild(0);
			_springBinder.initLen = _springConfig.springInitLength[lodIndex];
			_springShake.BonesRoot = model.transform.GetChild(0);
		}
	
		void Awake()
		{
			//_renderer = GetComponentInChildren<Renderer>();
		}
		
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
			if (GameObjectsPool.Inst != null) {
				ReleasePickupCollider();
			}
			_overlaySpring.Clear();
		}
	}
}
