using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ItemUsedUpInfo : MonoBehaviour
{
	public float moveHight = 500;
	
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		this.DOKill(false);
		gameObject.SetActive(true);
		var rect = GetComponent<RectTransform>();
		rect.anchoredPosition = Vector3.zero;
		var seq = DOTween.Sequence();
		seq.Append(rect.DOAnchorPosY(moveHight, 0.5f).SetTarget(this))
			.AppendInterval(2)
			.AppendCallback(() => gameObject.SetActive(false))
			.SetTarget(this);
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		
	}
}
