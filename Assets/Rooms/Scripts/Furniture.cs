using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CustomRoom {
	
	public class Furniture : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
        	
		}
		
		[Button]
		void MarkMeshColliderConvex(bool convex) {
			gameObject.GetComponentsInChildren<MeshCollider>().Foreach(collider => {
				collider.convex = convex;
			});
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}
}

