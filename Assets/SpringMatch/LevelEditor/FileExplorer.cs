using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpringMatch;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

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
					Debug.Log("Cancel Selection");
					_fileListView.ClearSelection();
				}
			});
			root.Q<Button>("OpenButton").RegisterCallback<ClickEvent>(evt => {
				System.Diagnostics.Process.Start("explorer.exe", _levelDataDir.Replace("/", "\\"));
			});
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
		}
		
		void OnDeleteLevelClick(ClickEvent evt) {
			var selectedItem = _fileListView.selectedItem;
			if (selectedItem != null) {
				string fn = (string)selectedItem;
				string path = Path.Join(_levelDataDir, $"{fn}.json");
				File.Delete(path);
				PopulateFileList();
			}
		}
		
		void PopulateFileList() {
			var files = Directory.GetFiles(_levelDataDir, "*.json")
				.Select(e => Path.GetFileNameWithoutExtension(e)).ToArray();
			_fileListView.itemsSource = files;
		}
		
		void OnSelectionChange(IEnumerable<object> selection) {
			_fileNameInput.value = (string)selection.FirstOrDefault();
		}

		void SetupFileListScrollView() {
			_fileListView.RegisterCallback<ClickEvent>(evt => {
				Debug.Log($"Click list view {evt.target} {evt.currentTarget}");
			});
			_fileListView.makeItem = () => new MyLabel();
			_fileListView.bindItem = (e, i) => {
				Label l = (Label)e;
				l.RegisterCallback<ClickEvent>(evt => {
					Debug.Log($"Click label {evt.target} {evt.currentTarget}");
				});
				l.AddToClassList("file-list-item");
				l.text = (string)_fileListView.itemsSource[i];
				e.userData = i;
			};
			_fileListView.Q<ScrollView>().mouseWheelScrollSize = 500;
			_fileListView.onSelectionChange += OnSelectionChange;
			PopulateFileList();
		}
	}

}
