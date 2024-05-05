using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarkSDKSpace;
using StarkSDKSpace.UNBridgeLib.LitJson;
using System;

namespace SpringMatch {
	
	public static class SDKManager
	{
		public static void Share() {
			JsonData jd = new JsonData();
			//jd["channel"] = "invite";
			jd["title"] = "测试分享好友";
			jd["desc"] = "测试描述";
			jd["imageUrl"] = "";
			jd["query"] = "";
			StarkSDK.API.GetStarkShare().ShareAppMessage(
				msg => {
					Debug.Log($"success {msg}");
				},
				msg => {
					Debug.Log($"fail {msg}");
				},
				() => {
					Debug.Log("cancel");
				},
				jd);
		}
		
		public static void NavSideBar() {
			StarkSDK.API.GetStarkSideBarManager().NavigateToScene(StarkSideBar.SceneEnum.SideBar,
			() => { Debug.Log("Nav to Side Bar success"); },
			() => {},
			(code, msg) => {
				Debug.Log($"Nav to Side Bar error: {code} {msg}");
			}
			);
		}
		
		public static bool InRecord = false;
		public static bool PendingShare = false;
		
		public static void StartRecord() {
			StarkSDK.API.GetStarkGameRecorder().StartRecord(true,
				startCallback: () => {
					InRecord = true;
					PendingShare = true;
				}
			);
		}
		
		public static void StopRecord() {
			InRecord = false;
			StarkSDK.API.GetStarkGameRecorder().StopRecord();
		}
		
		public static void ShareRecord() {
			PendingShare = false;
			StarkSDK.API.GetStarkGameRecorder().ShareVideo(
				dict => {},
				errorMsg => {},
				() => {}
			);
		}
		
		public static void CreateRewardedAd(Action onComplete) {
			StarkSDK.API.GetStarkAdManager().ShowVideoAdWithId("",
				ret => {
					if (ret) {
						onComplete?.Invoke();
					}
				});
		}
		
		public static string GetPrefsString(string key, string defaultValue) {
			return StarkSDK.API.PlayerPrefs.GetString("key", defaultValue);
		}
		
		public static void SetPrefsString(string key, string value) {
			StarkSDK.API.PlayerPrefs.SetString(key, value);
		}
	}

}
