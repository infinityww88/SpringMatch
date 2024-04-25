using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

namespace SpringMatch.UI {
	
	public class ItemButton : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _supText;
		
		[SerializeField]
		private RectTransform _requestItemDialog;
		
		[SerializeField]
		private UnityEvent<ItemButton> OnUseItem;
		
		//[SerializeField]
		//private ItemUsedUpInfo _itemUsedUpInfo;
		
		private bool ItemUsed { get; set; }
		
		private int _num = 0;
		
		public void ShakeButton() {
			this.DOKill(true);
			GetComponent<RectTransform>().DOPunchRotation(new Vector3(0, 0, 30f), 0.3f).SetTarget(this);
		}
		
		public void ScaleButton() {
			this.DOKill(true);
			transform.DOScale(1.2f, 0.1f).SetLoops(2, LoopType.Yoyo).SetTarget(this);
		}
		
		public void UseItem() {
			_num  = 0;
			ItemUsed = true;
			_supText.transform.parent.gameObject.SetActive(false);
		}
		
		public void OnClick() {
			if (ItemUsed) {
				ShakeButton();
				//_itemUsedUpInfo.gameObject.SetActive(true);
				GameLogic.Inst.ShowTips("无法重复使用");
				return;
			}
			ScaleButton();
			if (_num == 0) {
				_requestItemDialog.gameObject.SetActive(true);
				return;
			}
			OnUseItem.Invoke(this);
		}
		
		public void OnGetItem() {
			ScaleButton();
			_supText.text = "1";
			_num = 1;
		}
	}

}
