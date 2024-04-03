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
	
		private HashSet<Spring> _overlaySpring = new HashSet<Spring>();
	
		[SerializeField]
		private BoxCollider _springCollider;
	
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

		private Renderer _renderer;

		[SerializeField]
		private float _darkFactor = 0.6f;
		private bool _isDark = false;
	
		private Color _color;
	
		public int Type { get; set; }
	
		public bool InTween { get; private set; }
	
		public bool IsTop {
			get {
				return _overlaySpring.Count == 0;
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
	
		public void RemoveOverlaySpring(Spring spring) {
			_overlaySpring.Remove(spring);
			if (IsTop) {
				_SetColor(_color * DarkFactor());
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
	
		public Vector3 Foot0Pos { get; private set; } 
		public Vector3 Foot1Pos { get; private set; }
		public float Height { get; private set; }
		public SpringDeformer Deformer => _springDeformer;
		
		public void SwapFoots() {
			(Foot0Pos, Foot1Pos) = (Foot1Pos, Foot0Pos);
		}
		
		[Button]
		public void SetHeight(float height) {
			Height = height;
			_springDeformer.SetPose(Foot0Pos, Foot1Pos, height);
			GeneratePickupColliders(0.35f);
			Debug.Break();
		}
	
		public void Init(Vector3 pos0, Vector3 pos1, float height, int type) {
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
			Utils.AlignCollider(_springCollider, pos0, pos1, height);
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
			var hitInfos = Physics.BoxCastAll(_springCollider.center + _springCollider.transform.position,
				_springCollider.size / 2,
				Vector3.down,
				_springCollider.transform.rotation,
				Mathf.Infinity,
				LayerMask.GetMask("SpringCollider"),
				QueryTriggerInteraction.Collide);
			for (int i = 0; i < hitInfos.Length; i++) {
				Spring s = hitInfos[i].collider.GetComponentInParent<Spring>();
				if (s == this) {
					continue;
				}
				s.AddOverlaySpring(this);
				Debug.Log($"calc {gameObject.name} cover {s.gameObject.name}, hit {hitInfos[i].point}");
			}
		}
		
		public void EnableRender(bool enabled) {
			_renderer.enabled = enabled;
		}
	
		public void SetColor(Color color) {
			_color = color;
			_SetColor(_color * DarkFactor());
		}
	
		void _SetColor(Color c) {
			_renderer.material.SetColor("_BaseColor", c);
		}
	
		public void Darker() {
			_isDark = true;
			_SetColor(_color * DarkFactor());
		}
		
		public void Lighter() {
			_isDark = false;
			_SetColor(_color * DarkFactor());
		}
		
		public void EnablePickupCollider(bool enabled) {
			_pickupColliderRoot.gameObject.SetActive(enabled);
		}
		
		public void EnableDetectCollider(bool enabled) {
			_springCollider.gameObject.SetActive(enabled);
		}
	
		[Button]
		public void GeneratePickupColliders(float radius) {
			_spline.Refresh();
		
			while (_pickupColliderRoot.childCount > 0) {
				var c = _pickupColliderRoot.GetChild(0);
				GlobalManager.Inst.sphereColliderPool.Release(c.gameObject);
			}
		
			var len = _spline.Length;
			var n = Mathf.Ceil(len / radius);
			for (int i = 0; i < n; i++) {
				var l = i * len / n;
				var tf = _spline.DistanceToTF(l);
				var pos = _spline.Interpolate(tf, Space.World);
				GameObject o = GlobalManager.Inst.sphereColliderPool.Get();
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
	
		void Awake()
		{
			_spline.OnInitialized.AddListener(evt => {
				Debug.Log($"spring refresh {Time.frameCount}");
			});
			_renderer = GetComponentInChildren<Renderer>();
		}
	}
}
