using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace SpringMatch.HotRes {
	
	public class BuildinQueryServices : IBuildinQueryServices
	{
		public bool Query(string packageName, string fileName, string fileCRC) {
			Debug.Log($"BuildinQuery {packageName} {fileName}");
			return false;
		}
	}

}
