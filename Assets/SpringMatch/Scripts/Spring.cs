﻿using System.Collections;
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
	private SpringCurveFrame _springCurve;
	
	[SerializeField]
	private Transform _pickupColliderRoot;
	
	[SerializeField]
	private GameObject _pickupSlotCollider;
	
	[SerializeField]
	private CurvySpline _spline;
	
	[SerializeField]
	private float _darkFactor = 0.8f;
	
	private int _type;
	private Color _color;
	
	public int Type => _type;
	
	public bool InTween { get; private set; }
	
	public bool IsTop {
		get {
			return _overlaySpring.Count == 0;
		}
	}
	
	#region test
	[Button]
	void TestAddSpring() {
		SlotManager.Inst.AddSpring(this);
	}
	[Button]
	void TestMoveSpring(Transform target) {
		TweenToSlot(target.position, CancellationToken.None);
	}
	#endregion
	
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
	
	public void RemoveOverlaySpring(Spring spring) {
		_overlaySpring.Remove(spring);
		if (IsTop) {
			_SetColor(_color);
		}
	}
	
	public void AddOverlaySpring(Spring spring) {
		_overlaySpring.Add(spring);
	}
	
	public Vector3 Foot0Pos { get; private set; }
	public Vector3 Foot1Pos { get; private set; }
	public float Height { get; private set; }
	
	public void Init(Vector3 pos0, Vector3 pos1, float height, int type) {
		_type = type;
		_springCurve.SetFrame(pos0, pos1, height);
		Foot0Pos = pos0;
		Foot1Pos = pos1;
		Height = height;
		TargetSlotIndex = SlotIndex = EliminateIndex = ExtraSlotIndex = EliminateTargetSlotIndex = -1;
		_pickupSlotCollider.transform.position = _springCurve.head.position;
		Utils.AlignCollider(_springCollider, pos0, pos1, height);
	}
	
	public void CalcSpringOverlay() {
		Debug.Log($"calc overlay {gameObject.name} {_overlaySpring.Count}");
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
	
	public void EnableRender(bool active) {
		foreach (var r in GetComponentsInChildren<MeshRenderer>()) {
			r.enabled = active;
		}
	}
	
	public void SetColor(Color color) {
		_color = color;
		_SetColor(_color);
	}
	
	void _SetColor(Color c) {
		foreach (var s in GetComponentsInChildren<SphereCollider>()) {
			s.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", c);
		}
	}
	
	public void Darker(float f) {
		_SetColor(_color * f);
	}
	
	void OnCurveRefresh(CurvySplineEventArgs args) {
		Debug.Log("Spline Refresh");
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
	
	public async UniTask TweenToSlot(Vector3 pos, CancellationToken ct) {
		_springCollider.gameObject.SetActive(false);
		_pickupColliderRoot.gameObject.SetActive(false);
		_pickupSlotCollider.SetActive(true);
		_pickupSlotCollider.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", _color);
		await _pickupSlotCollider.transform.DOMove(pos, 0.5f).WithCancellation(ct);
	}
	
	public async UniTask TweenToGrid(Vector3 pos0, Vector3 pos1, float height, CancellationToken ct) {
		_springCollider.gameObject.SetActive(true);
		await _pickupSlotCollider.transform.DOMove((pos0 + pos1)/2 + Vector3.up * height, 0.5f).WithCancellation(ct);
		_pickupColliderRoot.gameObject.SetActive(true);
		_pickupSlotCollider.SetActive(false);
	}
	
	public async UniTask TweenToExtra(Vector3 pos0, Vector3 pos1, float height, CancellationToken ct) {
		await _pickupSlotCollider.transform.DOMove((pos0 + pos1)/2, 0.5f).WithCancellation(ct);
		_pickupSlotCollider.SetActive(true);
	}
	
	public void SetExtra(int extraIndex, Vector3 foot0Pos, Vector3 foot1Pos, float height) {
		Foot0Pos = foot0Pos;
		Foot1Pos = foot1Pos;
		Height = height;
		_springCurve.SetFrame(foot0Pos, foot1Pos, height);
		//Init(Foot0Pos, Foot1Pos, Height, Type);
		//_springCollider.gameObject.SetActive(false);
		//GeneratePickupColliders(0.35f);
		
		ExtraSlotIndex = extraIndex;
	}
	
	// This function is called when the MonoBehaviour will be destroyed.
	protected void OnDestroy()
	{
		
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    _spline.OnInitialized.AddListener(evt => {
	    	Debug.Log($"spring refresh {Time.frameCount}");
	    });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

}
