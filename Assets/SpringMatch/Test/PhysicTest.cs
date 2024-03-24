using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FluffyUnderware.Curvy;

public class PhysicTest : MonoBehaviour
{
	public BoxCollider _boxCollider;
	public Transform cube;
	
	public CurvySpline spline;
	public float radius;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
	[Button]
	void GenerateCollider() {
		var len = spline.Length;
		var n = Mathf.Ceil(len / radius);
		for (int i = 0; i < n; i++) {
			var l = i * len / n;
			var tf = spline.DistanceToTF(l);
			var pos = spline.Interpolate(tf, Space.World);
			GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			o.transform.position = pos;
			o.transform.SetParent(transform, true);
			o.transform.localScale = Vector3.one * radius * 2;
		}
	}
    
	[Button]
	void Test() {
		var box = cube.gameObject.AddComponent<BoxCollider>();
		box.center = Vector3.zero;
		box.size = Vector3.one;
		var colliders = Physics.OverlapBox(GetComponent<Collider>().transform.TransformPoint(_boxCollider.center), _boxCollider.size/2, _boxCollider.transform.rotation, -1, QueryTriggerInteraction.Collide);
		Debug.Log($"colliders {colliders} {_boxCollider.center}");
		if (colliders != null) {
			for (int i = 0; i < colliders.Length; i++) {
				Debug.Log($"{colliders[i].gameObject.name}");
			}
		};
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
