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
		private LevelEditorUI editorUI;
		
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
		
		private LevelData _leveData;
		
		private HashSet<Spring> _springs = new HashSet<Spring>();
		
		public Dictionary<int, Color> TypeColorPattle => _typeColorPattle;
		
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
					ld.springs.Add(sd);
				}
			}
			Debug.Log(JsonConvert.SerializeObject(ld));
			return ld;
		}
		
		// Start is called before the first frame update
		void Awake()
		{
			string json = File.ReadAllText(Path.Join(Application.persistentDataPath, "color.json"));
			LoadPattle(json);
			json = File.ReadAllText(Path.Join(Application.persistentDataPath, "level.json"));
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
			_leveData = JsonConvert.DeserializeObject<LevelData>(json);
			_leveData.springs.ForEach(data => {
				PutSpring(data.x0, data.y0, data.x1, data.y1, data.type, data.heightStep);
			});
			_leveData.holes.ForEach(data => {
				grid.MakeHole(data.x0, data.y0);
				Spring spring = PutSpring(data.x0, data.y0, data.x1, data.y1, data.types[0], data.heightStep);
				spring.GetComponent<EditorSpring>().Add(data.types.Skip(1).ToList());
			});
			CalcOverlay();
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
				if (!spring.IsTop) {
					spring.Darker();
				} else {
					spring.Lighter();
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
				SelectedSpring = PutSpring(grid.GetCellCoord(startCell), grid.GetCellCoord(currCell), 1, -1);
				_editorState = EditorState.EditSpring;
				CalcOverlay();
			}
		}
		
		Spring PutSpring(Vector2Int pos0, Vector2Int pos1, int type, int heightStep) {
			return PutSpring(pos0.x, pos0.y, pos1.x, pos1.y, type, heightStep);
		}
		
		Spring PutSpring(int x0, int y0, int x1, int y1, int type, int heightStep) {
			Transform startCell = grid.GetCell(x0, y0);
			Transform currCell = grid.GetCell(x1, y1);
			var spring = Instantiate(springPrefab);
			float height = (startCell.position - currCell.position).magnitude / 1.5f;
			if (heightStep >= 0) {
				height = heightStep * scrollHeightFactor;
			} else {
				heightStep = (int)(height / scrollHeightFactor);
				height = heightStep * scrollHeightFactor;
			}
			spring.Init(startCell.position, currCell.position, height, type);
			spring.SetColor(TypeColorPattle[type]);
			spring.GeneratePickupColliders(0.35f);
			var editorSpring = spring.gameObject.AddComponent<EditorSpring>();
			editorSpring.pos0 = new Vector2Int(x0, y0);
			editorSpring.pos1 = new Vector2Int(x1, y1);
			editorSpring.heightStep = heightStep;
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
			var hit = Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask(layer));
			if (!hit) {
				return null;
			}
			return hitInfo.collider.transform;
		}
		
		public void MakeHole() {
			if (_editedSpring == null) {
				return;
			}
			var editorSpring = _editedSpring.GetComponent<EditorSpring>();
			grid.MakeHole(editorSpring.pos0.x, editorSpring.pos0.y);
		}
		
		public void ClearHole() {
			if (_editedSpring == null) {
				return;
			}
			var editorSpring = _editedSpring.GetComponent<EditorSpring>();
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
				_editedSpring.Type);
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

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(0)) {
				var spring = PickupSpring(Input.mousePosition);
				if (spring == null) {
					DrawSpring();
				}
				else {
					OnPickupSpring(spring);
				}
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
		}
	}
}
