using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	public class PrefsManager : MonoBehaviour
	{
		public const string GOLD = "gold";
		public const string HEART = "heart";
		public const string REVOKE_ITEM = "revoke_item";
		public const string SHIFT_ITEM = "shift_item";
		public const string RANDOM_ITEM = "random_item";
		public const string LAST_HEART_REFILL_TIME = "last_heart_refill_time";
		
		private Dictionary<string, int> intCache = new Dictionary<string, int>();
		private Dictionary<string, float> floatCache = new Dictionary<string, float>();
		private Dictionary<string, string> stringCache = new Dictionary<string, string>();
		
		public static PrefsManager Inst;
		
		[SerializeField]
		public IntVariable refillLiveInterval;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
		}
		
		[Button]
		public void DeleteAll() {
			Debug.Log("Delete All Prefs");
			PlayerPrefs.DeleteAll();
		}
		
		public int GoldNum {
			get {
				return GetInt(GOLD, 0);
			}
			set {
				SetInt(GOLD, value);
			}
		}
		
		[Button]
		public void RefillLives() {
			HeartNum = 5;
		}

		[Button]
		public void DecNum() {
			UpdateHeartNum();
			if (HeartNum == 0) {
				return;
			}
			if (HeartNum == 5) {
				LastRefillLifeTime = System.DateTime.Now;
			}
			HeartNum--;
		}
		
		public System.TimeSpan GetRemainRefillTime() {
			if (HeartNum == 5) {
				return System.TimeSpan.FromSeconds(0);
			}
			return System.TimeSpan.FromSeconds(refillLiveInterval.Value) - (System.DateTime.Now - LastRefillLifeTime);
		}
		
		public void UpdateHeartNum() {
			System.TimeSpan timeSpan = System.DateTime.Now - LastRefillLifeTime;
			int seconds = (int)timeSpan.TotalSeconds;
			int shortNum = Mathf.Clamp(5 - HeartNum, 0, 5);
			int refillNum = seconds / refillLiveInterval.Value;
			int fillNum = Mathf.Min(shortNum, refillNum);
			if (fillNum > 0) {
				HeartNum += fillNum;
				LastRefillLifeTime = LastRefillLifeTime + new System.TimeSpan(0, 0, fillNum * refillLiveInterval.Value);
			}
		}
		
		public int HeartNum {
			get {
				return GetInt(HEART, 5);
			}
			set {
				SetInt(HEART, value);
			}
		}
		
		public System.DateTime LastRefillLifeTime {
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
	
		public int GetInt(string key, int dafaultValue) {
			if (!intCache.ContainsKey(key)) {
				intCache[key] = PlayerPrefs.GetInt(key, dafaultValue);
			}
			return intCache[key];
		}
	
		public void SetInt(string key, int value) {
			intCache[key] = value;
			PlayerPrefs.SetInt(key, value);
		}
		
		public float GetFloat(string key, float dafaultValue) {
			if (!floatCache.ContainsKey(key)) {
				floatCache[key] = PlayerPrefs.GetFloat(key, dafaultValue);
			}
			return floatCache[key];
		}
	
		public void SetFloat(string key, float value) {
			floatCache[key] = value;
			PlayerPrefs.SetFloat(key, value);
		}
		
		public string GetString(string key, string dafaultValue) {
			if (!stringCache.ContainsKey(key)) {
				stringCache[key] = PlayerPrefs.GetString(key, dafaultValue);
			}
			return stringCache[key];
		}
	
		public void SetString(string key, string value) {
			stringCache[key] = value;
			PlayerPrefs.SetString(key, value);
		}
	}

}
