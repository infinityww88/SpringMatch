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
			arrow.transform.forward = (start - end).normalized;
			line.material.SetTextureScale("_BaseMap", texScale * (end - start).magnitude);
		}
	}
}

