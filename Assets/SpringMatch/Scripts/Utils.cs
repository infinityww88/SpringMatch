using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FluffyUnderware.Curvy;

public class Utils
{
	public static void AlignCollider(BoxCollider collider, Vector3 pos0, Vector3 pos1, float height) {
		var s = collider.size;
		s.x = (pos0 - pos1).magnitude;
		collider.size = s;
		collider.transform.position = (pos0 + pos1) / 2 + Vector3.up * height;
		collider.transform.rotation = Quaternion.FromToRotation(Vector3.right, pos0 - pos1);
	}
	
	public static async UniTaskVoid RunNextFrame(Action action, int frameCount = 1) {
		await UniTask.DelayFrame(frameCount);
		action?.Invoke();
	}
	
	public static async UniTask CallDelay(Action action, float seconds) {
		await UniTask.WaitForSeconds(seconds);
		action?.Invoke();
	}
}