using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace SpringMatch.HotUpdate {
	
	public class RemoteServices : IRemoteServices
	{
		private string _cdnAddr = "";
		
		public RemoteServices(string cdnAddr) {
			_cdnAddr = cdnAddr;
		}
		
		public string GetRemoteMainURL(string fileName) {
			return $"{_cdnAddr}/{fileName}";
		}
		
		public string GetRemoteFallbackURL(string fileName) {
			return $"{_cdnAddr}/{fileName}";
		}
	}
}

