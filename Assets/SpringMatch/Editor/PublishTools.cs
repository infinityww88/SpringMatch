using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SpringMatch {
	
	public static class PublishTools
	{
		[MenuItem("Tools/Publish/Copy")]
		public static void CopyHotUpdateAssembly() {
			File.Copy($"./HybridCLRData/HotUpdateDlls/Android/HotUpdateAssembly.dll",
				"./Assets/SpringMatch/Assembly/HotUpdateAssembly.dll.bytes", true);
			Debug.Log("Copy HotUpdateAssembly.dll");
		}
	}
}

