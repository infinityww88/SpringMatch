using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Linq;
using SpringMatch;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace SpringMatchEditor {
	
	[AttributeUsage(AttributeTargets.Field, Inherited = true)]
	public class MyUTKElementAttr : Attribute {
		public string FieldName { get; set; }
		public MyUTKElementAttr(string fieldName) {
			FieldName = fieldName;
		}
	} 
	
	public class LevelEditorUI : MonoBehaviour
	{	
		[MyUTKElementAttr("ColorNumGroup")]
		private VisualElement colorNumGroup;

		[MyUTKElementAttr("HeightInputField")]
		private TextField heightInputField;
		
		[MyUTKElementAttr("LevelNumberInputField")]
		private TextField levelNumberInputField;
		
		[MyUTKElementAttr("HideWhenCovered")]
		private Toggle hideWhenCoveredToggle;
		
		//[MyUTKElementAttr("ViewWithoutHide")]
		//private Toggle viewWithoutHideToggle;
		
		[MyUTKElementAttr("HoleNumInputField")]
		private TextField holeNumInputField;
		
		[MyUTKElementAttr("NewLevelButton")]
		private Button newLevelButton;
		
		[MyUTKElementAttr("PlayButton")]
		private Button playButton;
		
		[MyUTKElementAttr("OpenButton")]
		private Button openButton;
		
		[MyUTKElementAttr("SaveButton")]
		private Button saveButton;
		
		[MyUTKElementAttr("LoadButton")]
		private Button loadButton;
		
		[MyUTKElementAttr("RandomColorButton")]
		private Button randomColorButton;
		
		[MyUTKElementAttr("ResetViewButton")]
		private Button resetViewButton;
		
		[MyUTKElementAttr("SaveViewButton")]
		private Button saveViewButton;
		
		[MyUTKElementAttr("NumInfo")]
		private Label numInfo;
		
		[MyUTKElementAttr("RowInput")]
		private TextField rowInputField;
		
		[MyUTKElementAttr("ColInput")]
		private TextField colInputField;
		
		[SerializeField]
		private LevelEditor levelEditor;
		
		private DialogUI _dialog;
		
		//public bool ViewWithoutHide => viewWithoutHideToggle.value;
		
		[Button]
		void Init() {
			Utils.InitUTK(this);
		}
		
		#region start
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		IEnumerator Start()
		{
			Utils.InitUTK(this);
			_dialog = GetComponent<DialogUI>();
			yield return UniTask.WaitUntil(() => levelEditor.ColorNums.Count > 0).ToCoroutine();
			SetupColorNums();
			SetupHeightInputField();
			SetupHideWhenCoveredToggle();
			//SetupViewWithoutHideToggle();
			SetupHoleNumInputField();
			SetupGridSizeInput();
			newLevelButton.RegisterCallback<ClickEvent>(OnNewLevelButtonClick);
			openButton.RegisterCallback<ClickEvent>(evt => {
				System.Diagnostics.Process.Start("explorer.exe", Path.GetFullPath("."));
			});
			saveButton.RegisterCallback<ClickEvent>(SaveLevel);
			randomColorButton.RegisterCallback<ClickEvent>(OnRandomColorClick);
			playButton.RegisterCallback<ClickEvent>(OnPlay);
			loadButton.RegisterCallback<ClickEvent>(OnLoad);
			resetViewButton.RegisterCallback<ClickEvent>(OnResetView);
			saveViewButton.RegisterCallback<ClickEvent>(OnSaveView);
		}
		#endregion
		
		public void UpdateNumInfo() {
			numInfo.text = $"{levelEditor.TotalSpringNum()} / {levelEditor.TotalColorNum()}";
		}
		
		public void SetInputLevelNumber(string number) {
			levelNumberInputField.SetValueWithoutNotify(number);
		}
		
		string GetInputLevelNumber() {
			return levelNumberInputField.value.Trim();
		}
		
		void OnLoad(ClickEvent evt) {
			if (GetInputLevelNumber() == "") {
				_dialog.Show($"<color=red>No Level Number</color>");
			}
			LevelEditor.Inst.LoadLevel($"level_{GetInputLevelNumber()}.json");
		}
		
		void SaveLevel(ClickEvent evt) {
			if (GetInputLevelNumber() == "") {
				_dialog.Show($"<color=red>No Level Number</color>");
			}
			else if (levelEditor.TotalSpringNum() > levelEditor.TotalColorNum()) {
				_dialog.Show($"<color=red>Spring number ({levelEditor.TotalSpringNum()}) > total color number ({levelEditor.TotalColorNum()}</color>)");
			}
			else {
				string levelFile = $"level_{GetInputLevelNumber()}.json";
				LevelEditor.CurrEditLevel = levelFile;
				var path = Path.GetFullPath(levelFile);
				File.WriteAllText(path,
					levelEditor.ExportLevel());
				_dialog.Show("<color=green>Level saved.</color>");
			}
			
		}
		
		public void UpdateColorNum() {
			for (int i = 0; i < colorNumGroup.childCount; i++) {
				var tf = (TextField)colorNumGroup[i];
				int idx = (int)tf.userData;
				var cn = levelEditor.ColorNums[idx];
				tf.SetValueWithoutNotify($"{cn.num}");
			}
		}
		
		#region setup
		
		void SetupGridSizeInput() {
			rowInputField.RegisterCallback<ChangeEvent<string>>(OnGridSizeChange);
			colInputField.RegisterCallback<ChangeEvent<string>>(OnGridSizeChange);
		}
		
		void SetupHoleNumInputField() {
			holeNumInputField.RegisterCallback<ChangeEvent<String>>(OnHoleNumChange);
		}
		
		void SetupColorNums() {
			for(int i = 0; i < levelEditor.ColorNums.Count; i++) {
				var cn = levelEditor.ColorNums[i];
				TextField tf = new	TextField();
				tf.userData = i;
				tf.Q(className: "unity-text-field__input").style.backgroundColor = cn.color;
				tf.AddToClassList("color-num-input");
				tf.SetValueWithoutNotify(levelEditor.ColorNums[i].num.ToString());
				tf.RegisterCallback<ChangeEvent<string>>(OnColorNumChange);
				colorNumGroup.Add(tf);
			}
		}
		
		void SetupHeightInputField() {
			heightInputField.RegisterCallback<ChangeEvent<string>>(OnHeightChange);
		}
		
		void SetupHideWhenCoveredToggle() {
			hideWhenCoveredToggle.RegisterCallback<ChangeEvent<bool>>(OnHideWhenCoveredToggleChange);
		}
		
		/*
		void SetupViewWithoutHideToggle() {
			viewWithoutHideToggle.RegisterCallback<ChangeEvent<bool>>(OnViewWithoutHideToggleChange);
		}
		*/
		
		#endregion
	
		#region callback
		
		public void LoadCameraView() {
			var path = Path.GetFullPath("cameraConfig.json");
			if (!File.Exists(path)) {
				return;
			}
			var cameraConfig = JsonUtility.FromJson<CameraConfig>(File.ReadAllText(path));
			Camera.main.transform.localRotation = cameraConfig.cameraRotation;
			Camera.main.transform.parent.localRotation = cameraConfig.VertRotation;
			Camera.main.transform.parent.parent.localRotation = cameraConfig.HorzRotation;
			Camera.main.transform.position = cameraConfig.cameraPosition;
		}
		
		void OnResetView(ClickEvent evt) {
			LoadCameraView();
		}
		
		void OnSaveView(ClickEvent evt) {
			SpringMatch.CameraConfig cameraConfig = new CameraConfig();
			cameraConfig.cameraPosition = Camera.main.transform.position;
			cameraConfig.cameraRotation = Camera.main.transform.localRotation;
			cameraConfig.VertRotation = Camera.main.transform.parent.localRotation;
			cameraConfig.HorzRotation = Camera.main.transform.parent.parent.localRotation;
			var text = JsonUtility.ToJson(cameraConfig);
			Debug.Log($"camera config {text}");
			File.WriteAllText(Path.GetFullPath("cameraConfig.json"), text);
		}
		
		void OnPlay(ClickEvent evt) {
			if (LevelEditor.CurrEditLevel == "") {
				_dialog.Show("<color=red>Save Level First</color>");
				return;
			}
			UnityEngine.SceneManagement.SceneManager.LoadScene(1);
		}
		
		void OnRandomColorClick(ClickEvent evt) {
			if (levelEditor.TotalSpringNum() > levelEditor.TotalColorNum()) {
				_dialog.Show($"<color=red>Spring number ({levelEditor.TotalSpringNum()}) > total color number ({levelEditor.TotalColorNum()}</color>)");
			}
			else {
				levelEditor.RandomColor();
			}
		}
		
		void OnGridSizeChange(ChangeEvent<string> evt) {
			int v = 0;
			int.TryParse(evt.newValue, out v);
			var e = (TextField)evt.target;
			//v = Mathf.Max(6, Mathf.Min(15, v));
			e.SetValueWithoutNotify($"{v}");
		}
		
		void OnNewLevelButtonClick(ClickEvent evt) {
			int row = int.Parse(rowInputField.value);
			int col = int.Parse(colInputField.value);
			levelEditor.NewLevel(row, col);
		}
		
		void OnHoleNumChange(ChangeEvent<string> evt) {
			int num = 0;
			int.TryParse(evt.newValue, out num);
			num = Mathf.Clamp(num, 0, 100);
			var e = (TextField)evt.target;
			e.SetValueWithoutNotify($"{num}");
			levelEditor.SetHoleSpringNum(num);
			UpdateNumInfo();
		}
		
		void OnColorNumChange(ChangeEvent<string> evt) {
			int num = 0;
			int.TryParse(evt.newValue, out num);
			num = Mathf.Clamp(num, 0, 100);
			var e = (TextField)evt.target;
			var index = (int)(e.userData);
			e.SetValueWithoutNotify($"{num}");
			levelEditor.SetColorNum(index, num);
			UpdateNumInfo();
		}
		
		void OnHideWhenCoveredToggleChange(ChangeEvent<bool> evt) {
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			levelEditor.SelectedSpring.HideWhenCovered = evt.newValue;
		}
		
		/*
		void OnViewWithoutHideToggleChange(ChangeEvent<bool> evt) {
			levelEditor.ForeachSpring(s => {
				var es = s.GetComponent<EditorSpring>();
				s.HideWhenCovered = es.HideWhenCovered && !viewWithoutHideToggle.value;
				Debug.Log($">> es.hidewhencovered {es.HideWhenCovered} {viewWithoutHideToggle.value} {s.HideWhenCovered}");
			});
		}
		*/
		
		void OnHeightChange(ChangeEvent<string> evt) {
			int height = 0;
			int.TryParse(evt.newValue, out height);
			levelEditor.SetHeightStep(height);
		}
		
		#endregion
		
		public void UpdateHeight() {
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			heightInputField.SetValueWithoutNotify(
				levelEditor.SelectedSpring.GetComponent<EditorSpring>().heightStep.ToString());
		}
		
		public void Inspector(Spring spring) {
			if (spring == null) {
				return;
			}
			heightInputField.SetValueWithoutNotify(spring.GetComponent<EditorSpring>().heightStep.ToString());
			var editorSpring = spring.GetComponent<EditorSpring>();
			holeNumInputField.SetValueWithoutNotify($"{editorSpring.followNum}");
			hideWhenCoveredToggle.SetValueWithoutNotify(spring.HideWhenCovered);
		}
	}

}
