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
		public GameObject holeSpringPrefab;
		public Grid grid;
		public TextAsset colorJson;
		public TextAsset levelJson;
		
		private Dictionary<int, Color> colorPattle = new Dictionary<int, Color>();
		
		public static Level Inst { get; private set; }
		public float stepHeight = 0.2f;
		
		private HashSet<Spring> _springs = new HashSet<Spring>();
		private Dictionary<int, SpringHole> _holes = new Dictionary<int, SpringHole>();
		[Button]
		void Load() {
			try {
				//string pattleJson = File.ReadAllText(Path.Join(Application.persistentDataPath, "color.json"));
				string pattleJson = colorJson.text;
				JsonConvert.DeserializeObject<List<SpringColorPattle>>(pattleJson).ForEach(item => {
					colorPattle[item.type] = item.color;
				});
				
				//string levelJson = File.ReadAllText(Path.Join(Application.persistentDataPath, "level.json"));
				var levelData = JsonConvert.DeserializeObject<LevelData>(levelJson.text);
				int i = 0;
				foreach (var sd in levelData.springs) {
					var s = NewSpring(sd.x0, sd.y0,
						sd.x1, sd.y1,
						sd.heightStep * stepHeight,
						$"spring {i++}",
						sd.type,
						colorPattle[sd.type],
						-1,
						sd.hideWhenCovered);
					s.EnableRender(false);
					s.GetComponent<BoardState>().enabled = true;
					_springs.Add(s);
				}
				for (i = 0; i < levelData.holes.Count; i++) {
					if (levelData.holes[i].types.Count == 0) {
						continue;
					}
					HoleData holeData = levelData.holes[i];
					grid.MakeHole(holeData.x0, holeData.y0);
					SpringHole hole = new SpringHole();
					hole.ID = i;
					int j = 0;
					foreach (var t in holeData.types) {
						var s = NewSpring(holeData.x0, holeData.y0,
							holeData.x1, holeData.y1,
							holeData.heightStep * stepHeight,
							$"hole {i} spring {j++}",
							t,
							colorPattle[t],
							i,
							holeData.hideWhenCovered);
						s.gameObject.SetActive(false);
						s.EnableRender(false);
						hole.AddSpring(s);
					}
					_holes[i] = hole;
				}
				foreach (var entry in _holes) {
					var hole = entry.Value;
					var s = hole.PopSpring();
					s.GetComponent<BoardState>().enabled = true;
					_springs.Add(s);
				}
				Utils.RunNextFrame(() => {
					CalcOverlay();
					foreach (var s in _springs) {
						s.EnableRender(true);
					}
				}, 2);
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
		}
		
		[Button]
		void CalcOverlay(Spring spring = null) {
			if (spring == null) {
				foreach (var s in _springs) {
					s.CalcSpringOverlay();
				}
			}
			else {
				spring.CalcSpringOverlay();
			}

			foreach (var s in _springs) {
				if (!s.IsTop) {
					s.Darker();
				} else {
					s.Lighter();
				}
			}
		}
		
		[Button]
		Spring NewSpring(int x0, int y0, int x1, int y1, float height, string name, int type, Color color, int holeId, bool hideWhenCovered) {
			var springCurve = Instantiate(holeId >= 0 ? holeSpringPrefab : springPrefab);
			springCurve.name = name;
			var pos0 = grid.GetCell(x0, y0).position;
			var pos1 = grid.GetCell(x1, y1).position;
			Spring spring = springCurve.GetComponent<Spring>();
			if (holeId >= 0) {
				var holeSpring = spring.gameObject.GetComponent<HoleSpring>();
				holeSpring.HoleId = holeId;
				spring.HoleSpring = holeSpring;
			}
			spring.Init(pos0, pos1, height, type, hideWhenCovered);
			spring.SetColor(color);
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
			EffectManager.Inst.VibratePickup();
			if (SlotManager.Inst.IsFull() || !spring.IsTop) {
				spring.Shake();
				return;
			}
			spring.EnablePickupCollider(false);
			spring.EnableDetectCollider(false);
			
			if (lastPickupSpring != null && lastPickupSpring.HoleSpring != null) {
				lastPickupSpring.HoleSpring = null;
			}
			
			lastPickupSpring = spring;
			
			RemoveSpring(spring);
			
			if (spring.HoleSpring != null) {
				if (spring.HoleSpring.HoleId >= 0 && _holes[spring.HoleSpring.HoleId].Count > 0) {
					NextSpring(spring);
				}
			}
			
			
			SlotManager.Inst.AddSpring(spring);
		}
		
		void NextSpring(Spring spring) {
			var newSpring = _holes[spring.HoleSpring.HoleId].PopSpring();
			spring.HoleSpring.NextHoleSpring = newSpring;
			newSpring.HoleSpring.PrevHoleSpring = spring;
			newSpring.GetComponent<Hole2BoardState>().enabled = true;
			_springs.Add(newSpring);
			CalcOverlay(newSpring);
		}
		
		[Button]
		public void RestoreLastPickupSpring() {
			if (lastPickupSpring == null) {
				return;
			}
			_springs.Add(lastPickupSpring);
			
			if (lastPickupSpring.LastExtraSlotIndex < 0) {
				if (lastPickupSpring.HoleSpring != null && lastPickupSpring.HoleSpring.NextHoleSpring != null) {
					var nextSpring = lastPickupSpring.HoleSpring.NextHoleSpring;
					nextSpring.EnablePickupCollider(false);
					nextSpring.EnableDetectCollider(false);
					nextSpring.HoleSpring.GoBack = true;
					RemoveSpring(nextSpring);
					_holes[nextSpring.HoleSpring.HoleId].PushSpring(nextSpring);
				}
				CalcOverlay(lastPickupSpring);
			} else {
				lastPickupSpring.ExtraSlotIndex = lastPickupSpring.LastExtraSlotIndex;
				ExtraSlotManager.Inst.RestoreSpring(lastPickupSpring);
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
