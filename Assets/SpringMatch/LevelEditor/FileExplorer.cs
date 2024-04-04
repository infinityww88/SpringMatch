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
		private const string FILE_NAME_INPUT = "FileNameInput";
		private const string NEW_LEVEL_BUTTON = "NewLevelButton";
		private const string DELETE_LEVEL_BUTTON = "DeleteLevelButton";
		private const string FILE_LIST_VIEW = "FileList";
		
		private ListView _fileListView;
		private TextField _fileNameInput;
		
		private string _levelDataDir;
		
		[SerializeField]
		private LevelEditor _levelEditor;
		
		// Start is called before the first frame update
		void Start()
		{
			CreateLevelDir();
			var root = GetComponent<UIDocument>().rootVisualElement;
			_fileNameInput = root.Q<TextField>(FILE_NAME_INPUT);
			_fileListView = root.Q<ListView>(FILE_LIST_VIEW);
			var newLevelButton = root.Q<Button>(NEW_LEVEL_BUTTON);
			var deleteLevelButton = root.Q<Button>(DELETE_LEVEL_BUTTON);
			newLevelButton.RegisterCallback<ClickEvent>(OnNewLevelClick);
			deleteLevelButton.RegisterCallback<ClickEvent>(OnDeleteLevelClick);
			SetupFileListScrollView();
			root.Q<VisualElement>("FileExplorer").RegisterCallback<ClickEvent>(evt => {
				if (!(evt.target is Label)) {
					_fileListView.ClearSelection();
				}
			});
			_fileNameInput.value = "untitled";
			root.Q<Button>("OpenButton").RegisterCallback<ClickEvent>(evt => {
				System.Diagnostics.Process.Start("explorer.exe", _levelDataDir.Replace("/", "\\"));
			});
			root.Q<Button>("SaveButton").RegisterCallback<ClickEvent>(SaveLevel);
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
