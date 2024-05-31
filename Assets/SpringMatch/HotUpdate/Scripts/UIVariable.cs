using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;

namespace SpringMatch.UI {
	
	public class UIVariable : MonoBehaviour
	{
		public static UIVariable Inst;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
		}
		
		[FoldoutGroup("Dialog")]
		public GameObject shopDialog;
		[FoldoutGroup("Dialog")]
		public GameObject itemRequestDialog;
		[FoldoutGroup("Dialog")]
		public GameObject refillLifeDialog;
		[FoldoutGroup("Dialog")]
		public GameObject outOfLifeDialog;
		[FoldoutGroup("Dialog")]
		public GameObject outOfLifeStartDialog;
		[FoldoutGroup("Dialog")]
		public GameObject outOfSpaceDialog;
		[FoldoutGroup("Dialog")]
		public GameObject loseLifeConfirmDialog;
		[FoldoutGroup("Dialog")]
		public GameObject SettingDialog;
		[FoldoutGroup("Dialog")]
		public GameObject getItemDialog;
		[FoldoutGroup("Dialog")]
		public GameObject shareDialog;
		[FoldoutGroup("Dialog")]
		public PurchaseResult shopBundleGet;
		[FoldoutGroup("Dialog")]
		public PurchaseResult shopGoldGet;
		
		[FoldoutGroup("Effect")]
		public GameObject toast;
		[FoldoutGroup("Effect")]
		public GameObject levelPass;
		[FoldoutGroup("Effect")]
		public GameObject rewardGoldEffect;
		
		[FoldoutGroup("Variable")]
		public IntVariable heartGoldCost;
		[FoldoutGroup("Variable")]
		public IntVariable playOnGoldCost;
		[FoldoutGroup("Variable")]
		public IntVariable levelPassGoldReward;
		
		[Button]
		public void ShowToast(string msg) {
			toast.SetActive(true);
			toast.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = msg;
			UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(toast.GetComponent<RectTransform>());
			toast.GetComponent<VisualTweenSequence.TweenSequence>().Play();
		}
	}
}

