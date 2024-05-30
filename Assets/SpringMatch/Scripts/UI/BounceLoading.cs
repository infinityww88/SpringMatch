using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BoundLoading : MonoBehaviour
{
	public Image image;
	public float duration = 1f;
	
	[Button]
	void Test(float d) {
		image.material.SetTextureOffset("_MainTex", Vector2.zero);
		image.material.DOOffset(new Vector2(-1, 0), Shader.PropertyToID("_MainTex"), duration).SetLoops(10, LoopType.Restart);
	}
}
