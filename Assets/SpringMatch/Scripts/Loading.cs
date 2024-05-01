using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using SpringMatch.HotRes;
using YooAsset;
using QFSW.QC;
using HybridCLR;
using System.Reflection;
using System;
using Cysharp.Threading.Tasks;

namespace SpringMatch {
	
	public class Loading : MonoBehaviour
	{
		[SerializeField]
		private Slider _progressBar;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			_progressBar.value = 0;
		}
		
		[Command]
		private void TestProgress() {
			HotResManager.Inst.UpdateResource(OnDownloadProgress);
		}
		
		[Command]
		private void TestRes() {
			var handle = YooAssets.LoadAssetSync("HotUpdate_HotUpdateAssembly.dll");
			TextAsset textAsset = handle.AssetObject as TextAsset;
			var data = textAsset.bytes;
			Assembly assembly = Assembly.Load(data);
			Type type = assembly.GetType("SpringMatch.HotUpdate.TestHotUpdate");
			type.GetMethod("Info").Invoke(null, null);
		}
		
		[Command]
		private async UniTaskVoid TestHotScene() {
			var handle = YooAssets.LoadSceneAsync("HotScene_TestHotScene");
			await handle;
			Debug.Log("Loaded Test Hot Scene");
		}
		
		void OnDownloadProgress(
			int totalDownloadCount,
			int currentDownloadCount,
			long totalDownloadBytes,
			long currentDownloadBytes)
		{
			Debug.Log($"downloading: {currentDownloadCount}/{totalDownloadCount} {currentDownloadBytes}/{totalDownloadBytes}");
		}
	}

}
