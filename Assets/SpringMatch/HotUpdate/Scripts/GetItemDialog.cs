using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

namespace SpringMatch.UI {
	
	public class GetItemDialog : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup shareButton;
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			shareButton.interactable = RewardManager.Inst.CurrentItemConfig.itemType != SpringMatch.ItemConfig.Type.Shift;
		}
	}

}
