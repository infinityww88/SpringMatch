﻿using System.Collections;
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
	
		public float initLen = 1f;
		
		[SerializeField]
		[Range(0, 1)]
		private float _normalLength = 1;
		
		private Spring _spring;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			_spring = GetComponentInParent<Spring>();
		}

		[SerializeField]
		private bool _inverse = false;
		
		public Transform BonesRoot {
			get {
				return root;
			}
			set {
				root = value;
			}
		}
		
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

		// Update is called once per frame
		void LateUpdate()
		{
			if (root == null) {
				return;
			}
			
			var len = spline.Length;
			float step = _normalLength / (root.childCount - 1);
			for (int i = 0; i < root.childCount; i++) {
				var bone = root.GetChild(i);
				//float distance = step * i * len;
				//float tf = spline.DistanceToTF(distance);
				float tf = step * i;
				if (_inverse) {
					tf = 1 - _normalLength + tf;
				}
				spline.InterpolateAndGetTangent(tf,
					out Vector3 pos,
					out Vector3 tangent,
					Space.World);
				var rot = spline.GetOrientationFast(tf, false, Space.World);
				bone.rotation = rot * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
				bone.localScale = new Vector3(1, 
					Mathf.Max(_spring.Config.minScale,
						Mathf.Min(_spring.Config.maxScale, len / initLen * _spring.Config.scaleFactor)) * _normalLength, 1);
				bone.position = pos;
			}
		}
	}
}

