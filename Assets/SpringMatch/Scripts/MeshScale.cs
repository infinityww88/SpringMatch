using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringMatch {
	
	public class MeshScale : MonoBehaviour
	{
		public float scale = 1;
		public SkinnedMeshRenderer meshFilter;
		Vector3[] vertices;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			vertices = meshFilter.sharedMesh.vertices;
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			for (var i = 0; i < vertices.Length; i++) {
				var t = vertices[i];
				t.y *= scale;
				vertices[i] = t;
			}
			meshFilter.sharedMesh.RecalculateBounds();
		}
	}
}

