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
		
		public string StoreLink => PrefsManager.GetString(PrefsManager.Store_LINK, "");
		
		public void ShareTwitter(System.Action onSuccess, System.Action onFailed) {
			var available = SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Twitter);
			if (!available) {
				Debug.Log("twitter share is not available");
				return;
			}
			SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Twitter);
			composer.AddScreenshot();
			composer.AddURL(URLString.URLWithPath(StoreLink));
			composer.SetCompletionCallback((result, error) => {
				if (result.ResultCode == SocialShareComposerResultCode.Done) {
					Debug.Log("Social Share Composer was Done");
					onSuccess?.Invoke();
				}
				else {
					Debug.Log("Social Share Composer was Cancel");
					onFailed?.Invoke();
				}
				
			});
			composer.Show();
		}
	}

}
