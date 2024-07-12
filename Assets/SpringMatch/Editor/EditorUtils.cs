using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using Unity.Linq;

public class EditorUtils
{
	public static void CombineMesh(GameObject gameObject) {
		var mf = gameObject.GetComponent<MeshFilter>();
			
		MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);
		meshFilters = meshFilters.Where(f => mf != f).ToArray();
	
		CombineInstance[] combines = new	CombineInstance[meshFilters.Length];
		for (int i = 0; i < meshFilters.Length; i++) {
			combines[i].mesh = meshFilters[i].sharedMesh;
			var t = meshFilters[i].transform;
			combines[i].transform = Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
			meshFilters[i].gameObject.SetActive(false);
		}
			
		Mesh mesh = new Mesh();
		mesh.CombineMeshes(combines);
			
		var vertices = mesh.vertices;
		Vector3 bottomCenter = mesh.bounds.center - Vector3.up * (mesh.bounds.max.y - mesh.bounds.min.y)/2;
		Vector3 worldPos = gameObject.transform.TransformPoint(bottomCenter);
		for (int i = 0; i < mesh.vertexCount; i++){
			vertices[i] -= bottomCenter;
		}
		mesh.SetVertices(vertices);
		mesh.RecalculateBounds();
			
		gameObject.transform.position = worldPos;
			
		MeshCollider mc = null;
			
		string path = null;
		
		var folderName = gameObject.Ancestors()
			.Where(o => Regex.IsMatch(o.name, @"Room_\d+"))
			.First()
			.name;
		
		var folderPath = $"Assets/Rooms/CombineMesh/{folderName}";
		
		if (!System.IO.Directory.Exists(folderPath)) {
			System.IO.Directory.CreateDirectory(folderPath);
		}
			
		if (mf == null) {
			path = $"{folderPath}/{System.Guid.NewGuid().ToString()}.mesh";
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			mc = gameObject.AddComponent<MeshCollider>();
			mc.convex = true;
				
		}
		else {
			path = AssetDatabase.GetAssetPath(mf.sharedMesh);
			mf.sharedMesh = mesh;
			GameObject.DestroyImmediate(gameObject.GetComponent<MeshCollider>());
			mc = gameObject.AddComponent<MeshCollider>();
			mc.convex = true;
		}
		
		var render = gameObject.GetComponent<MeshRenderer>();
		if (render == null) {
			var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Interior_1/Materials/Color.mat");
			render = gameObject.AddComponent<MeshRenderer>();
			render.sharedMaterial = mat;
		}
		
		AssetDatabase.CreateAsset(mesh, path);
	}
}
