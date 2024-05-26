using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

namespace SpringMatch {
	
	public class RewardManager : MonoBehaviour
	{
		[SerializeField]
		private ItemConfig revokeItemConfig, shiftItemConfig, randomItemConfig;
		public static RewardManager Inst;
		
		[SerializeField]
		private SimpleAnimator revokeRewardAnimator, shiftRewardAnimator, randomRewardAnimator;
		
		[SerializeField]
		private GameObject _getItemDialog;
		
		private ItemConfig currentItemConfig;
		
		void Awake()
		{
			Inst = this;
		}
		
		public void AddItem() {
			if (currentItemConfig.itemType == ItemConfig.Type.Revoke) {
				PrefsManager.Inst.AddRevoke();
				revokeRewardAnimator.gameObject.SetActive(true);
				revokeRewardAnimator.Play();
			}
			else if (currentItemConfig.itemType == ItemConfig.Type.Shift) {
				PrefsManager.Inst.AddShift();
				shiftRewardAnimator.gameObject.SetActive(true);
				shiftRewardAnimator.Play();
			}
			else {
				PrefsManager.Inst.AddRandom();
				randomRewardAnimator.gameObject.SetActive(true);
				randomRewardAnimator.Play();
			}
		}
		
		public void BuyGold() {
			Debug.Log($"{PrefsManager.Inst.GoldNum} {currentItemConfig.goldCost}");
			if (PrefsManager.Inst.GoldNum < currentItemConfig.goldCost) {
				UI.UIVariable.Inst.shopDialog.gameObject.SetActive(true);
				return;
			}
			PrefsManager.Inst.GoldNum -= currentItemConfig.goldCost;
			AddItem();
		}
		
		public void GetFreeByAd() {
			AdManager.Inst.ShowRewardedAd(AddItem);
		}
		
		public void GetRevokeItem() {
			GetItem(revokeItemConfig);
		}
		
		public void GetShiftItem() {
			GetItem(shiftItemConfig);
		}
		
		public void GetRandomItem() {
			GetItem(randomItemConfig);
		}
		
		public void GetItem(ItemConfig itemConfig) {
			currentItemConfig = itemConfig;
			_getItemDialog.SetActive(true);
		}
		
		public void ShareByFacebook() {
			//ShareManager.Inst.
		}
		
		public void ShareByTwitter() {
			ShareManager.Inst.ShareTwitter(AddItem, null);
		}
		
		public void ShareByWhatsApp() {
			
		}
	}

}
