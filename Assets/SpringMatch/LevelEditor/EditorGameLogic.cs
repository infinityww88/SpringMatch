using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpringMatch;
using Newtonsoft.Json;

namespace SpringMatchEditor {
	
	public class EditorGameLogic : MonoBehaviour
	{
		public LevelPrefabConfig levelPrefabs;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Screen.SetResolution(450, 900, FullScreenMode.Windowed);
		}
		
		// Start is called before the first frame update
		void Start()
		{
			LoadLevel();
		}
		
		public void LoadLevel() {
			if (!string.IsNullOrEmpty(LevelEditor.lastLevelFile)) {
				var text = System.IO.File.ReadAllText(LevelEditor.lastLevelFile);
				var data = JsonConvert.DeserializeObject<LevelData>(text);
				var level = Instantiate(levelPrefabs.prefabs[$"{data.row}x{data.col}"]).GetComponent<Level>();
				Level.Inst.LoadData(data);
				Camera.main.transform.position = level.CameraPlayPos;
			}
			
		}
		
		public void ReloadLevel() {
			UnityEngine.SceneManagement.SceneManager.LoadScene(1);
		}
		
		public void Revoke() {
			if (Level.Inst != null) {
				Level.Inst.RestoreLastPickupSpring();
			}
		}
		
		public void Shift3() {
			if (Level.Inst != null) {
				Level.Inst.Shift3ToExtra();
			}
		}
		
		public void Shift1() {
			if (Level.Inst != null) {
				Level.Inst.Shift1ToExtra();
			}
		}
		
		public void Random() {
			if (Level.Inst != null) {
				Level.Inst.RandomAllSpringTypes();
			}
		}
		
		public void BackEditor() {
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
	}
}

