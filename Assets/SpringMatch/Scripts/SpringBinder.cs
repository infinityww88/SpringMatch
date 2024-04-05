using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FluffyUnderware.Curvy;

namespace SpringMatch {
	
	public class SpringBinder : MonoBehaviour
	{
		[SerializeField]
		private CurvySpline spline;
	
		[SerializeField]
		private Transform root;
	
		private Transform[] points;
	
		[Range(0, 1)]
		public float scaleFactor = 0.03f;
		
		[SerializeField]
		[Range(0, 1)]
		private float _normalLength = 1;

		[SerializeField]
		private bool _inverse = false;
		
		public Transform BonesRoot => root;
		
		public float NormalLength {
			get {
				return _normalLength;
			}
			set {
				_normalLength = value;
			}
		}
		
		public bool Inverse {
			get {
				return _inverse;
			}
			set {
				_inverse = value;
			}
		}
	
		// Start is called before the first frame update
		void Awake()
		{
			points = new Transform[root.childCount];
			for (int i = 0; i < root.childCount; i++) {
				points[i] = root.GetChild(i);
			}
		}

		// Update is called once per frame
		void Update()
		{
			var len = spline.Length;
			float step = _normalLength / (points.Length - 1);
			for (int i = 0; i < points.Length; i++) {
				//float distance = step * i;
				float tf = step * i;
				if (_inverse) {
					tf = 1 - _normalLength + tf;
				}
				spline.InterpolateAndGetTangent(tf,
					out Vector3 pos,
					out Vector3 tangent,
					Space.World);
				var up = spline.GetOrientationUpFast(tf, Space.World);
				var forward = tangent;
				if (forward != Vector3.zero && up != Vector3.zero) {
					points[i].rotation = Quaternion.LookRotation(forward, up) * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
				} else {
					Debug.LogWarning($"spline forward or up vector is zero: {GetComponentInParent<Spring>().gameObject.name} forward {forward}, up {up}, bone {points[i].gameObject.name}, index {i}, tf {tf}");
					Debug.Break();
				}
				points[i].position = pos;
				points[i].localScale = new Vector3(1, len * scaleFactor * _normalLength, 1);
			}
		}
	}
}

