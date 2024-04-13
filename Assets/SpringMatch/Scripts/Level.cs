﻿using System.Collections;
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
		
		[SerializeField]
		private Transform _springsRoot;
		
		public Grid grid;
		//public TextAsset colorJson;
		//public TextAsset levelJson;
		
		public static Level Inst { get; private set; }
		public float stepHeight = 0.2f;
		
		private HashSet<Spring> _springs = new HashSet<Spring>();
		private Dictionary<int, SpringHole> _holes = new Dictionary<int, SpringHole>();
		
		private bool _done = false;
		
		public void Done() {
			_done = true;
			GetComponentInChildren<Pickup>().enabled = false;
		}
		
		IEnumerator<ValueTuple<Color, int>> CreateColorTypeGenerator(List<ColorNums> colorNums) {
			for (int i = 0; i < colorNums.Count; i++) {
				yield return ValueTuple.Create(colorNums[i].color, i);
			}
		}
		
		public void RandomType(List<Spring> spring) {
			for (int i = 1; i < spring.Count; i++) {
				int j = UnityEngine.Random.Range(i, spring.Count);
				(spring[i-1].Type, spring[j].Type) = (spring[j].Type, spring[i-1].Type);
				(spring[i-1].Color, spring[j].Color) = (spring[j].Color, spring[i-1].Color);
			}
		}
		
		public void RandomAllSpringTypes() {
			List<Spring> t = new List<Spring>();
			t.AddRange(_springs);
			foreach (var e in _holes) {
				SpringHole hole = e.Value;
				hole.ForeachSpring(s => t.Add(s));
			}
			RandomType(t);
		}

		public void Load() {
			try {				
				string levelJson = File.ReadAllText(Path.Join(Application.persistentDataPath, "level.json"));
				var levelData = JsonConvert.DeserializeObject<LevelData>(levelJson);
				
				grid.GenerateGrid(levelData.row, levelData.col);
				var colorTypeEnumerator = CreateColorTypeGenerator(levelData.colorNums);
				int i = 0;
				int holeId = 0;
				foreach (var sd in levelData.springs) {
					colorTypeEnumerator.MoveNext();
					var colorType = colorTypeEnumerator.Current;
					var s = NewSpring(sd.x0, sd.y0,
						sd.x1, sd.y1,
						sd.heightStep * stepHeight,
						$"spring {i++}",
						colorType.Item2,
						colorType.Item1,
						sd.IsHole ? holeId : -1,
						sd.hideWhenCovered);
						
					s.transform.SetParent(_springsRoot);
					s.EnableRender(false);
					
					if (sd.IsHole) {
						SpringHole hole = new SpringHole();
						hole.ID = holeId++;
						s.gameObject.SetActive(false);
						hole.AddSpring(s);
						
						grid.MakeHole(sd.x0, sd.y0);
						
						s.gameObject.name = $"hole {hole.ID} spring 0";
						
						for (int j = 0; j < sd.followNum; j++) {
							colorTypeEnumerator.MoveNext();
							colorType = colorTypeEnumerator.Current;
							s = NewSpring(sd.x0, sd.y0,
								sd.x1, sd.y1,
								sd.heightStep * stepHeight,
								$"hole {hole.ID} spring {j+1}",
								colorType.Item2,
								colorType.Item1,
								hole.ID,
								sd.hideWhenCovered);
							s.transform.SetParent(_springsRoot);
							s.EnableRender(false);
							s.gameObject.SetActive(false);
							hole.AddSpring(s);
						}
						_holes[hole.ID] = hole;
					}
					else {
						s.GetComponent<BoardState>().enabled = true;
						_springs.Add(s);
					}
				}
				
				RandomAllSpringTypes();
				
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
			spring.GridPos0 = new	Vector2Int(x0, y0);
			spring.GridPos1 = new	Vector2Int(x1, y1);
			spring.Init(pos0, pos1, height, type, hideWhenCovered);
			spring.Color = color;
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
			if (!_springs.Contains(spring) && !ExtraSlotManager.Inst.Contains(spring)) {
				return;
			}
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
			var pos0 = grid.GetCell(newSpring.GridPos0.x, newSpring.GridPos0.y).position;
			var pos1 = grid.GetCell(newSpring.GridPos1.x, newSpring.GridPos1.y).position;
			newSpring.Init(pos0, pos1, newSpring.Height, newSpring.Type, newSpring.HideWhenCovered);
			newSpring.Deformer.Shrink(pos0);
			_springs.Add(newSpring);
			CalcOverlay(newSpring);
		}
		
		[Button]
		public void RestoreLastPickupSpring() {
			if (lastPickupSpring == null) {
				return;
			}
			
			if (lastPickupSpring.LastExtraSlotIndex < 0) {
				_springs.Add(lastPickupSpring);
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
			foreach (var s in springs) {
				s.HoleSpring = null;
			}
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
		
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
			if (Inst == this) {
				Inst = null;
			}
		}
	}
}
