using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

namespace SpringMatch {
	
	public class Grid : MonoBehaviour
	{
		public int Row { get; private set; }
		public int Col { get; private set; }
		
		public float cellSize;
		
		public float Width => Col * cellSize;
		public float Height => Row * cellSize;
		
		public GameObject cellPrefab;
		
		[SerializeField]
		private GameObject _numInfoPrefab;

		private RectTransform _numInfoRoot;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			_numInfoRoot = GameObject.FindGameObjectWithTag("NumInfoRoot").GetComponent<RectTransform>();
		}

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
		public void MakeHole(int x, int y, int followNum) {
			var cell = GetCell(x, y);
			cell.GetChild(0).gameObject.SetActive(false);
			cell.GetChild(1).gameObject.SetActive(true);
			var c = cell.GetComponent<Cell>();
			if (c.NumInfo == null) {
				var numInfo = Instantiate(_numInfoPrefab, _numInfoRoot);
				c.NumInfo = numInfo;
			}
			
			c.SetNum(followNum + 1);
			c.SetNumInfoPos(_numInfoRoot);
		}
		
		[Button]
		public void ClearHole(int x, int y) {
			var cell = GetCell(x, y);
			cell.GetChild(0).gameObject.SetActive(true);
			cell.GetChild(1).gameObject.SetActive(false);
			var c = cell.GetComponent<Cell>();
			Destroy(c.NumInfo);
			c.NumInfo = null;
		}
		
		public bool updateCellNumInfo = false;
		
		public void Update() {
			if (!updateCellNumInfo) {
				return;
			}
			for (int i = 0; i < transform.childCount; i++) {
				var c = transform.GetChild(i).GetComponent<Cell>();
				if (c.NumInfo != null) {
					c.SetNumInfoPos(_numInfoRoot);
				}
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

