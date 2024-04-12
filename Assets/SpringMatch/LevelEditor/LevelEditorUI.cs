using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Linq;
using SpringMatch;
using System;

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
		[SerializeField]
		private VisualTreeAsset typeButtonTemplate;
		
		[MyUTKElementAttr("TypeButtonGroup")]
		private VisualElement typeButtonGroup;

		[MyUTKElementAttr("HeightInputField")]
		private TextField heightInputField;
		
		[MyUTKElementAttr("HideWhenCovered")]
		private Toggle hideWhenCoveredToggle;
		
		[MyUTKElementAttr("ViewWithoutHide")]
		private Toggle viewWithoutHideToggle;
		
		[MyUTKElementAttr("HoleToggle")]
		private Toggle holeToggle;
		
		[MyUTKElementAttr("AddSpringButton")]
		private VisualElement addHoleSpringButton;
		
		[MyUTKElementAttr("RemoveSpringButton")]
		private VisualElement removeHoleSpringButton;
		
		[MyUTKElementAttr("HoleSprings")]
		private VisualElement holeSpringGroup;
		
		[MyUTKElementAttr("HoleInspector")]
		private VisualElement holeInspector;
		
		private VisualElement selectedHoleSpringButton;
		private VisualElement selectedTypeButton;
		
		private const string CSS_BUTTON_SELECT = "button-select";
		
		[SerializeField]
		private LevelEditor levelEditor;
		
		public bool ViewWithoutHide => viewWithoutHideToggle.value;
		
		[Button]
		void Init() {
			Utils.InitUTK(this);
		}
		
		#region start
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Utils.InitUTK(this);
			
			SetupTypeButtonGroup();
			SetupHeightInputField();
			SetupHideWhenCoveredToggle();
			SetupViewWithoutHideToggle();
			SetupHoleToggle();
			SetupAddHoleSpringButton();
			SetupRemoveHoleSpringButton();
			SetupHoleSpringGroup();
		}
		#endregion
		
		#region setup
		void SetupTypeButtonGroup() {
			foreach (var i in levelEditor.TypeColorPattle) {
				var t = i.Key;
				var color = i.Value;
				
				var temp = typeButtonTemplate.Instantiate();
				VisualElement typeButton = temp[0];
				typeButton.userData = t;
				typeButton.style.backgroundColor = color;
				typeButtonGroup.Add(typeButton);
				typeButton.RegisterCallback<ClickEvent>(OnTypeButtonClick);
			}
		}
		
		void SetupHeightInputField() {
			heightInputField.RegisterCallback<ChangeEvent<string>>(OnHeightChange);
		}
		
		void SetupHoleToggle() {
			holeToggle.RegisterCallback<ChangeEvent<bool>>(OnHoleToggleChange);
		}
		
		void SetupHideWhenCoveredToggle() {
			hideWhenCoveredToggle.RegisterCallback<ChangeEvent<bool>>(OnHideWhenCoveredToggleChange);
		}
		
		void SetupViewWithoutHideToggle() {
			viewWithoutHideToggle.RegisterCallback<ChangeEvent<bool>>(OnViewWithoutHideToggleChange);
		}
		
		void SetupAddHoleSpringButton() {
			addHoleSpringButton.RegisterCallback<ClickEvent>(OnAddHoleSpringButtonClick);
		}
		
		void SetupRemoveHoleSpringButton() {
			removeHoleSpringButton.RegisterCallback<ClickEvent>(OnRemoveHoleSpringButtonClick);
		}
		
		void SetupHoleSpringGroup() {
			
		}
		
		#endregion
	
		#region callback
		
		void OnHideWhenCoveredToggleChange(ChangeEvent<bool> evt) {
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			var es = levelEditor.SelectedSpring.GetComponent<EditorSpring>();
			es.HideWhenCovered = evt.newValue;
			levelEditor.SelectedSpring.HideWhenCovered = es.HideWhenCovered && !viewWithoutHideToggle.value;
		}
		
		void OnViewWithoutHideToggleChange(ChangeEvent<bool> evt) {
			levelEditor.ForeachSpring(s => {
				var es = s.GetComponent<EditorSpring>();
				s.HideWhenCovered = es.HideWhenCovered && !viewWithoutHideToggle.value;
			});
		}
		
		void OnHeightChange(ChangeEvent<string> evt) {
			int height = 0;
			int.TryParse(evt.newValue, out height);
			levelEditor.SetHeightStep(height);
		}
		
		void OnHoleToggleChange(ChangeEvent<bool> evt) {
			DisplayStyle display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
			holeInspector.style.display = display;
			if (evt.newValue == false) {
				selectedHoleSpringButton = null;
			}
			if (evt.newValue == true) {
				levelEditor.MakeHole();
			} else {
				levelEditor.ClearHole();
				holeSpringGroup.Clear();
			}
		}
		
		void OnTypeButtonClick(ClickEvent evt) {
			VisualElement e = (VisualElement)evt.target;
			SelectTypeButton(e);
			Debug.Log($"type button {e.userData} click");
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			int type = (int)e.userData;
			if (selectedHoleSpringButton != null) {
				selectedHoleSpringButton.style.backgroundColor = e.style.backgroundColor;
				int index = selectedHoleSpringButton.parent.IndexOf(selectedHoleSpringButton);
				Debug.Log($"set hole spring button {index} => {type}");
				levelEditor.ChangeHoleSpringType(index, type);
			}
			else {
				levelEditor.ChangeSpringType(type);
			}
		}
		
		void OnAddHoleSpringButtonClick(ClickEvent evt) {
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			AddHoleSpringButton(1);
			levelEditor.SelectedSpring.GetComponent<EditorSpring>().Add(1);
		}
		
		void OnRemoveHoleSpringButtonClick(ClickEvent evt) {
			if (selectedHoleSpringButton != null) {
				int index = selectedHoleSpringButton.parent.IndexOf(selectedHoleSpringButton);
				levelEditor.SelectedSpring.GetComponent<EditorSpring>().Remove(index);
				holeSpringGroup.Remove(selectedHoleSpringButton);
				selectedHoleSpringButton = null;
				return;
			}
			if (holeSpringGroup.childCount == 0) {
				return;
			}
			levelEditor.SelectedSpring.GetComponent<EditorSpring>().Remove(holeSpringGroup.childCount-1);
			holeSpringGroup.Remove(holeSpringGroup.Children().Last());
		}
		
		void OnHoleSpringButtonClick(ClickEvent evt) {
			SelectHoleSpringButton((VisualElement)(evt.target));
		}
		
		#endregion
		
		void SelectTypeButton(VisualElement button) {
			selectedTypeButton?.RemoveFromClassList(CSS_BUTTON_SELECT);
			selectedTypeButton = button;
			button?.AddToClassList(CSS_BUTTON_SELECT);
		}
		
		void SelectHoleSpringButton(VisualElement button) {
			selectedHoleSpringButton?.RemoveFromClassList(CSS_BUTTON_SELECT);
			selectedHoleSpringButton = button;
			button?.AddToClassList(CSS_BUTTON_SELECT);
		}
		
		VisualElement AddHoleSpringButton(int type) {
			var temp = typeButtonTemplate.Instantiate();
			VisualElement typeButton = temp[0];
			typeButton.RegisterCallback<ClickEvent>(OnHoleSpringButtonClick);
			holeSpringGroup.Add(typeButton);
			typeButton.style.backgroundColor = levelEditor.TypeColorPattle[type];
			return typeButton;
		}
		
		[Button]
		void ResetUI() { 
			SelectTypeButton(null);
			SelectHoleSpringButton(null);
			holeSpringGroup.Clear();
			holeToggle.SetValueWithoutNotify(false);
			hideWhenCoveredToggle.SetValueWithoutNotify(false);
			holeInspector.style.display = DisplayStyle.None;
			heightInputField.SetValueWithoutNotify("0");
		}
		
		public void UpdateHeight() {
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			heightInputField.SetValueWithoutNotify(
				levelEditor.SelectedSpring.GetComponent<EditorSpring>().heightStep.ToString());
		}
		
		public void Inspector(Spring spring) {
			ResetUI();
			if (spring == null) {
				return;
			}
			heightInputField.SetValueWithoutNotify(spring.GetComponent<EditorSpring>().heightStep.ToString());
			var editorSpring = spring.GetComponent<EditorSpring>();
			if (editorSpring.IsHole) {
				holeInspector.style.display = DisplayStyle.Flex;
				holeToggle.SetValueWithoutNotify(true);
				foreach (var t in editorSpring.HoleSpringTypes) {
					AddHoleSpringButton(t);
				}
			} else {
				holeToggle.SetValueWithoutNotify(false);
			}
			hideWhenCoveredToggle.SetValueWithoutNotify(editorSpring.HideWhenCovered);
		}
	}

}
