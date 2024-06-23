using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using SpringMatch.HotRes;
using YooAsset;
using QFSW.QC;
using System.Reflection;
using System;
using System.IO;
using System.IO.Compression;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.Networking;

namespace SpringMatch {
	
	public class Loading : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _loadingText;
		
		private Tween loadingTextTween;
		
		[SerializeField]
		private Button playButton, consoleButton;
		
		[SerializeField]
		private TextAsset aseKey, aseIV;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			bool devMode = PrefsManager.GetDevMode();
			playButton.gameObject.SetActive(devMode);
			consoleButton.gameObject.SetActive(devMode);
			QualitySettings.skinWeights =   SkinWeights.FourBones;
			TweenLoadingText();
			Load().Forget();
		}
		
		void TweenLoadingText() {
			string[] texts = new string[] {
				"Loading", "Loading.", "Loading..", "Loading..."
			};
			int index = 0;
			loadingTextTween = DOTween.Sequence()
				.AppendCallback(() => {
					_loadingText.text = texts[index++];
					index = index % texts.Length;
				})
				.AppendInterval(0.5f)
				.SetLoops(-1, LoopType.Restart)
				.SetTarget(_loadingText.gameObject);
		}
		
		async UniTask DownloadRes(string path) {
			string cdn = PrefsManager.GetString(PrefsManager.CDN, Global.DEFAULT_CDN);
			var uri = new Uri(new Uri(cdn), path);
			Debug.Log(uri);
			UnityWebRequest request = null;
			for (int i = 0; i < 3; i++) {
				request = UnityWebRequest.Get(uri);
				request.downloadHandler = new DownloadHandlerFile(Path.Join(Application.persistentDataPath, path));
				request.timeout = 15;
				await request.SendWebRequest();
				if (request.result == UnityWebRequest.Result.Success) {
					return;
				}
			}
			throw new Exception($"DownloadRes {path} failed {request.error}");
		}
		
		[Button]
		async UniTask DownloadContent() {
			try {
				var path = Path.Join(Application.persistentDataPath, "meta.json");
				await DownloadRes("meta.json");
				var data = File.ReadAllBytes(path);
				var text = Utils.Decrypt(data, aseKey.bytes, aseIV.bytes);
				Debug.Log(text);
				MetaInfo meta = JsonUtility.FromJson<MetaInfo>(text);
				PrefsManager.SetString(PrefsManager.CDN, meta.cdn);
				int localVersion = PrefsManager.GetInt(PrefsManager.VERSION, 0);
				Debug.Log($"online version {meta.version}, local version {localVersion}");
				if (meta.version > localVersion) {
					await DownloadLevels();
					PrefsManager.SetInt(PrefsManager.VERSION, meta.version);
				}
			} catch (Exception e) {
				Debug.LogWarning($"Download Content error {e}");
			}
		}
		
		[Button]
		async UniTask DownloadLevels() {
			var path = Path.Join(Application.persistentDataPath, "levels.zip");
			await DownloadRes("levels.zip");
			var dir = Path.Join(Application.persistentDataPath, "levels");
			if (Directory.Exists(dir)) {
				Directory.Delete(dir, true);
			}
			ZipFile.ExtractToDirectory(path, Path.GetDirectoryName(path), true);
		}
		
		async UniTaskVoid Load() {
			await DownloadContent();
			Debug.Log("Loaded Content");
			
			try {
				await HotResManager.Inst.UpdateResource(OnDownloadProgress);
			} catch (Exception e) {
				Debug.LogWarning($"UpdateResource error {e}");
			}
			
			var handle = YooAssets.LoadAssetAsync("HotUpdate_HotUpdateAssembly.dll");
			await handle;
			TextAsset textAsset = handle.AssetObject as TextAsset;
			var data = textAsset.bytes;
			Assembly assembly = Assembly.Load(data);
			
			Debug.Log("Loaded assembly");
			
			if (!PrefsManager.GetDevMode()) {
				var sceneHandle = YooAssets.LoadSceneAsync("HotScene_Play");
				await sceneHandle;
				Debug.Log("Loaded Play Scene");
			}
		}
		
		public void Play() {
			YooAssets.LoadSceneAsync("HotScene_Play");
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
