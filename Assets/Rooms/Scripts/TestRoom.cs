using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;

namespace CustomRoom {
	
	public class TestRoom : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			
		}
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			var mesh = GetComponent<MeshFilter>();
			if(mesh == null) {
				return; 
			}
			Gizmos.color = Color.red;
			var bound = mesh.sharedMesh.bounds;
			var oldMat = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(bound.center, bound.size);
			Gizmos.matrix = oldMat;
		}
		
		/*
		void GeneratePivot() {
			var bound = meshCollider.bounds;
			Vector3 a = bound.min, b = new Vector3(bound.max.x, bound.min.y, bound.max.z);
			var c = (a + b) / 2;
			GameObject o = new	GameObject("hello");
			o.transform.position = c;
			transform.SetParent(o.transform, true);
		}
		*/

		[Button]
		void CombineMesh() {
			EditorUtils.CombineMesh(gameObject);
		}
	}
}

