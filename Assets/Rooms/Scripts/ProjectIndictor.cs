using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CustomRoom {
	
	public class ProjectIndictor : MonoBehaviour
	{
		[SerializeField]
		private LineRenderer line;
		[SerializeField]
		private GameObject arrow;
		[SerializeField]
		private Vector2 texScale;
	
		public Transform start, end;
	
		[Button]
		void Test() {
			SetPos(start.position, end.position);
		}
	
		public void SetPos(Vector3 start, Vector3 end) {
			line.SetPosition(0, start);
			line.SetPosition(1, end);
			arrow.transform.position = end;
			Vector3 diff = end - start;
			Vector3 up = Vector3.up;
			if (diff.x == 0 && diff.z == 0) {
				up = Vector3.right;
			}
			arrow.transform.LookAt(arrow.transform.position + diff, Vector3.up);
			line.material.SetTextureScale("_BaseMap", texScale * diff.magnitude);
		}
	}
}

