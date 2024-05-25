using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace SpringMatch {
	
	public class ShareManager : MonoBehaviour
	{
		public static ShareManager Inst;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
		}
		
		public void ShareGeneral() {
			ShareSheet shareSheet = ShareSheet.CreateInstance();
			shareSheet.AddText("Share the app");
			shareSheet.AddScreenshot();
			shareSheet.SetCompletionCallback((result, error) => {
    Debug.Log("Share Sheet was closed. Result code: " + result.ResultCode);
			});
			shareSheet.Show();
		}
		
		public void ShareTweet() {
			var available = SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Twitter);
			if (!available) {
				Debug.Log("twitter share is not available");
				return;
			}
			SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Twitter);
			composer.AddScreenshot();
			composer.AddURL(URLString.URLWithPath("https://www.google.com"));
			composer.SetCompletionCallback((result, error) => {
				Debug.Log("Social Share Composer was closed, Result Code : " + result.ResultCode);
			});
			composer.Show();
		}
	}

}
