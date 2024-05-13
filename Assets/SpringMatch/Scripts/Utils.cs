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

public static class Utils
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
	
	public static void CaptureCamera(Camera camera, RenderTexture rt, string path) {
		var t = RenderTexture.active;
		RenderTexture.active = rt;
		camera.targetTexture = rt;
		camera.Render();
		Texture2D tex = new Texture2D(rt.width, rt.height);
		tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		tex.Apply();
		byte[] data = ImageConversion.EncodeToPNG(tex);
		File.WriteAllBytes(path, data);
		camera.targetTexture = null;
		RenderTexture.active = t;
	}
	
	public static void Foreach<T>(this IEnumerable<T> array, Action<T> action) {
		foreach (var e in array) {
			action(e);
		}
	}
	
	public static void Foreach<T>(this IEnumerable<T> array, Action<int, T> action)
	{
		int i = 0;
		foreach (var e in array) {
			action(i, e);
			i++;
		}
	}
	
	public static Vector3 SetX(this Vector3 self, float v) {
		self.x = v;
		return self;
	}
	
	public static Vector3 SetY(this Vector3 self, float v) {
		self.y = v;
		return self;
	}
	
	public static Vector3 SetZ(this Vector3 self, float v) {
		self.z = v;
		return self;
	}
}