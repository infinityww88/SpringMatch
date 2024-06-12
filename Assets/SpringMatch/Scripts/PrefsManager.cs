using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using ScriptableObjectArchitecture;
using QFSW.QC;

namespace SpringMatch {

	public class PrefsManager : MonoBehaviour
	{
		public const string GOLD = "gold";
		public const string HEART = "heart";
		public const string REVOKE_ITEM = "revoke_item";
		public const string SHIFT_ITEM = "shift_item";
		public const string RANDOM_ITEM = "random_item";
		public const string LAST_HEART_REFILL_TIME = "last_heart_refill_time";
		public const string VERSION = "version";
		public const string RES_VERSION = "resVersion";
		public const string LEVEL_INDEX = "level_index";
		public const string SOUND_ON = "setting_sound";
		public const string VIBRATE_ON = "setting_vibrate";
		public const string CDN = "cdn";
		
		private static Dictionary<string, int> intCache = new Dictionary<string, int>();
		private static Dictionary<string, float> floatCache = new Dictionary<string, float>();
		private static Dictionary<string, string> stringCache = new Dictionary<string, string>();
		
		public static PrefsManager Inst;

		[SerializeField]
		private IntVariable refillLifeInterval;
		
		[SerializeField]
		private IntGameEvent goldUpdate, heartUpdate, revokeUpdate, shiftUpdate, randomUpdate;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			UpdateHeartNum();
			goldUpdate.Raise(GoldNum);
			heartUpdate.Raise(HeartNum);
			revokeUpdate.Raise(RevokeItemNum);
			shiftUpdate.Raise(ShiftItemNum);
			randomUpdate.Raise(RandomItemNum);
		}
		
		public void AddRevoke(int num = 1) {
			RevokeItemNum += num;
		}
		
		public void AddShift(int num = 1) {
			ShiftItemNum += num;
		}
		
		public void AddRandom(int num = 1) {
			RandomItemNum += num;
		}
		
		[Button]
		public void DeleteAll() {
			Debug.Log("Delete All Prefs");
			ES3.DeleteFile();
		}
		
		[Button]
		public int RevokeItemNum {
			get {
				return GetInt(REVOKE_ITEM, 0);
			}
			set {
				if (RevokeItemNum != value) {
					SetInt(REVOKE_ITEM, value);
					revokeUpdate.Raise(value);
				}
			}
		}
		
		[Button]
		public int ShiftItemNum {
			get {
				return GetInt(SHIFT_ITEM, 0);
			}
			set {
				if (ShiftItemNum != value) {
					SetInt(SHIFT_ITEM, value);
					shiftUpdate.Raise(value);
				}
			}
		}
		
		[Button]
		public int RandomItemNum {
			get {
				return GetInt(RANDOM_ITEM, 0);
			}
			set {
				if (RandomItemNum != value) {
					SetInt(RANDOM_ITEM, value);
					randomUpdate.Raise(value);
				}
			}
		}
		
		[Button]
		public int GoldNum {
			get {
				return GetInt(GOLD, 0);
			}
			set {
				if (GoldNum != value) {
					SetInt(GOLD, value);
					goldUpdate.Raise(value);
				}
			}
		}
		
		public void RefillLives() {
			HeartNum = 5;
		}

		[Button]
		public void DecHeartNum(int n=1) {
			n = Mathf.Min(Mathf.Clamp(n, 0, 5), PrefsManager.Inst.HeartNum);
			if (HeartNum == 0) {
				return;
			}
			if (HeartNum == 5) {
				LastRefillLifeTime = System.DateTime.Now;
			}
			HeartNum -= n;
		}
		
		public void UpdateHeartNum() {
			System.TimeSpan timeSpan = System.DateTime.Now - PrefsManager.Inst.LastRefillLifeTime;
			int seconds = (int)timeSpan.TotalSeconds;
			int shortNum = Mathf.Clamp(5 - PrefsManager.Inst.HeartNum, 0, 5);
			int refillNum = seconds / refillLifeInterval.Value;
			int fillNum = Mathf.Min(shortNum, refillNum);
			if (fillNum > 0) {
				PrefsManager.Inst.HeartNum += fillNum;
				PrefsManager.Inst.LastRefillLifeTime = PrefsManager.Inst.LastRefillLifeTime + new System.TimeSpan(0, 0, fillNum * refillLifeInterval.Value);
			}
		}
		
		[Button]
		public int HeartNum {
			get {
				return GetInt(HEART, 5);
			}
			private set {
				SetInt(HEART, value);
				heartUpdate.Raise(value);
			}
		}
		
		[Button]
		public int LevelIndex {
			get {
				return GetInt(LEVEL_INDEX, 0);
			}
			set {
				SetInt(LEVEL_INDEX, value);
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
		
		public static bool GetBool(string key, bool defaultValue) {
			int df = defaultValue ? 1 : 0;
			if (!intCache.ContainsKey(key)) {
				intCache[key] = ES3.Load<int>(key, df);
			}
			return intCache[key] != 0;
		}
		
		[Command]
		public static void SetDevMode(bool on) {
			SetBool("dev_mode", on);
		}
		
		[Command]
		public static bool GetDevMode() {
			return GetBool("dev_mode", true);
		}
	
		public static void SetBool(string key, bool value) {
			int v = value ? 1 : 0;
			intCache[key] = v;
			ES3.Save<int>(key, v);
		}
		
		public static int GetInt(string key, int defaultValue) {
			if (!intCache.ContainsKey(key)) {
				intCache[key] = ES3.Load<int>(key, defaultValue);
			}
			return intCache[key];
		}
	
		public static void SetInt(string key, int value) {
			intCache[key] = value;
			ES3.Save<int>(key, value);
		}
		
		public static float GetFloat(string key, float defaultValue) {
			if (!floatCache.ContainsKey(key)) {
				floatCache[key] = ES3.Load<float>(key, defaultValue);
			}
			return floatCache[key];
		}
	
		public static void SetFloat(string key, float value) {
			floatCache[key] = value;
			ES3.Save<float>(key, value);
		}
		
		public static string GetString(string key, string defaultValue) {
			if (!stringCache.ContainsKey(key)) {
				stringCache[key] = ES3.Load<string>(key, defaultValue: defaultValue);
			}
			return stringCache[key];
		}
	
		public static void SetString(string key, string value) {
			stringCache[key] = value;
			ES3.Save<string>(key, value);
		}
	}

}
