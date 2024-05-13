using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;

namespace SpringMatch.UI {
	
	public class DialogTween : MonoBehaviour
	{
		[SerializeField]
		private FloatVariable duration, targetScale;
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			GameLogic.Inst.PendInteract = true;
			transform.localScale = Vector3.one * 0.5f;
			transform.DOScale(targetScale.Value, duration.Value).SetEase(Ease.OutBack);
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			GameLogic.Inst.PendInteract = false;
		}
	}
}

