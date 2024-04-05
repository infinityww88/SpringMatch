using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpringMatch {
	
	public class Grid : MonoBehaviour
	{
		public int row = 8;
		public int col = 8;
		
		public float width = 10;
		public float height = 10;
		
		public GameObject cellPrefab;
		public GameObject holePrefab;
		
		public Transform GetCell(int x, int y) {
			Assert.IsTrue(x >= 0 && x < col && y >= 0 && y < row);
			int index = y * col + x;
			return transform.GetChild(index);
		}
		
		public Vector2Int GetCellCoord(Transform cell) {
			int index = cell.GetSiblingIndex();
			return new Vector2Int(index % col, index / col);
		}
		
		[Button]
		public void MakeHole(int x, int y) {
			var cell = GetCell(x, y);
			int index = cell.transform.GetSiblingIndex();
			var hole = Instantiate(holePrefab, cell.position, cell.rotation, transform);
			Destroy(cell.gameObject);
			hole.transform.SetSiblingIndex(index);
		}
		
		public void ClearHole(int x, int y) {
			var cell = GetCell(x, y);
			int index = cell.transform.GetSiblingIndex();
			var newCell = Instantiate(cellPrefab, cell.position, cell.rotation, transform);
			Destroy(cell.gameObject);
			newCell.transform.SetSiblingIndex(index);
		}

		#if UNITY_EDITOR
		[Button]
		void GenerateGrid() {
			while (transform.childCount > 0) {
				DestroyImmediate(transform.GetChild(0).gameObject);
			}
			float hstep = width / col;
			float vstep = height / row;
			float left = -width / 2 + hstep / 2;
			float top = height / 2 - vstep / 2;
			
			for (int j = 0; j < row; j++) {
				for (int i = 0; i < col; i++) {
					GameObject o = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, transform);
					
					o.transform.localPosition = new Vector3(left + hstep * i, 0, top - vstep * j);
				}
			}
		}
		#endif
	}
}

