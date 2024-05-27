using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace SpringMatch {
	
	public class ShareManager : MonoBehaviour
	{
		public static ShareManager Inst;
		[SerializeField]
		private Texture2D sharedImage;
		[SerializeField]
		private string sharedLink;
		[SerializeField]
		private string sharedText;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
		}
		
		public bool IsFacebookAvailable => SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Facebook);
		public bool IsTwitterAvailable => SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Twitter);
		public bool IsWhatsAppAvailable => SocialShareComposer.IsComposerAvailable(SocialShareComposerType.WhatsApp);
		
		public void ShareFacebook(System.Action onSuccess, System.Action onFailed) {
			UI.UIVariable.Inst.ShowToast("Share with facebook");
			onSuccess?.Invoke();
			return;
			if (!IsFacebookAvailable) {
				return;
			}
			SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Facebook);
			composer.AddImage(sharedImage);
			composer.AddURL(URLString.URLWithPath(sharedLink));
			composer.SetCompletionCallback((result, error) => {
    Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
			});
			composer.Show();
		}
		
		public void ShareTwitter(System.Action onSuccess, System.Action onFailed) {
			if (!IsTwitterAvailable) {
				Debug.Log("twitter share is not available");
				return;
			}
			
			SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Twitter);
			composer.SetText(sharedText);
			composer.AddImage(sharedImage);
			composer.AddURL(URLString.URLWithPath(sharedLink));
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
		
		public void ShareWhatsApp(System.Action onSuccess, System.Action onFailed) {
			if (!IsWhatsAppAvailable) {
				return;
			}
			SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.WhatsApp);
			composer.SetText("Share text");
			composer.AddImage(sharedImage);
			composer.AddURL(URLString.URLWithPath(sharedLink));
			composer.SetCompletionCallback((result, error) => {
    Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
			});
			composer.Show();
		}
	}

}
