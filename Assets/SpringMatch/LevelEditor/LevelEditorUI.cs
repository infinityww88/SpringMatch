using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Linq;
using SpringMatch;

namespace SpringMatchEditor {
	
	public class LevelEditorUI : MonoBehaviour
	{
		[SerializeField]
		private VisualTreeAsset typeButtonTemplate;
		
		private VisualElement root;
		private VisualElement typeButtonGroup;
		private IntegerField heightInputField;
		private Toggle holeToggle;
		private VisualElement addHoleSpringButton;
		private VisualElement removeHoleSpringButton;
		private VisualElement holeSpringGroup;
		private VisualElement holeInspector;
		
		private const string typeButtonGroupName = "TypeButtonGroup";
		private const string heightInputFieldName = "HeightInputField";
		private const string holeToggleName = "HoleToggle";
		private const string addHoleSpringButtonName = "AddSpringButton";
		private const string removeHoleSpringButtonName = "RemoveSpringButton";
		private const string holeSpringGroupName = "HoleSprings";
		private const string holeInspectorName = "HoleInspector";
		
		private const string CSS_BUTTON_SELECT = "button-select";
		
		private VisualElement selectedHoleSpringButton = null;
		private VisualElement selectedTypeButton = null;
		
		[SerializeField]
		private LevelEditor levelEditor;
		
		#region start
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			root = GetComponent<UIDocument>().rootVisualElement;
			
			typeButtonGroup = root.Q(typeButtonGroupName);
			heightInputField = root.Q<IntegerField>(heightInputFieldName);
			holeToggle = root.Q<Toggle>(holeToggleName);
			addHoleSpringButton = root.Q(addHoleSpringButtonName);
			removeHoleSpringButton = root.Q(removeHoleSpringButtonName);
			holeSpringGroup = root.Q(holeSpringGroupName);
			holeInspector = root.Q(holeInspectorName);
			
			Assert.IsNotNull(typeButtonGroup);
			Assert.IsNotNull(heightInputField);
			Assert.IsNotNull(holeToggle);
			Assert.IsNotNull(addHoleSpringButton);
			Assert.IsNotNull(removeHoleSpringButton);
			Assert.IsNotNull(holeSpringGroup);
			Assert.IsNotNull(holeInspector);
			
			SetupTypeButtonGroup();
			SetupHeightInputField();
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
			heightInputField.RegisterCallback<ChangeEvent<int>>(OnHeightChange);
		}
		
		void SetupHoleToggle() {
			holeToggle.RegisterCallback<ChangeEvent<bool>>(OnHoleToggleChange);
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
		
		void OnHeightChange(ChangeEvent<int> evt) {
			levelEditor.SetHeightStep(evt.newValue);
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
			holeInspector.style.display = DisplayStyle.None;
			heightInputField.SetValueWithoutNotify(0);
		}
		
		public void UpdateHeight() {
			if (levelEditor.SelectedSpring == null) {
				return;
			}
			heightInputField.SetValueWithoutNotify(
				levelEditor.SelectedSpring.GetComponent<EditorSpring>().heightStep);
		}
		
		public void Inspector(Spring spring) {
			ResetUI();
			if (spring == null) {
				return;
			}
			heightInputField.SetValueWithoutNotify(spring.GetComponent<EditorSpring>().heightStep);
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
		}
	}

}
