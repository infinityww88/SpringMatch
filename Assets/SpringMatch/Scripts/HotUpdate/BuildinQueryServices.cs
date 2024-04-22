using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace SpringMatch.HotUpdate {
	
	public class BuildinQueryServices : IBuildinQueryServices
	{
		public bool Query(string packageName, string fileName, string fileCRC) {
			return false;
		}
	}

}
