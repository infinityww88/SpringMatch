using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarkSDKSpace;
using StarkSDKSpace.UNBridgeLib.LitJson;

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
	}

}
