using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using UnityEngine.Networking;

namespace SpringMatch.HotRes {
	
	public class HotResManager : MonoBehaviour
	{
		public static HotResManager Inst { get; private set; }
		
		public ResourcePackage DefaultPkg { get; private set; }
		
		public const string DEFAULT_PKG = "DefaultPackage";
		
		// Start is called before the first frame update
		void Awake()
		{
			if (Inst != null) {
				Destroy(gameObject);
			}
			Inst = this;
			DontDestroyOnLoad(gameObject);
			Init();
		}
		
		public async UniTask UpdateResource(DownloaderOperation.OnDownloadProgress onProgress) {
			#if UNITY_EDITOR
			await InitPkgEditor(DEFAULT_PKG);
			#else
			await InitRemote(DEFAULT_PKG);
			#endif
			string version = await UpdatePackageVersion(DEFAULT_PKG);
			await UpdatePackageManifest(DEFAULT_PKG, version);
			await DownloadPatch(DEFAULT_PKG,
				null,
				onProgress,
				null,
				null);
		}
		
		#region Init
		
		private IEnumerator InitializeYooAsset(string pkgName)
		{
			var pkg = YooAssets.GetPackage(pkgName);
			var initParameters = new OfflinePlayModeParameters();
			yield return pkg.InitializeAsync(initParameters);
		}
		
		private void Init() {
			YooAssets.Initialize();
			DefaultPkg = YooAssets.CreatePackage("DefaultPackage");
			YooAssets.SetDefaultPackage(DefaultPkg);
		}
		
		private async UniTask InitPkgEditor(string pkgName) {
			var pkg = YooAssets.GetPackage(pkgName);
			var initParameters = new EditorSimulateModeParameters();
			var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
			initParameters.SimulateManifestFilePath = simulateManifestFilePath;
			await pkg.InitializeAsync(initParameters);
		}
		
		private async UniTask InitPkgOffline(string pkgName) {
			var pkg = YooAssets.GetPackage(pkgName);
			var initParameters = new OfflinePlayModeParameters();
			await pkg.InitializeAsync(initParameters);
		}
		
		private async UniTask InitRemote(string pkgName) {
			var pkg = YooAssets.GetPackage(pkgName);
			string cdn = PrefsManager.GetString(PrefsManager.CDN, Global.DEFAULT_CDN);;
			var initParameters = new HostPlayModeParameters();
			initParameters.BuildinQueryServices = new BuildinQueryServices();
			initParameters.RemoteServices = new RemoteServices(new Uri(new Uri(cdn), "Resource").ToString());
			var initOperation = pkg.InitializeAsync(initParameters);
			await initOperation;
			if (initOperation.Status == EOperationStatus.Succeed) {
				Debug.Log($"Package {pkgName} init succeed");
			}
			else {
				var msg = $"Package {pkgName} init failed: {initOperation.Error}";
				Debug.LogWarning(msg);
				throw new Exception(msg);
			}
		}
		
		#endregion
		
		#region UpdatePackage
		
		private async UniTask<string> UpdatePackageVersion(string pkgName) {
			var pkg = YooAssets.GetPackage(pkgName);
			var operation = pkg.UpdatePackageVersionAsync();
			await operation;
			
			if (operation.Status == EOperationStatus.Succeed) {
				Debug.Log($"Updated package Version: {operation.PackageVersion}");
				return operation.PackageVersion;
			}
			else {
				var msg = $"Update pkg {pkgName} failed: {operation.Error}";
				Debug.LogWarning(msg);
				throw new Exception(msg);
			}
		}
		
		private async UniTask UpdatePackageManifest(string pkgName, string version) {
			var pkg = YooAssets.GetPackage(pkgName);
			var operation = pkg.UpdatePackageManifestAsync(version, true);
			await operation;
			
			if (operation.Status == EOperationStatus.Succeed) {
				Debug.Log($"Update pkg {pkgName} manifest succeed");
			}
			else {
				var msg = $"Update pkg {pkgName} failed: {operation.Error}";
				Debug.LogWarning(msg);
				throw new Exception(msg);
			}
		}
		
		#endregion
		
		#region Download
		
		private async UniTask DownloadPatch(string pkgName,
			DownloaderOperation.OnDownloadError onError,
			DownloaderOperation.OnDownloadProgress onProgress,
			DownloaderOperation.OnDownloadOver onOver,
			DownloaderOperation.OnStartDownloadFile onStart)
		{
			int downloadingMaxNum = 10;
			int failedTryAgain = 3;
			var pkg = YooAssets.GetPackage(pkgName);
			var downloader = pkg.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
			
			if (downloader.TotalDownloadCount == 0) {
				return;
			}
			
			int totalDownloadCount = downloader.TotalDownloadCount;
			long totalDownloadBytes = downloader.TotalDownloadBytes;
			
			downloader.OnDownloadErrorCallback = onError;
			downloader.OnDownloadProgressCallback = onProgress;
			downloader.OnStartDownloadFileCallback = onStart;
			downloader.OnDownloadOverCallback = onOver;
			
			downloader.BeginDownload();
			await downloader;
			
			if (downloader.Status == EOperationStatus.Succeed) {
				Debug.Log($"Download pkg {pkgName} succeed");
			}
			else {
				var msg = $"Download pkg {pkgName} failed: {downloader.Error}";
				Debug.LogWarning(msg);
				throw new Exception(msg);
			}
		}
		
		#endregion
	}

}
