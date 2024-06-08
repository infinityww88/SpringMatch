using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		for (int i = 0 ; i < transform.childCount; i++) {
			Gizmos.DrawSphere(transform.GetChild(i).position, 0.2f);
		}
	}
}
