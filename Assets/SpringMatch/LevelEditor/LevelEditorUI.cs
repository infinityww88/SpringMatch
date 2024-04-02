using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Linq;

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
		
		[SerializeField]
		private List<Color> types = new List<Color>();
		
		private VisualElement selectedHoleSpringButton = null;
		private VisualElement selectedTypeButton = null;
		
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
			int t = 0;
			types.ForEach(c => {
				var temp = typeButtonTemplate.Instantiate();
				VisualElement typeButton = temp[0];
				typeButton.userData = t++;
				typeButton.style.backgroundColor = c;
				typeButtonGroup.Add(typeButton);
				typeButton.RegisterCallback<ClickEvent>(OnTypeButtonClick);
			});
		}
		
		void SetupHeightInputField() {
			
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
		
		void OnHoleToggleChange(ChangeEvent<bool> evt) {
			DisplayStyle display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
			holeInspector.style.display = display;
			if (evt.newValue == false) {
				selectedHoleSpringButton = null;
			}
		}
		
		void OnTypeButtonClick(ClickEvent evt) {
			VisualElement e = (VisualElement)evt.target;
			SelectTypeButton(e);
			Debug.Log($"type button {e.userData} click");
			if (selectedHoleSpringButton != null) {
				selectedHoleSpringButton.style.backgroundColor = e.style.backgroundColor;
			}
		}
		
		void OnAddHoleSpringButtonClick(ClickEvent evt) {
			var temp = typeButtonTemplate.Instantiate();
			VisualElement typeButton = temp[0];
			typeButton.RegisterCallback<ClickEvent>(OnHoleSpringButtonClick);
			holeSpringGroup.Add(typeButton);
		}
		
		void OnRemoveHoleSpringButtonClick(ClickEvent evt) {
			if (selectedHoleSpringButton != null) {
				holeSpringGroup.Remove(selectedHoleSpringButton);
				selectedHoleSpringButton = null;
				return;
			}
			if (holeSpringGroup.childCount == 0) {
				return;
			}
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
	
	}

}
