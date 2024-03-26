using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class SpringDeformer : MonoBehaviour
	{
		[SerializeField]
		private SpringCurveFrame _destCurveFrame;
		
		[SerializeField]
		private SpringCurveFrame _springCurve;
		
		[SerializeField]
		private CurvySpline _footTweenCurve;
		[SerializeField]
		private CurvySpline _handTweenCurve;
		[SerializeField]
		private CurvySpline _headTweenCurve;
		
		#region test
		public Transform testBegin;
		public Transform testEnd;
		public Transform testTarget;
		public float testHeight = 4;
		#endregion
		
		private Transform footCp, handCp, headCp;
		
		[SerializeField]
		[OnValueChanged("LerpCPTweenCurve")]
		[Range(0, 1)]
		private float lertTf = 0;
		
		public float headHeightFactor = 1;
		public float headHeightOffset = 0;
		
		public float footHeightFactor = 1;
		public float footHeightOffset = 0;
		
		public float handHeightFactor = 1;
		public float handHeightOffset = 0;
		
		[Button]
		void InitPose() {
			_springCurve.SetFrame(testBegin.position, testEnd.position, testHeight);
		}
		
		[Button]
		void SetTargetPos() {
			_destCurveFrame.SetFrame(testEnd.position, testTarget.position, testHeight);
			SetCPTweenCurves(true);
		}
		
		void LerpCPTweenCurve() {
			var pos = _footTweenCurve.Interpolate((lertTf == 0 ? 0 : 0.3f) + 0.7f * lertTf, Space.World);
			footCp.position = pos;
			
			pos = _handTweenCurve.Interpolate((lertTf == 0 ? 0 : 0.1f) + 0.9f * lertTf, Space.World);
			handCp.position = pos;
			
			pos = _headTweenCurve.Interpolate(lertTf, Space.World);
			headCp.position = pos;
		}
		
		[Button]
		void SetCPTweenCurves(bool fixTail) {
			
			headCp = _springCurve.head;
			
			if (fixTail) {
				footCp = _springCurve.foot0;
				handCp = _springCurve.hand0;
			}
			else {
				footCp = _springCurve.foot1;
				handCp = _springCurve.hand1;
			}
			
			_footTweenCurve.transform.GetChild(0).position = footCp.position;
			_footTweenCurve.transform.GetChild(2).position = _destCurveFrame.foot1.position;
			_footTweenCurve.transform.GetChild(1).position =
			(footCp.position + _destCurveFrame.foot1.position) / 2 + Vector3.up * (footHeightFactor * (footCp.position -  _destCurveFrame.foot1.position).magnitude + footHeightOffset);
				
			_handTweenCurve.transform.GetChild(0).position = handCp.position;
			_handTweenCurve.transform.GetChild(2).position = _destCurveFrame.hand1.position;
			_handTweenCurve.transform.GetChild(1).position = 
			(handCp.position + _destCurveFrame.hand1.position) / 2 + Vector3.up * (handHeightFactor * (handCp.position - _destCurveFrame.hand1.position).magnitude + handHeightOffset);
				
			_headTweenCurve.transform.GetChild(0).position = headCp.position;
			_headTweenCurve.transform.GetChild(2).position = _destCurveFrame.head.position;
			_headTweenCurve.transform.GetChild(1).position =
			(headCp.position + _destCurveFrame.head.position) / 2 + Vector3.up * (headHeightFactor * (headCp.position - _destCurveFrame.head.position).magnitude + headHeightOffset);
		}
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			if (_destCurveFrame == null) {
				return;
			}
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(_destCurveFrame.foot0.position, 0.2f);
			Gizmos.DrawSphere(_destCurveFrame.hand0.position, 0.2f);
			Gizmos.DrawSphere(_destCurveFrame.head.position, 0.2f);
			Gizmos.DrawSphere(_destCurveFrame.hand1.position, 0.2f);
			Gizmos.DrawSphere(_destCurveFrame.foot1.position, 0.2f);
		}
	}
}

