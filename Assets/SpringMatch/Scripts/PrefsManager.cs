using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public static class PrefsManager
	{
		public const string GOLD = "gold";
		public const string HEART = "heart";
		public const string REVOKE_ITEM = "revoke_item";
		public const string SHIFT_ITEM = "shift_item";
		public const string RANDOM_ITEM = "random_item";
		public const string LAST_HEART_REFILL_TIME = "last_heart_refill_time";
		public const string VERSION = "version";
		public const string RES_VERSION = "resVersion";
		
		private static Dictionary<string, int> intCache = new Dictionary<string, int>();
		private static Dictionary<string, float> floatCache = new Dictionary<string, float>();
		private static Dictionary<string, string> stringCache = new Dictionary<string, string>();
		
		public static void DeleteAll() {
			Debug.Log("Delete All Prefs");
			PlayerPrefs.DeleteAll();
		}
		
		public static int GoldNum {
			get {
				return GetInt(GOLD, 0);
			}
			set {
				SetInt(GOLD, value);
			}
		}
		
		public static void RefillLives() {
			HeartNum = 5;
		}

		public static void DecHeartNum(int n=1) {
			n = Mathf.Min(Mathf.Clamp(n, 0, 5), PrefsManager.HeartNum);
			//UpdateHeartNum();
			if (HeartNum == 0) {
				return;
			}
			if (HeartNum == 5) {
				LastRefillLifeTime = System.DateTime.Now;
			}
			HeartNum -= n;
		}
		
		public static void UpdateHeartNum(int refillLiveInterval) {
			System.TimeSpan timeSpan = System.DateTime.Now - PrefsManager.LastRefillLifeTime;
			int seconds = (int)timeSpan.TotalSeconds;
			int shortNum = Mathf.Clamp(5 - PrefsManager.HeartNum, 0, 5);
			int refillNum = seconds / refillLiveInterval;
			int fillNum = Mathf.Min(shortNum, refillNum);
			if (fillNum > 0) {
				PrefsManager.HeartNum += fillNum;
				PrefsManager.LastRefillLifeTime = PrefsManager.LastRefillLifeTime + new System.TimeSpan(0, 0, fillNum * refillLiveInterval);
			}
		}
		
		public static int HeartNum {
			get {
				return GetInt(HEART, 5);
			}
			private set {
				SetInt(HEART, value);
			}
		}
		
		public static System.DateTime LastRefillLifeTime {
			get {
				var v = GetString(LAST_HEART_REFILL_TIME, "");
				if (v == "") {
					var now = System.DateTime.Now;
					SetString(LAST_HEART_REFILL_TIME, now.ToString());
					return now;
				}
				return System.DateTime.Parse(v);
			}
			set {
				SetString(LAST_HEART_REFILL_TIME, value.ToString());
			}
		}
	
		public static int GetInt(string key, int dafaultValue) {
			if (!intCache.ContainsKey(key)) {
				intCache[key] = PlayerPrefs.GetInt(key, dafaultValue);
			}
			return intCache[key];
		}
	
		public static void SetInt(string key, int value) {
			intCache[key] = value;
			PlayerPrefs.SetInt(key, value);
		}
		
		public static float GetFloat(string key, float dafaultValue) {
			if (!floatCache.ContainsKey(key)) {
				floatCache[key] = PlayerPrefs.GetFloat(key, dafaultValue);
			}
			return floatCache[key];
		}
	
		public static void SetFloat(string key, float value) {
			floatCache[key] = value;
			PlayerPrefs.SetFloat(key, value);
		}
		
		public static string GetString(string key, string dafaultValue) {
			if (!stringCache.ContainsKey(key)) {
				stringCache[key] = PlayerPrefs.GetString(key, dafaultValue);
			}
			return stringCache[key];
		}
	
		public static void SetString(string key, string value) {
			stringCache[key] = value;
			PlayerPrefs.SetString(key, value);
		}
	}

}
