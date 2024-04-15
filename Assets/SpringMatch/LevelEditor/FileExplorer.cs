using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpringMatch;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using UnityEngine.Events;

namespace SpringMatchEditor {
	
	public class MyLabel : Label {}
	
	public class FileExplorer : MonoBehaviour
	{
		[MyUTKElementAttr("RowInput")]
		private TextField rowInputField;
		
		[MyUTKElementAttr("ColInput")]
		private TextField colInputField;
		
		[MyUTKElementAttr("NewLevelButton")]
		private Button newLevelButton;
		[MyUTKElementAttr("DeleteLevelButton")]
		private Button deleteLevelButton;
		[MyUTKElementAttr("FileList")]
		private ListView _fileListView;
		[MyUTKElementAttr("FileNameInput")]
		private TextField _fileNameInput;
		
		[MyUTKElementAttr("OpenButton")]
		private Button openButton;
		[MyUTKElementAttr("SaveButton")]
		private Button saveButton;
		
		private string _levelDataDir;
		
		[SerializeField]
		private LevelEditor _levelEditor;
		
		// Start is called before the first frame update
		void Start()
		{
			CreateLevelDir();
			Utils.InitUTK(this);
			var root = GetComponent<UIDocument>().rootVisualElement;
			newLevelButton.RegisterCallback<ClickEvent>(OnNewLevelClick);
			deleteLevelButton.RegisterCallback<ClickEvent>(OnDeleteLevelClick);
			SetupFileListScrollView();
			root.Q<VisualElement>("FileExplorer").RegisterCallback<ClickEvent>(evt => {
				if (!(evt.target is Label)) {
					_fileListView.ClearSelection();
				}
			});
			_fileNameInput.value = "untitled";
			openButton.RegisterCallback<ClickEvent>(evt => {
				System.Diagnostics.Process.Start("explorer.exe", _levelDataDir.Replace("/", "\\"));
			});
			saveButton.RegisterCallback<ClickEvent>(SaveLevel);
			
			SetupGridSizeInput();
		}
		
		void SetupGridSizeInput() {
			rowInputField.RegisterCallback<ChangeEvent<string>>(OnGridSizeChange);
			colInputField.RegisterCallback<ChangeEvent<string>>(OnGridSizeChange);
		}
				
		void OnGridSizeChange(ChangeEvent<string> evt) {
			int v = 0;
			int.TryParse(evt.newValue, out v);
			var e = (TextField)evt.target;
			v = Mathf.Max(6, Mathf.Min(15, v));
			e.SetValueWithoutNotify($"{v}");
		}
		
		void CreateLevelDir() {
			_levelDataDir = Path.Join(Application.persistentDataPath, "Levels");
			if (Directory.Exists(_levelDataDir)) {
				return;
			}
			Directory.CreateDirectory(_levelDataDir);
		}

		
		void OnNewLevelClick(ClickEvent evt) {
			_fileNameInput.value = "untitled";
			_levelEditor.ClearLevel();
			Debug.Log($"{rowInputField.value} {colInputField.value}");
			var row = int.Parse(rowInputField.text);
			var col = int.Parse(colInputField.text);
			_levelEditor.NewLevel(row, col);
		}
		
		void OnDeleteLevelClick(ClickEvent evt) {
			var selectedItem = _fileListView.selectedItem;
			if (selectedItem != null) {
				string fn = (string)selectedItem;
				string path = Path.Join(_levelDataDir, $"{fn}.json");
				File.Delete(path);
				_levelEditor.ClearLevel();
				PopulateFileList();
			}
		}
		
		void PopulateFileList() {
			var files = Directory.GetFiles(_levelDataDir, "*.json")
				.Select(e => Path.GetFileNameWithoutExtension(e)).ToArray();
			_fileListView.itemsSource = files;
		}
		
		void OnSelectionChange(IEnumerable<object> selection) {
			if (selection.Count() == 0) {
				return;
			}
			_fileNameInput.value = (string)selection.First();
			_levelEditor.LoadLevel(_fileNameInput.value);
		}

		void SetupFileListScrollView() {
			_fileListView.RegisterCallback<ClickEvent>(evt => {
			});
			_fileListView.makeItem = () => new MyLabel();
			_fileListView.bindItem = (e, i) => {
				Label l = (Label)e;
				l.RegisterCallback<ClickEvent>(evt => {
				});
				l.AddToClassList("file-list-item");
				l.text = (string)_fileListView.itemsSource[i];
				e.userData = i;
			};
			_fileListView.Q<ScrollView>().mouseWheelScrollSize = 500;
			_fileListView.onSelectionChange += OnSelectionChange;
			PopulateFileList();
		}
		
		void ClearLevel() {
			_levelEditor.ClearLevel();
		}
		
		void SaveLevel(ClickEvent evt) {
			if (string.IsNullOrEmpty(_fileNameInput.value)) {
				return;
			}
			var path = Path.Join(_levelDataDir, $"{_fileNameInput.value}.json");
			File.WriteAllText(path,
				_levelEditor.ExportLevel());
			PopulateFileList();
		}
	}

}
