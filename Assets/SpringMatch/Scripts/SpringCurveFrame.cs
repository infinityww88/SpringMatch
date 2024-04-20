using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class SpringCurveFrame : MonoBehaviour
	{
		public Transform foot0;
		public Transform hand0;
		public Transform head;
		public Transform hand1;
		public Transform foot1;
		public float height;
	
		[SerializeField]
		private float handHeightN = 0.3f;
	
		public void SetFrame(Vector3 pos0, Vector3 pos1, float height) {
			foot0.position = pos0;
			foot1.position = pos1;
			this.height = height;
			Vector3 center = (pos0 + pos1) / 2;
			head.position = center + Vector3.up * height;
			Vector3 bias = pos1 - pos0;
			hand0.position = foot0.position + Vector3.up * height * handHeightN + bias.normalized * 0.001f;
			hand1.position = foot1.position + Vector3.up * height * handHeightN + bias.normalized * -0.001f;
		}
	}

}
