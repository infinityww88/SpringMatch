using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UnityEngine.Events;

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
		
		private Transform footCp, handCp, fixHandCp, headCp;
		private Vector3 fixHandCpInitPos;
		
		[SerializeField]
		private UnityEvent<GameObject, int> OnChangeSpringModel;
		
		private int currLodIndex = -1;
		private GameObject currModel = null;
		
		#region test
		public Transform foot0;
		public Transform foot1;
		public Transform target;
		[Button]
		void TestSetPose() {
			SetPose(foot0.position, foot1.position, (foot0.position - foot1.position).magnitude / 1.5f);
		}
		
		[Button]
		async UniTaskVoid TestMove() {
			var pos = target.position;
			SetPose(foot0.position, foot1.position, (foot0.position - foot1.position).magnitude / 1.5f);
			var mag0 = (_springCurve.foot0.position - pos).magnitude;
			var mag1 = (_springCurve.foot1.position - pos).magnitude;
			bool fixTail = false;
			if (mag0 > mag1) {
				fixTail = true;
			}
			_springBinder.NormalLength = 1;
			var height = Mathf.Min(mag0, mag1);
			await MoveToTarget(pos, _springCurve.height, fixTail, _spring.Config.gridSlotMoveDuration, CancellationToken.None);
		}
		#endregion
		
		void UpdateModelByDistance(float distance) {
			int n = _spring.Config.springPoolKeys.Count;
			float step = _spring.Config.lodRange.y / n;
			int index = (int)Mathf.Min(n-1, Mathf.Floor(distance / step));
			
			if (index == currLodIndex) {
				return;
			}
			string key = null;
			if (currModel != null) {
				key = _spring.Config.springPoolKeys[currLodIndex];
				GameObjectsPool.Inst.Release(key, currModel);
			}
			
			key = _spring.Config.springPoolKeys[index];
			currModel = GameObjectsPool.Inst.Get(key);
			currModel.GetComponentInChildren<Renderer>().enabled = true;
			currModel.transform.SetParent(transform);
			currLodIndex = index;
			OnChangeSpringModel.Invoke(currModel, currLodIndex);
		}
		
		public void UpdateModelByDistance(Vector3 pos0, Vector3 pos1) {
			UpdateModelByDistance((pos0 - pos1).magnitude);
			
		}

		public void SetPose(Vector3 pos0, Vector3 pos1, float height) {
			_springCurve.SetFrame(pos0, pos1, height);
			UpdateModelByDistance(pos0, pos1);
		}
		
		#region tween interface
		
		private Spring _spring;
		
		public void Shrink(Vector3 pos) {
			_spline.Interpolation = CurvyInterpolation.Linear;
			float step = _spring.Config.shrinkHeight / 4;
			_springBinder.NormalLength = 1;
			_springCurve.foot0.position = pos + Vector3.up * step * 0;
			_springCurve.hand0.position  = pos + Vector3.up * step * 1;
			_springCurve.head.position = pos + Vector3.up * step * 2;
			_springCurve.hand1.position = pos + Vector3.up * step * 3;
			_springCurve.foot1.position = pos + Vector3.up * step * 4;
			UpdateModelByDistance(0);
			_spline.Refresh();
		}
		
		float ClampAutoHeight(float height) {
			return Mathf.Min(_spring.Config.maxAutoHeight, height);
		}
		
		public async UniTask Shrink2Target(Vector3 pos,
			float autoHeightFactor,
			float moveDuration,
			float shrinkDuration,
			CancellationToken ct) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			var mag0 = (_springCurve.foot0.position - pos).magnitude;
			var mag1 = (_springCurve.foot1.position - pos).magnitude;
			bool fixTail = false;
			if (mag0 > mag1) {
				fixTail = true;
			}
			_springBinder.NormalLength = 1;
			var height = Mathf.Min(mag0, mag1);
			//await MoveToTarget(pos, _springCurve.height, fixTail, moveDuration, ct);
			await MoveToTarget(pos, ClampAutoHeight(height * autoHeightFactor), fixTail, moveDuration, ct);
			await TweenSpringLen(1, 0.03f, shrinkDuration, !fixTail).WithCancellation(ct);
		}
		
		public async UniTask Shrink2Shrink(Vector3 pos0,
			Vector3 pos1,
			float autoHeightFactor,
			float duration,
			CancellationToken ct) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			_springBinder.NormalLength = 0.03f;
			float height = (pos0 - pos1).magnitude * autoHeightFactor;
			SetPose(pos0, pos1, Mathf.Min(height, _spring.Config.shrinkToShrinkMaxHeight));
			await TweenSpringLen(0.03f, 1, duration, false).WithCancellation(ct);
			await TweenSpringLen(1, 0.03f, duration, true).WithCancellation(ct);
		}
		
		public async UniTask Shrink2StretchWithHeight(Vector3 pos0, Vector3 pos1, float height, float duration, CancellationToken ct) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			_springBinder.NormalLength = 0.03f;
			_springBinder.Inverse = false;
			SetPose(pos0, pos1, height);
			await TweenSpringLen(0.03f, 1, duration, false).WithCancellation(ct);
		}
		
		public async UniTask Shrink2StretchAutoHeight(Vector3 pos0, Vector3 pos1, float autoHeightFactor, float duration, CancellationToken ct) {
			await Shrink2StretchWithHeight(pos0,
				pos1,
				ClampAutoHeight((pos0 - pos1).magnitude * autoHeightFactor),
				duration,
				ct);
		}
		
		public async UniTask Shrink2Stretch3WithHeight(Vector3 pos0, Vector3 pos1, Vector3 pos2, float height0, float height1, float moveDuration, float stretchDuration, CancellationToken ct) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			await Shrink2StretchWithHeight(pos0, pos1, height0, stretchDuration, ct);
			await MoveToTarget(pos2, height1, true, moveDuration, ct);
		}
		
		public async UniTask Shrink2Stretch3Auto1(Vector3 pos0, Vector3 pos1, Vector3 pos2, float autoHeightFactor, float height, float moveDuration, float stretchDuration, CancellationToken ct) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			await Shrink2StretchAutoHeight(pos0, pos1, autoHeightFactor, stretchDuration, ct);
			await MoveToTarget(pos2, height, true, moveDuration, ct);
		}
		
		public async UniTask Stretch2Strink(Vector3 pos0, Vector3 pos1, float height, float duration, CancellationToken ct) {
			_spline.Interpolation = CurvyInterpolation.BSpline;
			_springBinder.NormalLength = 1f;
			_springBinder.Inverse = false;
			SetPose(pos0, pos1, height);
			await TweenSpringLen(1, 0.03f, duration, false).WithCancellation(ct);
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
			_spring = GetComponentInParent<Spring>();
		}

		async UniTask MoveToTarget(Vector3 pos, float height, bool fixTail, float duration, CancellationToken ct) {
			SetTargetPose(pos, height, fixTail);
			await UniTask.NextFrame();
			float t = 0;
			await DOTween.To(() => t, v => {
				t = v;
				LerpCPTweenCurve(t);
			}, 1, duration)
				.SetEase(Ease.Linear)
				.WithCancellation(ct);
		}
		
		void SetTargetPose(Vector3 targetPos, float height, bool fixTail) {
			var point = _springCurve.foot0;
			if (fixTail) {
				point = _springCurve.foot1;
			}
			UpdateModelByDistance(point.position, targetPos);
			_destCurveFrame.SetFrame(point.position, targetPos, height);
			SetCPTweenCurves(fixTail);
		}
		
		void LerpCPTweenCurve(float lertTf) {
			var pos = _footTweenCurve.Interpolate(_spring.Config.footLerpFactorCurve.Evaluate(lertTf), Space.World);
			footCp.position = pos;
			
			pos = _handTweenCurve.Interpolate(_spring.Config.handLerpFactorCurve.Evaluate(lertTf), Space.World);
			handCp.position = pos;
			
			pos = _headTweenCurve.Interpolate(_spring.Config.headLerpFactorCurve.Evaluate(lertTf), Space.World);
			headCp.position = pos;
			
			fixHandCp.position = Vector3.Lerp(fixHandCpInitPos, _destCurveFrame.hand0.position, lertTf);
		}
		
		void SetCPTweenCurves(bool fixTail) {
			
			var _springConfig = _spring.Config;
			
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
			(handCp.position + _destCurveFrame.hand1.position) / 2 + Vector3.up * (_springConfig.handHeightFactor * (handCp.position - _destCurveFrame.hand1.position).magnitude + _springConfig.handHeightOffset);
			
			_footTweenCurve.transform.GetChild(0).position = footCp.position;
			_footTweenCurve.transform.GetChild(2).position = _destCurveFrame.foot1.position;
			_footTweenCurve.transform.GetChild(1).position = _handTweenCurve.transform.GetChild(1).position
			 + Vector3.up * (_springConfig.footHeightFactor * (footCp.position - _destCurveFrame.foot1.position).magnitude + _springConfig.footHeightOffset);
			_headTweenCurve.transform.GetChild(0).position = headCp.position;
			_headTweenCurve.transform.GetChild(2).position = _destCurveFrame.head.position;
			_headTweenCurve.transform.GetChild(1).position =
			(headCp.position + _destCurveFrame.head.position) / 2 + Vector3.up * (_springConfig.headHeightFactor * (headCp.position - _destCurveFrame.head.position).magnitude + _springConfig.headHeightOffset);
		}
	}
}

