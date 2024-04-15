using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine;
using Newtonsoft.Json;

namespace SpringMatch {
	
	public class Grid : MonoBehaviour
	{
		public int Row { get; private set; }
		public int Col { get; private set; }
		
		public float cellSize;
		
		public float Width => Col * cellSize;
		public float Height => Row * cellSize;
		
		public GameObject cellPrefab;

		public Transform GetCell(int x, int y) {
			Assert.IsTrue(x >= 0 && x < Col && y >= 0 && y < Row);
			int index = y * Col + x;
			return transform.GetChild(index);
		}
		
		public Vector2Int GetCellCoord(Transform cell) {
			int index = cell.GetSiblingIndex();
			return new Vector2Int(index % Col, index / Col);
		}
		
		[Button]
		public void MakeHole(int x, int y) {
			var cell = GetCell(x, y);
			cell.GetChild(0).gameObject.SetActive(false);
			cell.GetChild(1).gameObject.SetActive(true);
		}
		
		[Button]
		public void ClearHole(int x, int y) {
			var cell = GetCell(x, y);
			cell.GetChild(0).gameObject.SetActive(true);
			cell.GetChild(1).gameObject.SetActive(false);
		}
		
		public bool showCoord = false;
		
		// OnGUI is called for rendering and handling GUI events.
		protected void OnGUI0()
		{
			if (!showCoord) {
				return;
			}
			GUI.contentColor = Color.red;
			for (int i = 0; i < transform.childCount; i++) {
				var c = transform.GetChild(i);
				var coord = GetCellCoord(transform.GetChild(i));
				var screenPos = Camera.main.WorldToScreenPoint(c.position);
				GUI.Label(new Rect(screenPos.x-10, Screen.height - screenPos.y-10, 200f, 80f), $"{coord.x},{coord.y}");
			}
		}

		[Button]
		public void GenerateGrid(int row, int col) {
			Row = row;
			Col = col;
			while (transform.childCount > 0) {
				var t = transform.GetChild(0);
				t.SetParent(null);
				Destroy(t.gameObject);
			}
			float left = -Width / 2 + cellSize / 2;
			float top = Height / 2 - cellSize / 2;
			
			for (int j = 0; j < Row; j++) {
				for (int i = 0; i < Col; i++) {
					//GameObject o = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, transform);
					GameObject o = (GameObject)Instantiate(cellPrefab, transform);
					o.transform.localPosition = new Vector3(left + cellSize * i, 0, top - cellSize * j);
				}
			}
		}
	}
}

