using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace SpringMatch {
	
	public class Level : MonoBehaviour
	{
		public GameObject springPrefab;
		public GameObject springCurveFramePrefab;
		public Grid grid;
		
		private Dictionary<int, Color> colorPattle = new Dictionary<int, Color>();
		
		public static Level Inst { get; private set; }
		
		private HashSet<Spring> _springs = new HashSet<Spring>();
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
					s.EnableRender(false);
					_springs.Add(s);
				}
				Utils.RunNextFrame(() => {
					CalcOverlay();
				}, 2);
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
		}
		
		[Button]
		void CalcOverlay() {
			foreach (var s in _springs) {
				s.CalcSpringOverlay();
			}

			foreach (var s in _springs) {
				if (!s.IsTop) {
					s.Darker(0.6f);
				}
				s.EnableRender(true);
			}
		}
		
		[Button]
		Spring NewSpring(int x0, int y0, int x1, int y1, float height, string name, int type, Color color) {
			var springCurve = Instantiate(springPrefab);
			springCurve.name = name;
			var pos0 = grid.GetCell(x0, y0).position;
			var pos1 = grid.GetCell(x1, y1).position;
			Spring spring = springCurve.GetComponent<Spring>();
			spring.Init(pos0, pos1, height, type);
			spring.SetColor(color);
			spring.GetComponent<BoardState>().enabled = true;
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
			Inst = this;
		}
		
		private Spring lastPickupSpring = null;
		
		public void ClearLastPickupSpring() {
			lastPickupSpring = null;
		}
		
		public void OnPickupSpring(Spring spring) {
			if (!spring.IsTop) {
				spring.Shake();
				return;
			}
			spring.EnablePickupCollider(false);
			
			lastPickupSpring = spring;
			
			RemoveSpring(spring);
			SlotManager.Inst.AddSpring(spring);
		}
		
		[Button]
		public void RestoreLastPickupSpring() {
			if (lastPickupSpring == null) {
				return;
			}
			_springs.Add(lastPickupSpring);
			
			if (lastPickupSpring.LastExtraSlotIndex < 0) {
				CalcOverlay();
			} else {
				lastPickupSpring.ExtraSlotIndex = lastPickupSpring.LastExtraSlotIndex;
			}
			
			SlotManager.Inst.RemoveSpring(lastPickupSpring.TargetSlotIndex);
			lastPickupSpring.TargetSlotIndex = -1;
			
			lastPickupSpring = null;
		}
		
		[Button]
		public void Shift3ToExtra() {
			if (SlotManager.Inst.UsedSlotsNum == 0 || !ExtraSlotManager.Inst.Available()) {
				return;
			}
			lastPickupSpring = null;
			var springs = SlotManager.Inst.ShiftOutString(3);
			ExtraSlotManager.Inst.AddSprings(springs);
		}
		
		public void RemoveSpring(Spring spring) {
			foreach (var s in _springs) {
				s.RemoveOverlaySpring(spring);
			}
			_springs.Remove(spring);
		}
	
		public void ReloadScene() {
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Load();
		}
	}
}
