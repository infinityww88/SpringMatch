using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using SpringMatch;
using ScriptableObjectArchitecture;

namespace SpringMatch.UI {
	
	public class ItemButton : MonoBehaviour
	{
		[SerializeField]
		private Sprite _grayBg, _normalBg;
		
		[SerializeField]
		private TextMeshProUGUI _itemNumText, _itemCostText;
		
		private int _itemNum;
		private bool _valid = false;
		
		[SerializeField]
		private string _saveKey;
		
		[SerializeField]
		private ItemConfig itemConfig;
		
		[SerializeField]
		private UnityEngine.Events.UnityEvent onRequestItem, onUseItem;
		
		[Button]
		public int ItemNum {
			get {
				return _itemNum;
			}
			set {
				_itemNum = value;
				_itemNumText.text = $"{_itemNum}";
				PrefsManager.SetInt(_saveKey, value);
			}
		}
		
		[Button]
		public bool Valid {
			get {
				return _valid;
			}
			set {
				GetComponent<Image>().sprite = value ? _normalBg : _grayBg;
				_valid = value;
			}
		}
		
		public void OnClick() {
			if (ItemNum > 0) {
				if (Valid) {
					ItemNum--;
					onUseItem.Invoke();
				}
				
			} else {
				onRequestItem.Invoke();
			}
		}
		
		// Awake is called when the script instance is being loaded.
		protected void Start()
		{
			_itemNum = PrefsManager.GetInt(_saveKey, 0);
			_itemNumText.text = $"{_itemNum}";
			_itemCostText.text = $"{itemConfig.goldCost}";
		}
	}
}

