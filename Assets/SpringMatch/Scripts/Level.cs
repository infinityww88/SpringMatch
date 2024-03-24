using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace SpringMatch {
	
	public class Level : MonoBehaviour
	{
		public GameObject springPrefab;
		public GameObject springCurveFramePrefab;
		public Grid grid;
		
		private Dictionary<int, Color> colorPattle = new	Dictionary<int, Color>();
		
		private List<Spring> _springs = new List<Spring>();
		[Button]
		void Load() {
			try {
				string pattleJson = File.ReadAllText(Path.Join(Application.persistentDataPath, "color.json"));
				JsonConvert.DeserializeObject<List<SpringColorPattle>>(pattleJson).ForEach(item => {
					colorPattle[item.type] = item.color;
				});
				
				string levelJson = File.ReadAllText(Path.Join(Application.persistentDataPath, "level.json"));
				var levelData = JsonConvert.DeserializeObject<LevelData>(levelJson);
				int i = 0;
				foreach (var sd in levelData.springs) {
					var s = NewSpring(sd.x0, sd.y0, sd.x1, sd.y1, sd.height, $"spring {i++}", sd.type, colorPattle[sd.type]);
					_springs.Add(s);
				}
				Utils.RunNextFrame(() => {
					CalcOverlay();
				});
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
		}
		
		[Button]
		void CalcOverlay() {
			_springs.ForEach(s => {
				s.CalcSpringOverlay();
			});
			_springs.ForEach(s => {
				if (!s.IsTop) {
					s.Darker(0.6f);
				}
				s.EnableRender(true);
			});
			
		}
		
		[Button]
		Spring NewSpring(int x0, int y0, int x1, int y1, float height, string name, int type, Color color) {
			var springCurve = Instantiate(springPrefab);
			springCurve.name = name;
			var pos0 = grid.GetCell(x0, y0).position;
			var pos1 = grid.GetCell(x1, y1).position;
			Spring spring = springCurve.GetComponent<Spring>();
			spring.Init(pos0, pos1, height, type);
			spring.GeneratePickupColliders(0.35f);
			spring.SetColor(color);
			spring.EnableRender(false);
			return spring;
		}
		
		[Button]
		public void OpenFolder() {
			Debug.Log(Application.persistentDataPath);
			System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
		}
	
		// Start is called before the first frame update
		void Awake()
		{
			
		}
	
		public void ReloadScene() {
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Load();
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}
}
