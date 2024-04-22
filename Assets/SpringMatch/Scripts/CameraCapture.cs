﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

public class CaptureCamera : MonoBehaviour
{
	public RenderTexture rt;
	public string savePath;
    
	string GetCaptureGameObjectName() {
		for (int i = 0; i < transform.childCount; i++) {
			var o = transform.GetChild(i).gameObject;
			if (o.active) {
				return o.name;
			}
		}
		return null;
	}
	
	Vector2Int GetCellPos(int i) {
		int n = transform.childCount;
		int size = (int)Mathf.Ceil(Mathf.Sqrt(n));
		return new Vector2Int(i % size, i / size);
	}
	
	[Button(ButtonSizes.Medium)]
	void ArrangeModel() {
		for (int i = 0; i < transform.childCount; i++) {
			var pos = GetCellPos(i);
			var child = transform.GetChild(i);
			child.localPosition = new Vector3(pos.x * 4, pos.y * 4, 0);
			//child.localRotation = Quaternion.Euler(-90, 0, 0);
			//child.localScale = Vector3.one;
		}
	}
	
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		if (transform != null) {
			for (int i = 0; i < transform.childCount; i++) {
				var child = transform.GetChild(i);
				var cellPos = GetCellPos(i);
				var center = transform.position + new Vector3(cellPos.x * 4, cellPos.y * 4, 0);
			}
		}
	}
	
	[Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
	void Capture() {
		for (int i = 0; i < transform.childCount; i++) {
			var target = transform.GetChild(i);
			string outputPath = Path.Combine(Application.dataPath, savePath, target.gameObject.name + ".png");
			var cellPos = GetCellPos(i);
			Camera.main.transform.position = transform.position + new Vector3(cellPos.x * 4, cellPos.y * 4, -10);
			Utils.CaptureCamera(Camera.main, rt, outputPath);
			Debug.Log($"Generate {outputPath}");
		}
		Debug.Log($"<color=green>Complete</color>");
	}
}