using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using SpringMatch.HotUpdate;
using YooAsset;
using QFSW.QC;

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
		
		[Button]
		[Command]
		private void TestProgress() {
			HotResManager.Inst.UpdateResource(OnDownloadProgress);
		}
		
		[Button]
		[Command]
		private void TestRes() {
			var handle = YooAssets.LoadAssetSync("Levels_level_1.json");
			TextAsset textAsset = handle.AssetObject as TextAsset;
			Debug.Log(textAsset.text);
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
