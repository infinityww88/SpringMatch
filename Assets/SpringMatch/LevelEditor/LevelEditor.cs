using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SpringMatch;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;

using Grid = SpringMatch.Grid;

namespace SpringMatchEditor {
	
	public enum EditorState {
		EditSpring,
		DrawSpring
	}
	
	public class LevelEditor : MonoBehaviour
	{
		public Grid grid;
		
		public Color cellHighlightColor;
		public Color cellColor;
		public Spring springPrefab;
		
		[SerializeField]
		private EditorState _editorState;
		
		[SerializeField]
		private Spring _editedSpring;
		
		[SerializeField]
		private float scrollHeightFactor = 1;
		
		[SerializeField]
		private float drawSpringHeightFactor = 0.5f;
		
		[SerializeField]
		private LevelEditorUI editorUI;
		
		[SerializeField]
		[FoldoutGroup("RoateView")]
		private Transform _cameraHPivot;
		[SerializeField]
		[FoldoutGroup("RoateView")]
		private Transform _cameraVPivot;
		[SerializeField]
		[FoldoutGroup("RoateView")]
		private float _cameraRotateHFactor;
		[SerializeField]
		[FoldoutGroup("RoateView")]
		private float _cameraRotateVFactor;
		
		private Dictionary<int, Color> _typeColorPattle = new Dictionary<int, Color>();
		
		public Spring SelectedSpring {
			get {
				return _editedSpring;
			}
			private set {
				_editedSpring = value;
				editorUI.Inspector(_editedSpring);
			}
		}
		
		[SerializeField]
		private Color InvalidColor = Color.white;
		
		private HashSet<Spring> _springs = new HashSet<Spring>();
		
		public Dictionary<int, Color> TypeColorPattle => _typeColorPattle;
		
		public void ForeachSpring(Action<Spring> action) {
			foreach (var s in _springs) {
				action(s);
			}
		}
		
		[Button]
		private LevelData ToLevelData() {
			LevelData ld = new LevelData();
			foreach (var spring in _springs) {
				var es = spring.GetComponent<EditorSpring>();
				if (es.IsHole) {
					HoleData hd = new HoleData();
					hd.x0 = es.pos0.x;
					hd.y0 = es.pos0.y;
					hd.x1 = es.pos1.x;
					hd.y1 = es.pos1.y;
					hd.heightStep = es.heightStep;
					hd.hideWhenCovered = es.HideWhenCovered;
					hd.types.Add(spring.Type);
					hd.types.AddRange(es.HoleSpringTypes);
					ld.holes.Add(hd);
				} else {
					SpringData sd = new SpringData();
					sd.x0 = es.pos0.x;
					sd.y0 = es.pos0.y;
					sd.x1 = es.pos1.x;
					sd.y1 = es.pos1.y;
					sd.type = spring.Type;
					sd.heightStep = es.heightStep;
					sd.hideWhenCovered = es.HideWhenCovered;
					ld.springs.Add(sd);
				}
			}
			return ld;
		}
		
		// Start is called before the first frame update
		void Awake()
		{
			string json = File.ReadAllText(Path.Join(Application.persistentDataPath, "color.json"));
			LoadPattle(json);
		}
		
		public void LoadLevel(string fn) {
			if (string.IsNullOrEmpty(fn)) {
				return;
			}
			var path = Path.Join(Application.persistentDataPath, "Levels", $"{fn}.json");
			if (!File.Exists(path)) {
				return;
			}
			var json = File.ReadAllText(path);
			ClearLevel();
			Load(json);
		}
		
		void LoadPattle(string json) {
			var pattle = JsonConvert.DeserializeObject<List<SpringColorPattle>>(json);
			pattle.ForEach(p => {
				_typeColorPattle[p.type] = p.color;
			});
		}
		
		void Load(string json) {
			_springs.Clear();
			var leveData = JsonConvert.DeserializeObject<LevelData>(json);
			leveData.springs.ForEach(data => {
				PutSpring(data.x0, data.y0, data.x1, data.y1, data.type, data.heightStep, data.hideWhenCovered);
			});
			leveData.holes.ForEach(data => {
				grid.MakeHole(data.x0, data.y0);
				Spring spring = PutSpring(data.x0, data.y0, data.x1, data.y1, data.types[0], data.heightStep, data.hideWhenCovered);
				var es = ES(spring);
				es.IsHole = true;
				es.Add(data.types.Skip(1).ToList());
			});
			CalcOverlay();
		}
		
		[Button]
		public void ClearLevel() {
			_editedSpring = null;
			editorUI.Inspector(null);
			_editorState =	EditorState.DrawSpring;
			foreach (var s in _springs) {
				var es = ES(s);
				if (es.IsHole) {
					grid.ClearHole(es.pos0.x, es.pos0.y);
				}
				Destroy(s.gameObject);
			}
			_springs.Clear();
		}
		
		[Button]
		public string ExportLevel() {
			return JsonConvert.SerializeObject(ToLevelData());
		}
		
		bool pendingCalcOverlay = false;
		
		async UniTask CalcOverlay() {
			if (pendingCalcOverlay) {
				return;
			}
			pendingCalcOverlay = true;
			await UniTask.WaitForSeconds(0.1f);
			foreach (Spring spring in _springs) {
				spring.ClearOverlaySpring();
			}
			foreach (Spring spring in _springs) {
				spring.CalcSpringOverlay();
			}
			foreach (Spring spring in _springs) {
				spring.SetColor(_typeColorPattle[spring.Type]);
				spring.GetComponent<EditorSpring>().IsValid = true;
				if (!spring.IsTop) {
					spring.Darker();
				} else {
					spring.Lighter();
				}
			}
			foreach (Spring s0 in _springs) {
				foreach (Spring s1 in _springs) {
					if (s0 == s1) {
						continue;
					}
					if (s0.CoverdBy(s1) && s1.CoverdBy(s0)) {
						s0.SetColor(InvalidColor);
						s1.SetColor(InvalidColor);
						s0.GetComponent<EditorSpring>().IsValid = false;
						s1.GetComponent<EditorSpring>().IsValid = false;
					}
				}
			}
			pendingCalcOverlay = false;
		}
		
		void SetCellColor(GameObject cell, Color color) {
			if (cell == null) {
				return;
			}
			var renderer = cell.GetComponent<Renderer>();
			renderer.material.SetColor("_BaseColor", color);
		}
		
		async UniTaskVoid DrawSpring() {
			_editorState = EditorState.DrawSpring;
			Vector3 pos = Input.mousePosition;
			Transform startCell = PickupCell(pos);
			if (startCell == null) {
				_editorState = EditorState.EditSpring;
				return;
			}
			SetCellColor(startCell.gameObject, cellHighlightColor);
			Transform currCell = null;
			while (!Input.GetMouseButtonUp(0)) {
				await UniTask.NextFrame();
				var c = PickupCell(Input.mousePosition);
				if (c == null) {
					SetCellColor(currCell?.gameObject, cellColor);
					currCell = null;
					continue;
				}
				else if (c == currCell) {
					continue;
				}
				else if (c == startCell) {
					SetCellColor(currCell?.gameObject, cellColor);
					currCell = null;
					continue;
				}
				SetCellColor(currCell?.gameObject, cellColor);
				SetCellColor(c.gameObject, cellHighlightColor);
				currCell = c;
			}
			SetCellColor(startCell?.gameObject, cellColor);
			SetCellColor(currCell?.gameObject, cellColor);
			if (startCell != null && currCell != null) {
				SelectedSpring = PutSpring(grid.GetCellCoord(startCell), grid.GetCellCoord(currCell), 1, -1, false);
				_editorState = EditorState.EditSpring;
				CalcOverlay();
			}
		}
		
		Spring PutSpring(Vector2Int pos0, Vector2Int pos1, int type, int heightStep, bool hideWhenCovered) {
			return PutSpring(pos0.x, pos0.y, pos1.x, pos1.y, type, heightStep, hideWhenCovered);
		}
		
		Spring PutSpring(int x0, int y0, int x1, int y1, int type, int heightStep, bool hideWhenCovered) {
			Transform startCell = grid.GetCell(x0, y0);
			Transform currCell = grid.GetCell(x1, y1);
			var spring = Instantiate(springPrefab);
			float height = (startCell.position - currCell.position).magnitude * drawSpringHeightFactor;
			if (heightStep >= 0) {
				height = heightStep * scrollHeightFactor;
			} else {
				heightStep = (int)(height / scrollHeightFactor);
				height = heightStep * scrollHeightFactor;
			}
			Debug.Log($"PutSpring {hideWhenCovered}");
			spring.Init(startCell.position, currCell.position, height, type, hideWhenCovered);
			spring.SetColor(TypeColorPattle[type]);
			spring.GeneratePickupColliders(0.35f);
			var editorSpring = spring.gameObject.AddComponent<EditorSpring>();
			editorSpring.pos0 = new Vector2Int(x0, y0);
			editorSpring.pos1 = new Vector2Int(x1, y1);
			editorSpring.heightStep = heightStep;
			editorSpring.HideWhenCovered = hideWhenCovered;
			spring.HideWhenCovered = hideWhenCovered && !editorUI.ViewWithoutHide;
			_springs.Add(spring);
			return spring;
		}
		
		Transform PickupCell(Vector2 mousePos) {
			return Pickup(mousePos, "Cell");
		}
		
		Spring PickupSpring(Vector2 mousePos) {
			var o = Pickup(mousePos, "Pickup");
			return o?.GetComponentInParent<Spring>();
		}
		
		Transform Pickup(Vector2 mousePos, string layer) {
			var hit = Physics.Raycast(Camera.main.ScreenPointToRay(mousePos),
				out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask(layer));
			if (!hit) {
				return null;
			}
			return hitInfo.collider.transform;
		}
		
		private EditorSpring ES(Spring s) => s.GetComponent<EditorSpring>();
		
		public void ChangeSpringType(int type) {
			if (_editedSpring == null) {
				return;
			}
			_editedSpring.Type = type;
			if (ES(_editedSpring).IsValid) {
				_editedSpring.SetColor(_typeColorPattle[type]);
			}
		}
		
		public void ChangeHoleSpringType(int index, int type) {
			if (_editedSpring == null || !ES(_editedSpring).IsHole) {
				return;
			}
			ES(_editedSpring).Set(index, type);
		}
		
		public void MakeHole() {
			if (_editedSpring == null) {
				return;
			}
			var editorSpring = _editedSpring.GetComponent<EditorSpring>();
			editorSpring.IsHole = true;
			grid.MakeHole(editorSpring.pos0.x, editorSpring.pos0.y);
		}
		
		public void ClearHole() {
			if (_editedSpring == null) {
				return;
			}
			var editorSpring = _editedSpring.GetComponent<EditorSpring>();
			editorSpring.IsHole = false;
			grid.ClearHole(editorSpring.pos0.x, editorSpring.pos0.y);
		}
		
		public void OnPickupSpring(Spring spring) {
			_editorState =	EditorState.EditSpring;
			SelectedSpring = spring;
			spring.Shake();
		}
		
		public void SetHeightStep(int stepSteps) {
			var editorSpring = _editedSpring.GetComponent<EditorSpring>();
			editorSpring.heightStep = stepSteps;
			_editedSpring.Init(_editedSpring.Foot0Pos,
				_editedSpring.Foot1Pos,
				editorSpring.heightStep * scrollHeightFactor,
				_editedSpring.Type, false);
			Utils.RunNextFrame(() => {
				_editedSpring.GeneratePickupColliders(0.35f);
			}, 2);
			CalcOverlay();
		}
		
		public void SetHeightStepDelta(int stepDelta) {
			if (_editedSpring == null) {
				return;
			}
			SetHeightStep(_editedSpring.GetComponent<EditorSpring>().heightStep + stepDelta);
		}
		
		async UniTaskVoid RotateView() {
			Vector3 startPos = Input.mousePosition;
			Vector3 vpos = Camera.main.ScreenToViewportPoint(startPos);
			if (vpos.x < 0 || vpos.x > 1 || vpos.y < 0 || vpos.y > 1) {
				return;
			}
			while (!Input.GetMouseButtonUp(0)) {
				await UniTask.NextFrame();
				Vector2 diff = Input.mousePosition - startPos;
				startPos = Input.mousePosition;
				_cameraHPivot.localRotation *= Quaternion.Euler(0, diff.x * _cameraRotateHFactor, 0);
				_cameraVPivot.localRotation *= Quaternion.Euler(-diff.y * _cameraRotateVFactor, 0, 0);
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(0)) {
				var spring = PickupSpring(Input.mousePosition);
				if (spring != null) {
					OnPickupSpring(spring);
					return;
				}
				var cell = PickupCell(Input.mousePosition);
				if (cell != null) {
					DrawSpring();
					return;
				}
				RotateView();
			}
			if (Input.mouseScrollDelta.y != 0 && _editedSpring != null) {
				SetHeightStepDelta((int)Input.mouseScrollDelta.y);
				editorUI.UpdateHeight();
			}
			
			if (Input.GetKeyDown(KeyCode.Backspace)) {
				if (_editedSpring != null) {
					Destroy(_editedSpring.gameObject);
					_springs.Remove(_editedSpring);
					CalcOverlay();
				}
				SelectedSpring = null;
				editorUI.Inspector(null);
			}
			
			if (Input.GetKeyDown(KeyCode.Escape)) {
				_cameraHPivot.localRotation = Quaternion.identity;
				_cameraVPivot.localRotation = Quaternion.identity;
			}
		}
	}
}
