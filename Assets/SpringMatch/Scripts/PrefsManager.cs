using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public static class PrefsManager
	{
		public const string GOLD = "gold";
		public const string HEART = "heart";
		public const string REVOKE_ITEM = "revoke_item";
		public const string SHIFT_ITEM = "shift_item";
		public const string RANDOM_ITEM = "random_item";
	
		public static int GetInt(string key, int dafaultValue) {
			return PlayerPrefs.GetInt(key, dafaultValue);
		}
	
		public static void SetInt(string key, int value) {
			PlayerPrefs.SetInt(key, value);
		}
	}

}
