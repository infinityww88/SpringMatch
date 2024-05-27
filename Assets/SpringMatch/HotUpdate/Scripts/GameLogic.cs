using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using SpringMatch;
using System.IO;
using ScriptableObjectArchitecture;
using YooAsset;

namespace SpringMatch {
	
	public class GameLogic : MonoBehaviour
	{
		public static GameLogic Inst;
		[SerializeField]
		private Transform left;
		[SerializeField]
		private float moveDuration;
		[SerializeField]
		private Level levelPrefab;
		[SerializeField]
		private UI.LevelProgress levelProgress2, levelProgress3;
		[SerializeField]
		private IntVariable refillLiveInterval;
		[SerializeField]
		private VisualTweenSequence.TweenSequence startPlayEffect;
		
		private int levelIndex = 0;
		
		private UI.LevelProgress levelProgress;
		
		private int goldNum;
		private int heartNum;
		
		private Level currLevel = null;
		
		[SerializeField]
		private int subLevelNum = 2;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			Global.PendInteract = true;
			Global.GameOver = true;
			SetupLevelProgress();
			#if UNITY_EDITOR
			YooAssets.Initialize();
			
			if (!YooAssets.ContainsPackage("DefaultPackage")) {
				Debug.Log("init default package");
				var defaultPkg = YooAssets.CreatePackage("DefaultPackage");
				YooAssets.SetDefaultPackage(defaultPkg);
				var initParameters = new EditorSimulateModeParameters();
				var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
				initParameters.SimulateManifestFilePath = simulateManifestFilePath;
				defaultPkg.InitializeAsync(initParameters);
			} else {
				Debug.Log("set default package");
				var defaultPkg = YooAssets.GetPackage("DefaultPackage");
				YooAssets.SetDefaultPackage(defaultPkg);
			}
			#endif
		}
		
		void SetupLevelProgress() {
			levelProgress2.gameObject.SetActive(subLevelNum == 2);
			levelProgress3.gameObject.SetActive(subLevelNum == 3);
			levelProgress = subLevelNum == 2 ? levelProgress2 : levelProgress3;
		}
		
		[Button]
		public void Replay() {
			Destroy(currLevel.gameObject);
			var level = Instantiate(levelPrefab);
			var text = File.ReadAllText(Path.Join(Application.persistentDataPath, "levels", "level.json"));
			level.LoadJson(text);
			currLevel = level;
			levelIndex = 0;
			levelProgress.ToLevel0();
			PrefsManager.Inst.DecHeartNum();
		}
	
		public void StartPlay() {
			PrefsManager.Inst.UpdateHeartNum();
			if (PrefsManager.Inst.HeartNum == 0) {
				UI.UIVariable.Inst.outOfLifeStartDialog.gameObject.SetActive(true);
				return;
			}
			PrefsManager.Inst.DecHeartNum();
			Global.PendInteract = false;
			Global.GameOver = false;
			startPlayEffect.Play();
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			MsgBus.onLevelPass += OnLevelPass;
			MsgBus.onLevelFailed += OnLevelFailed;
			MsgBus.onElimiteString += onElimiteString;
			MsgBus.onInvalidPick += onInvalidPick;
			MsgBus.onToSlot += OnToSlot;
			MsgBus.onRevoke += onRevoke;
			MsgBus.onShift += onShift;
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			MsgBus.onLevelPass -= OnLevelPass;
			MsgBus.onLevelFailed -= OnLevelFailed;
			MsgBus.onElimiteString -= onElimiteString;
			MsgBus.onInvalidPick -= onInvalidPick;
			MsgBus.onToSlot -= OnToSlot;
			MsgBus.onRevoke -= onRevoke;
			MsgBus.onShift -= onShift;
		}
		
		public void OnLevelFailed() {
			UI.UIVariable.Inst.outOfSpaceDialog.gameObject.SetActive(true);
		}
		
		public void OnLevelPass() {
			EffectManager.Inst.PlayLevelPassEffect();
			SwitchLevel().Forget();
		}
		
		public void onElimiteString(Spring spring) {
			Debug.Log($"Eliminate {spring.Type}");
		}
		
		public void onInvalidPick(Spring spring) {
			Debug.Log("Invalid Pick");
		}
		
		public void OnToSlot(Spring spring) {
			Debug.Log("To Slot");
		}
		
		public void onRevoke(Spring spring) {
			Debug.Log($"On Revoke {spring.Type}");
		}
		
		public void onShift(int num) {
			Debug.Log($"On Shift {num}");
		}
		
		public void BackHome() {
			YooAssets.LoadSceneAsync("HotScene_Play");
		}
		
		public void PlayOnByGold() {
			if (PrefsManager.Inst.GoldNum < UI.UIVariable.Inst.playOnGoldCost.Value) {
				UI.UIVariable.Inst.shopDialog.SetActive(true);
			}
			else {
				PrefsManager.Inst.GoldNum -= UI.UIVariable.Inst.playOnGoldCost.Value;
				Level.Inst.Shift3ToExtra();
				UI.UIVariable.Inst.outOfSpaceDialog.SetActive(false);
			}
		}
		
		public void PlayOnByAd() {
			AdManager.Inst.ShowRewardedAd(() => {
				UI.UIVariable.Inst.outOfSpaceDialog.SetActive(false);
				Level.Inst.Shift1ToExtra();
			});
		}
		
		public void OnLoseLife() {
			PrefsManager.Inst.UpdateHeartNum();
			if (PrefsManager.Inst.HeartNum > 0) {
				Replay();
			} else {
				UI.UIVariable.Inst.outOfLifeDialog.gameObject.SetActive(true);
			}
		}
		
		public void ShiftOut3() {
			Level.Inst.Shift3ToExtra();
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			var path = Path.Join(Application.persistentDataPath, "levels", "level.json");
			Debug.Log($"Load level {path}");
			var text = File.ReadAllText(path);
			Level.Inst.LoadJson(text);
			currLevel = Level.Inst;
		}
		
		public void NextLevel() {
			Replay();
		}
		
		public void DoubleGold() {
			AdManager.Inst.ShowRewardedAd(() => {
				RewardGold(UI.UIVariable.Inst.levelPassGoldReward.Value);
			});
		}
		
		[Button]
		void RewardGold(int num) {
			PrefsManager.Inst.GoldNum += num;
			UI.UIVariable.Inst.rewardGoldEffect.GetComponent<VisualTweenSequence.TweenSequence>().Play();
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			await UniTask.Delay(3000);
			if (subLevelNum == 3 && levelIndex == 0) {
				levelProgress.ToLevel2();
			} else if (subLevelNum == 2 && levelIndex == 0 || subLevelNum == 3 && levelIndex == 1) {
				levelProgress.ToLevel3();
			}
			
			levelIndex++;
			
			if (levelIndex == subLevelNum) {
				RewardGold(UI.UIVariable.Inst.levelPassGoldReward.Value);
				UI.UIVariable.Inst.levelPass.SetActive(true);
				return;
			}
			
			currLevel.transform.Translate(left.localPosition, Space.Self);
			Camera.main.transform.Translate(left.localPosition, Space.Self);
			var nextLevel = Instantiate(levelPrefab);
			var text = File.ReadAllText(Path.Join(Application.persistentDataPath, "levels", "level.json"));
			nextLevel.LoadJson(text);
			
			var token = gameObject.GetCancellationTokenOnDestroy();
			
			await Camera.main.transform.DOLocalMoveX(0, moveDuration)
				.WithCancellation(token);
			Destroy(currLevel.gameObject);
			currLevel = nextLevel;
		}
	}
}
