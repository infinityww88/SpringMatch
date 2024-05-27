using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpringMatch {
	
	public class ShareDialog : MonoBehaviour
	{
		[SerializeField]
		private Sprite normalBg, grayBg;
		
		[SerializeField]
		private CanvasGroup facebook, twitter, whatsapp;
		
		// Awake is called when the script instance is being loaded.
		protected void Start()
		{
			facebook.interactable = ShareManager.Inst.IsFacebookAvailable;
			twitter.interactable = ShareManager.Inst.IsTwitterAvailable;
			whatsapp.interactable = ShareManager.Inst.IsWhatsAppAvailable;
		}
	}
}

