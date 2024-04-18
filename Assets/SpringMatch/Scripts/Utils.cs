using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FluffyUnderware.Curvy;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using System.IO;

public class Utils
{
	public static void AlignCollider(BoxCollider collider, Vector3 pos0, Vector3 pos1, float height, float sizeOffset) {
		var s = collider.size;
		s.x = (pos0 - pos1).magnitude + sizeOffset;
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
	
	public static void InitUTK<T>(T component) where T : MonoBehaviour {
		var doc = component.GetComponent<UIDocument>();
		var root = doc.rootVisualElement;
		FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (var fi in fields) {
			if (fi.FieldType == typeof(VisualElement) || fi.FieldType.IsSubclassOf(typeof(VisualElement))) {
				var attr = fi.GetCustomAttribute<SpringMatchEditor.MyUTKElementAttr>();
				if (attr != null) {
					var e = root.Q(attr.FieldName);
					Assert.IsNotNull(e, $"{attr.FieldName} visual element can't find");
					fi.SetValue(component, e);
				}
			}
		}
	}
}