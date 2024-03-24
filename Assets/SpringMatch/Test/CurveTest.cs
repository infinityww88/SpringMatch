using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy;
using Sirenix.OdinInspector;

public class CurveTest : MonoBehaviour
{
	public CurvySpline spline;
	public Transform target;
	
	[Range(0, 1)]
	[OnValueChanged("OnFactorChange")]
	public float factor;
	
	public void OnFactorChange() {
		target.position = spline.Interpolate(factor, Space.World);
	}
}
