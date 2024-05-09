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
				},
				errorCallback: (code, err) => {
					Debug.Log($"Start Record {code} {err}");
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
				errorMsg => { Debug.Log($"share error {errorMsg}"); },
				() => {}
			);
		}
		
		public static void CreateRewardedAd(Action onComplete) {
			onComplete?.Invoke();
			/*
			StarkSDK.API.GetStarkAdManager().ShowVideoAdWithId("36nk6gsn5k2vqatsma",
				closeCallback: ret => {
					if (ret) {
						onComplete?.Invoke();
					}
				},
				errCallback: (code, errMsg) => {
					Debug.Log($"Rewarded Ad {code} {errMsg}");
				}
			);
			*/
		}
		
		public static string GetPrefsString(string key, string defaultValue) {
			return StarkSDK.API.PlayerPrefs.GetString(key, defaultValue);
		}
		
		public static void SetPrefsString(string key, string value) {
			StarkSDK.API.PlayerPrefs.SetString(key, value);
			StarkSDK.API.PlayerPrefs.Save();
		}
		
		public static int GetPrefsInt(string key, int defaultValue) {
			return StarkSDK.API.PlayerPrefs.GetInt(key, defaultValue);
		}
		
		public static void SetPrefInt(string key, int value) {
			StarkSDK.API.PlayerPrefs.SetInt(key, value);
			StarkSDK.API.PlayerPrefs.Save();
		}
	}

}
