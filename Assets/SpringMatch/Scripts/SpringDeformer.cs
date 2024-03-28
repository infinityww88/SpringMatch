using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

namespace SpringMatch {
	
	public class SpringDeformer : MonoBehaviour
	{
		[SerializeField]
		private SpringCurveFrame _destCurveFrame;
		
		[SerializeField]
		private SpringCurveFrame _springCurve;
		
		[SerializeField]
		private CurvySpline _spline;
		
		[SerializeField]
		private CurvySpline _footTweenCurve;
		[SerializeField]
		private CurvySpline _handTweenCurve;
		[SerializeField]
		private CurvySpline _headTweenCurve;
		
		private SpringBinder _springBinder;
		
		#region test
		public Transform testBegin;
		public Transform testEnd;
		public Transform testTarget;
		
		[Button]
		void TestShrink() {
			Shrink(testBegin.position);
		} 
		
		[Button]
		void TestStretch2Shrink() {
			Stretch2Shrink(testTarget.position);
		} 
		
		[Button]
		void TestShrink2Shrink() {
			Shrink2Shrink(testBegin.position, testEnd.position);
		}
		
		[Button]
		void TestShrink2Stretch() {
			Shrink2Stretch(testBegin.position, testEnd.position);
		}
		
		[Button]
		void TestShrink2Stretch2() {
			Shrink2Stretch(testBegin.position, testEnd.position, testTarget.position, (testEnd.position - testTarget.position).magnitude/2);
		}
		
		#endregion
		
		private Transform footCp, handCp, fixHandCp, headCp;
		private Vector3 fixHandCpInitPos;
		
		public float headHeightFactor = 1;
		public float headHeightOffset = 0;
		
		public float footHeightFactor = 1;
		public float footHeightOffset = 0;
		
		public float handHeightFactor = 1;
		public float handHeightOffset = 0;
		
		public float duration = 1f;
		public float shrinkToShrinkDuration = 0.3f;
		
		public float shrinkHeight = 0.4f;

		public void SetPose(Vector3 pos0, Vector3 pos1, float height) {
			_springCurve.SetFrame(pos0, pos1, height);
		}
		
		#region tween interface
		
		public void Shrink(Vector3 pos) {
			_spline.Interpolation = CurvyInterpolation.Linear;
			float step = shrinkHeight / 4;
			_springBinder.NormalLength = 1;
			_springCurve.foot0.position = pos + Vector3.up * step * 0;
			_springCurve.hand0.position  = pos + Vector3.up * step * 1;
			_springCurve.head.position = pos + Vector3.up * step * 2;
			_springCurve.hand1.position = pos + Vector3.up * step * 3;
			_springCurve.foot1.position = pos + Vector3.up * step * 4;
			_spline.Refresh();
		}
		
		public async UniTask Stretch2Shrink(Vector3 pos) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			var mag0 = (_springCurve.foot0.position - pos).magnitude;
			var mag1 = (_springCurve.foot1.position - pos).magnitude;
			bool fixTail = false;
			if (mag0 > mag1) {
				fixTail = true;
			}
			_springBinder.NormalLength = 1;
			await MoveToTarget(pos, _springCurve.height, fixTail, duration);
			await TweenSpringLen(1, 0.03f, duration, !fixTail);
		}
		
		public async UniTask Shrink2Shrink(Vector3 pos0, Vector3 pos1) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			_springBinder.NormalLength = 0.03f;
			SetPose(pos0, pos1, (pos0 - pos1).magnitude * 2f);
			float t = 0.03f;
			await TweenSpringLen(0.03f, 1, shrinkToShrinkDuration, false);
			await TweenSpringLen(1, 0.03f, shrinkToShrinkDuration, true);
		}
		
		public async UniTask Shrink2Stretch(Vector3 pos0, Vector3 pos1) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			_springBinder.NormalLength = 0.03f;
			_springBinder.Inverse = false;
			SetPose(pos0, pos1, (pos0 - pos1).magnitude/2);
			await TweenSpringLen(0.03f, 1, duration, false);
		}
		
		public async UniTask Shrink2Stretch(Vector3 pos0, Vector3 pos1, Vector3 pos2, float height) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			await Shrink2Stretch(pos0, pos1);
			await MoveToTarget(pos2, height, true, duration);
		}
		#endregion
		
		private Tweener TweenSpringLen(float beginT, float endT, float duration, bool inverse) {
			float t = beginT;
			_springBinder.NormalLength = beginT;
			_springBinder.Inverse = inverse;
			return DOTween.To(() => t, v => {
				t = v;
				_springBinder.NormalLength = t;
			}, endT, duration).SetEase(Ease.Linear);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Awake()
		{
			//SetPose(testBegin.position, testEnd.position, (testBegin.position - testEnd.position).magnitude/2);
			_springBinder = GetComponent<SpringBinder>();
		}

		async UniTask MoveToTarget(Vector3 pos, float height, bool fixTail, float duration) {
			SetTargetPose(pos, height, fixTail);
			await UniTask.NextFrame();
			float t = 0;
			await DOTween.To(() => t, v => {
				t = v;
				LerpCPTweenCurve(t);
			}, 1, duration).SetEase(Ease.Linear);
		}
		
		void SetTargetPose(Vector3 targetPos, float height, bool fixTail) {
			var point = _springCurve.foot0;
			if (fixTail) {
				point = _springCurve.foot1;
			}
			_destCurveFrame.SetFrame(point.position, targetPos, height);
			SetCPTweenCurves(fixTail);
		}
		
		void LerpCPTweenCurve(float lertTf) {
			var pos = _footTweenCurve.Interpolate((lertTf == 0 ? 0 : 0.2f) + 0.8f * lertTf, Space.World);
			footCp.position = pos;
			
			pos = _handTweenCurve.Interpolate((lertTf == 0 ? 0 : 0.05f) + 0.95f * lertTf, Space.World);
			handCp.position = pos;
			
			pos = _headTweenCurve.Interpolate(lertTf, Space.World);
			headCp.position = pos;
			
			fixHandCp.position = Vector3.Lerp(fixHandCpInitPos, _destCurveFrame.hand0.position, lertTf);
		}
		
		void SetCPTweenCurves(bool fixTail) {
			
			headCp = _springCurve.head;
			
			if (fixTail) {
				footCp = _springCurve.foot0;
				handCp = _springCurve.hand0;
				fixHandCp = _springCurve.hand1;
			}
			else {
				footCp = _springCurve.foot1;
				handCp = _springCurve.hand1;
				fixHandCp = _springCurve.hand0;
			}
			
			fixHandCpInitPos = fixHandCp.position;
				
			_handTweenCurve.transform.GetChild(0).position = handCp.position;
			_handTweenCurve.transform.GetChild(2).position = _destCurveFrame.hand1.position;
			_handTweenCurve.transform.GetChild(1).position = 
			(handCp.position + _destCurveFrame.hand1.position) / 2 + Vector3.up * (handHeightFactor * (handCp.position - _destCurveFrame.hand1.position).magnitude + handHeightOffset);
			
			_footTweenCurve.transform.GetChild(0).position = footCp.position;
			_footTweenCurve.transform.GetChild(2).position = _destCurveFrame.foot1.position;
			_footTweenCurve.transform.GetChild(1).position = _handTweenCurve.transform.GetChild(1).position
			 + Vector3.up * (footHeightFactor * (footCp.position - _destCurveFrame.foot1.position).magnitude + footHeightOffset);
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

