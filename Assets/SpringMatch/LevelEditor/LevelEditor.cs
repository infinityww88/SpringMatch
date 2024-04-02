using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SpringMatch;

using Grid = SpringMatch.Grid;

namespace SpringMatchEditor {
	
	public enum EditorState {
		EditSpring,
		DrawSpring
	}
	
	public class LevelEditor : MonoBehaviour
	{
		public Grid grid;
		
		public Color cellHighlightColor;
		public Color cellColor;
		public Spring springPrefab;
		
		[SerializeField]
		private EditorState _editorState;
		
		[SerializeField]
		private Spring _editedSpring;
		
		[SerializeField]
		private float scrollHeightFactor = 1;
		
		// Start is called before the first frame update
		void Start()
		{
        
		}
		
		void SetCellColor(GameObject cell, Color color) {
			if (cell == null) {
				return;
			}
			var renderer = cell.GetComponent<Renderer>();
			renderer.material.SetColor("_BaseColor", color);
		}
		
		async UniTaskVoid DrawSpring() {
			_editorState = EditorState.DrawSpring;
			Vector3 pos = Input.mousePosition;
			Transform startCell = PickupCell(pos);
			if (startCell == null) {
				_editorState = EditorState.EditSpring;
				return;
			}
			SetCellColor(startCell.gameObject, cellHighlightColor);
			Transform currCell = null;
			while (!Input.GetMouseButtonUp(0)) {
				await UniTask.NextFrame();
				var c = PickupCell(Input.mousePosition);
				if (c == null) {
					SetCellColor(currCell?.gameObject, cellColor);
					currCell = null;
					continue;
				}
				else if (c == currCell) {
					continue;
				}
				else if (c == startCell) {
					SetCellColor(currCell?.gameObject, cellColor);
					currCell = null;
					continue;
				}
				SetCellColor(currCell?.gameObject, cellColor);
				SetCellColor(c.gameObject, cellHighlightColor);
				currCell = c;
			}
			SetCellColor(startCell?.gameObject, cellColor);
			SetCellColor(currCell?.gameObject, cellColor);
			if (startCell != null && currCell != null) {
				var spring = Instantiate(springPrefab);
				spring.Init(startCell.position,
					currCell.position,
					(startCell.position - currCell.position).magnitude / 1.5f,
					1);
				spring.GeneratePickupColliders(0.35f);
				_editedSpring = spring;
			}
			_editorState = EditorState.EditSpring;
		}
		
		Transform PickupCell(Vector2 mousePos) {
			return Pickup(mousePos, "Cell");
		}
		
		Spring PickupSpring(Vector2 mousePos) {
			var o = Pickup(mousePos, "Pickup");
			return o?.GetComponentInParent<Spring>();
		}
		
		Transform Pickup(Vector2 mousePos, string layer) {
			var hit = Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask(layer));
			if (!hit) {
				return null;
			}
			return hitInfo.collider.transform;
		}
		
		public void OnPickupSpring(Spring spring) {
			_editorState =	EditorState.EditSpring;
			_editedSpring = spring;
			spring.Shake();
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(0)) {
				var spring = PickupSpring(Input.mousePosition);
				if (spring == null) {
					DrawSpring();
				}
				else {
					OnPickupSpring(spring);
				}
			}
			if (Input.mouseScrollDelta.y != 0 && _editedSpring != null) {
				_editedSpring.Init(_editedSpring.Foot0Pos,
					_editedSpring.Foot1Pos,
					_editedSpring.Height + Input.mouseScrollDelta.y * scrollHeightFactor,
					_editedSpring.Type);
				Utils.RunNextFrame(() => {
					_editedSpring.GeneratePickupColliders(0.35f);
				}, 2);
			}
		}
	}
}
