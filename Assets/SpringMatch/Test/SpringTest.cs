using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FluffyUnderware.Curvy;

public class SpringTest : MonoBehaviour
{
	public CurvySpline spline;
	public Transform root;
	
	private Transform[] points;
	
	[Range(0, 1)]
	public float factor;
	
    // Start is called before the first frame update
    void Start()
    {
	    points = new Transform[root.childCount];
	    for (int i = 0; i < root.childCount; i++) {
	    	points[i] = root.GetChild(i);
	    }
    }
    
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		if (spline == null) {
			return;
		}
		
		spline.InterpolateAndGetTangent(factor,
			out Vector3 pos,
			out Vector3 tangent,
			Space.World);
		Gizmos.DrawSphere(pos, 0.2f);
		Gizmos.DrawRay(new Ray(pos, tangent));
	}

	[Range(0, 1)]
	public float normalLength = 1;
	
    // Update is called once per frame
    void Update()
	{
		var len = spline.Length;
		float step = normalLength / (points.Length - 1);
	    for (int i = 0; i < points.Length; i++) {
	    	//float distance = step * i;
	    	float tf = step * i;
	    	spline.InterpolateAndGetTangent(tf,
		    	out Vector3 pos,
		    	out Vector3 tangent,
		    	Space.World);
		    var up = spline.GetOrientationUpFast(tf, Space.World);
		    var forward = tangent;
		    points[i].rotation = Quaternion.LookRotation(forward, up) * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
		    points[i].position = pos;
		    points[i].localScale = new Vector3(1, len * factor * normalLength, 1);
	    }
    }
}
