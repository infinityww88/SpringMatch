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
		public SpringConfig config;
		
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
		
		[SerializeField]
		private float scrollViewFactor = 1f;
		
		public bool InteractPending { get; set; }
		
		public static string CurrEditLevel = "";
		
		private List<Color> _colors;
		
		private List<ColorNums> _colorNumsA = new List<ColorNums>();
		private List<ColorNums> _colorNumsB = new List<ColorNums>();
		
		public List<ColorNums> ColorNumsA => _colorNumsA;
		public List<ColorNums> ColorNumsB => _colorNumsB;
		
		public static LevelEditor Inst;
		
		public Spring SelectedSpring {
			get {
				return _editedSpring;
			}
			private set {
				_editedSpring = value;
				editorUI.Inspector(_editedSpring);
			}
		}
		
		public int TotalAreaANum() {
			return _springs.Select(e => e.AreaID == 0 ? 1 : 0).Sum();
		}
		
		public int TotalAreaBNum() {
			return _springs.Select(e => e.AreaID == 1 ? 1 : 0).Sum();
		}
		
		public int TotalColorANum() {
			int sum = 0;
			for (int i = 0; i < _colorNumsA.Count; i++) {
				sum += _colorNumsA[i].num;
			}
			return sum;
		}
		
		public int TotalColorBNum() {
			int sum = 0;
			for (int i = 0; i < _colorNumsB.Count; i++) {
				sum += _colorNumsB[i].num;
			}
			return sum;
		}
		
		public int TotalSpringNum() {
			return _springs.Select(e => ES(e).followNum + 1).Sum();
		}
		
		[SerializeField]
		private Color InvalidColor = Color.white;
		
		private HashSet<Spring> _springs = new HashSet<Spring>();
		
		public void ForeachSpring(Action<Spring> action) {
			foreach (var s in _springs) {
				action(s);
			}
		}
		
		public void LoadColors() {
			_colorNumsA.Clear();
			foreach (var c in _colors) {
				_colorNumsA.Add(new ColorNums {
					color = c,
					num = 0
				});
			}
			_colorNumsB.Clear();
			foreach (var c in _colors) {
				_colorNumsB.Add(new ColorNums {
					color = c,
					num = 0
				});
			}
			editorUI.UpdateNumInfo();
			editorUI.UpdateColorNum();
		}
		
		public void NewLevel(int row, int col) {
			CurrEditLevel = "";
			ClearLevel();
			grid.GenerateGrid(row, col);
			LoadColors();
		}
		
		[Button]
		private LevelData ToLevelData() {
			LevelData ld = new LevelData();
			foreach (var spring in _springs) {
				var es = spring.GetComponent<EditorSpring>();
				SpringData sd = new SpringData();
				sd.x0 = spring.GridPos0.x;
				sd.y0 = spring.GridPos0.y;
				sd.x1 = spring.GridPos1.x;
				sd.y1 = spring.GridPos1.y;
				sd.area = spring.AreaID;
				sd.followNum = ES(spring).followNum;
				sd.heightStep = es.heightStep;
				sd.hideWhenCovered = spring.HideWhenCovered;
				ld.springs.Add(sd);
			}
			ld.colorNumsA = _colorNumsA;
			ld.colorNumsB = _colorNumsB;
			ld.row = grid.Row;
			ld.col = grid.Col;
			return ld;
		}
		
		public void SetColorNum(int areaId, int index, int num) {
			if (areaId == 0) {
				_colorNumsA[index].num = num;
			}
			else {
				_colorNumsB[index].num = num;
			}
		}
		
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			string json = "";
			string path = Path.GetFullPath("color.json");
			try {
				json = File.ReadAllText(path);
				_colors = JsonConvert.DeserializeObject<List<Color>>(json, new ColorConvert());
			}
			catch (Exception e) {
				json = "[\"#AA0000\",\"#00AA00\",\"#004BAA\",\"#A40B88\"]";
				File.WriteAllText(path, json);
				_colors = JsonConvert.DeserializeObject<List<Color>>(json, new ColorConvert());
			}
	
			Screen.SetResolution(660, 720, FullScreenMode.Windowed);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			LoadLevel(CurrEditLevel);
			//editorUI.LoadCameraView();
		}
		
		/*
		public void RandomColor() {
			List<ValueTuple<Color, int>> colorTypes = new	List<ValueTuple<Color, int>>();
			int i = 0;
			int t = 0;
			foreach (var cn in _colorNums) {
				for (i = 0; i < cn.num; i++) {
					colorTypes.Add(ValueTuple.Create(cn.color, t));
				}
				t++;
			}
			for (i = 1; i < colorTypes.Count; i++) {
				int j = UnityEngine.Random.Range(i, colorTypes.Count);
				(colorTypes[i-1], colorTypes[j]) = (colorTypes[j], colorTypes[i-1]);
			}
			i = 0;
			foreach (var s in _springs) {
				s.Type = colorTypes[i].Item2;
				s.Color = colorTypes[i].Item1;
				i++;
			}
		}
		*/
		
		public void LoadLevel(string fn) {
			if (string.IsNullOrEmpty(fn) || !File.Exists(Path.GetFullPath($"{fn}"))) {
				Debug.Log("New Leve 6x6");
				NewLevel(13, 10);
				return;
			}
			
			editorUI.SetInputLevelNumber(fn.Substring(6, fn.Length - 6 - 5));
			var json = File.ReadAllText(Path.GetFullPath($"{fn}"));
			ClearLevel();
			Load(json);
		}
		
		void Load(string json) {
			_springs.Clear();
			
			var levelData = JsonConvert.DeserializeObject<LevelData>(json);
			
			_colorNumsA = levelData.colorNumsA;
			_colorNumsB = levelData.colorNumsB;
			editorUI.UpdateColorNum();
			
			grid.GenerateGrid(levelData.row, levelData.col);
			levelData.springs.ForEach(data => {
				int idx = UnityEngine.Random.Range(0, _colors.Count);
				PutSpring(data.x0, data.y0, data.x1, data.y1, idx, data.heightStep, data.hideWhenCovered, data.followNum, data.area);
				if (data.followNum > 0) {
					grid.MakeHole(data.x0, data.y0, data.followNum);
				}
			});
			CalcOverlay();
			editorUI.UpdateNumInfo();
			editorUI.UpdateAreaNum();
			//RandomColor();
		}
		
		[Button]
		public void ClearLevel() {
			_editedSpring = null;
			editorUI.Inspector(null);
			_editorState =	EditorState.DrawSpring;
			foreach (var s in _springs) {
				if (ES(s).followNum > 0) {
					grid.ClearHole(s.GridPos0.x, s.GridPos0.y);
				}
				Destroy(s.gameObject);
			}
			_springs.Clear();
		}
		
		[Button]
		public string ExportLevel() {
			return JsonConvert.SerializeObject(ToLevelData(), Formatting.Indented);
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
				spring.Color = spring.AreaID == 0 ? new Color(0.8f, 0, 0) : new Color(0.8f, 0, 0.8f); //_colors[spring.Type];
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
						s0.Color = InvalidColor;
						s1.Color = InvalidColor;
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
			var renderer = cell.GetComponentInChildren<Renderer>();
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
				SelectedSpring = PutSpring(grid.GetCellCoord(startCell), grid.GetCellCoord(currCell), 0, -1, false, 0, editorUI.CurrAreaID);
				_editorState = EditorState.EditSpring;
				CalcOverlay();
			}
		}
		
		Spring PutSpring(Vector2Int pos0, Vector2Int pos1, int type, int heightStep, bool hideWhenCovered, int followNum, int areaId) {
			return PutSpring(pos0.x, pos0.y, pos1.x, pos1.y, type, heightStep, hideWhenCovered, followNum, areaId);
		}
		
		Spring PutSpring(int x0, int y0, int x1, int y1, int type, int heightStep, bool hideWhenCovered, int followNum, int areaId) {
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
			spring.GridPos0 = new	Vector2Int(x0, y0);
			spring.GridPos1 = new	Vector2Int(x1, y1);
			spring.Init(startCell.position, currCell.position, height, type, hideWhenCovered, areaId);
			spring.Type = type;
			spring.Color = areaId == 0 ? new Color(1, 0, 0) : new Color(1, 0, 1) ; //_colors[type];
			spring.GeneratePickupColliders(spring.Config.colliderRadius);
			var editorSpring = spring.gameObject.AddComponent<EditorSpring>();
			editorSpring.heightStep = heightStep;
			editorSpring.followNum = followNum;
			//editorSpring.HideWhenCovered = hideWhenCovered;
			spring.HideWhenCovered = hideWhenCovered; // && !editorUI.ViewWithoutHide;
			_springs.Add(spring);
			editorUI.UpdateNumInfo();
			editorUI.UpdateAreaNum();
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
			return hitInfo.collider.transform.parent;
		}
		
		private EditorSpring ES(Spring s) => s.GetComponent<EditorSpring>();
		
		
		public void SetHoleSpringNum(int followNum) {
			if (_editedSpring == null) {
				return;
			}
			var es = ES(_editedSpring);
			if (followNum > 0) {
				MakeHole(followNum);
			} else {
				ClearHole();
			}
			ES(_editedSpring).followNum = followNum;
		}
		
		public void MakeHole(int followNum) {
			if (_editedSpring == null) {
				return;
			}
			grid.MakeHole(_editedSpring.GridPos0.x, _editedSpring.GridPos0.y, followNum);
		}
		
		public void ClearHole() {
			if (_editedSpring == null) {
				return;
			}
			grid.ClearHole(_editedSpring.GridPos0.x, _editedSpring.GridPos0.y);
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
				_editedSpring.Type,
				_editedSpring.HideWhenCovered,
				_editedSpring.AreaID);
			Utils.RunNextFrame(() => {
				_editedSpring.GeneratePickupColliders(config.colliderRadius);
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
			if (InteractPending) {
				return;
			}
			
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
			if (Input.mouseScrollDelta.y != 0) {
				if (Input.GetKey(KeyCode.LeftControl)) {
					var offset = scrollViewFactor * Input.mouseScrollDelta.y;
					Camera.main.transform.Translate(-Camera.main.transform.forward * offset, Space.World);
				}
				else if (_editedSpring != null) {
					SetHeightStepDelta((int)Input.mouseScrollDelta.y);
					editorUI.UpdateHeight();
				}
			}
			
			if (Input.GetKeyDown(KeyCode.Backspace)) {
				if (_editedSpring != null) {
					if (ES(_editedSpring).IsHole) {
						grid.ClearHole(_editedSpring.GridPos0.x, _editedSpring.GridPos0.y);
					}
					Destroy(_editedSpring.gameObject);
					_springs.Remove(_editedSpring);
					CalcOverlay();
					editorUI.UpdateNumInfo();
					editorUI.UpdateAreaNum();
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
