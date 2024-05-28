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
using Newtonsoft.Json;

namespace SpringMatch {
	
	public class GameLogic : MonoBehaviour
	{
		public static GameLogic Inst;
		[SerializeField]
		private Transform left;
		[SerializeField]
		private float moveDuration;
		[SerializeField]
		private UI.LevelProgress levelProgress2, levelProgress3;
		[SerializeField]
		private IntVariable refillLiveInterval;
		[SerializeField]
		private VisualTweenSequence.TweenSequence startPlayEffect;
		
		[SerializeField]
		private LevelPrefabConfig levelPrefabs;
		
		private int subLevelIndex = 0;
		
		private UI.LevelProgress levelProgress;
		
		private int goldNum;
		private int heartNum;
		
		private Level currLevel = null;
		
		private int SubLevelNum {
			get {
				var levelIndex = PrefsManager.Inst.LevelIndex;
				levelIndex %= levelConfig.levels.Count;
				var levelMeta = levelConfig.levels[levelIndex];
				return levelMeta.subLevels.Count;
			}
		}
		
		private LevelConfig levelConfig;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			Global.PendInteract = true;
			Global.GameOver = true;
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
			Debug.Log($"{SubLevelNum}");
			levelProgress2.gameObject.SetActive(SubLevelNum == 2);
			levelProgress3.gameObject.SetActive(SubLevelNum == 3);
			levelProgress = SubLevelNum == 2 ? levelProgress2 : levelProgress3;
		}
		
		[Button]
		public void Replay() {
			Destroy(currLevel.gameObject);
			subLevelIndex = 0;
			currLevel = LoadSubLevel();
			levelProgress.ToLevel0();
			PrefsManager.Inst.DecHeartNum();
			Camera.main.transform.position = Level.Inst.CameraPlayPos;
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
			Camera.main.transform.DOMove(Level.Inst.CameraPlayPos, 0.5f);
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
		
		private Level LoadLevelAsset(int row, int col) {
			string key = $"{row}x{col}";
			if (levelPrefabs.prefabs.ContainsKey(key)) {
				return Instantiate(levelPrefabs.prefabs[key]).GetComponent<Level>();
			}
			return null;
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			LoadLevelConfig();
			subLevelIndex = 0;
			SetupLevelProgress();
			currLevel = LoadSubLevel();
			SetCamera();
		}
		
		void LoadLevelConfig() {
			var text = File.ReadAllText(Path.Join(Application.persistentDataPath, "levels", "levelConfig.json"));
			levelConfig = JsonConvert.DeserializeObject<LevelConfig>(text);
		}
		
		public void SetCamera() {
			Camera.main.transform.position = Level.Inst.CameraStartPos;
		}
		
		public void NextLevel() {
			SetupLevelProgress();
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
		
		private Level LoadSubLevel() {
			var levelIndex = PrefsManager.Inst.LevelIndex;
			levelIndex %= levelConfig.levels.Count;
			var levelMeta = levelConfig.levels[levelIndex];
			var subLevelMeta = levelMeta.subLevels[subLevelIndex];
			var text = File.ReadAllText(Path.Join(Application.persistentDataPath, "levels", $"{subLevelMeta.fileName}.json"));
			var levelData = JsonConvert.DeserializeObject<LevelData>(text);
			var level = LoadLevelAsset(levelData.row, levelData.col);
			level.LoadData(levelData);
			return level;
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			await UniTask.Delay(3000);
			if (SubLevelNum == 3 && subLevelIndex == 0) {
				levelProgress.ToLevel2();
			} else if (SubLevelNum == 2 && subLevelIndex == 0 || SubLevelNum == 3 && subLevelIndex == 1) {
				levelProgress.ToLevel3();
			}
			
			subLevelIndex++;
			
			if (subLevelIndex == SubLevelNum) {
				PrefsManager.Inst.LevelIndex += 1;
				RewardGold(UI.UIVariable.Inst.levelPassGoldReward.Value);
				UI.UIVariable.Inst.levelPass.SetActive(true);
				return;
			}
			
			currLevel.transform.Translate(left.localPosition, Space.Self);
			Camera.main.transform.Translate(left.localPosition, Space.Self);
			
			var nextLevel = LoadSubLevel();
			
			var token = gameObject.GetCancellationTokenOnDestroy();
			
			await Camera.main.transform.DOMove(nextLevel.CameraPlayPos, moveDuration)
				.WithCancellation(token);
			Destroy(currLevel.gameObject);
			currLevel = nextLevel;
		}
		
		[Button]
		void TestConfig() {
			SubLevelMetaData subLevelMeta0 = new SubLevelMetaData();
			subLevelMeta0.fileName = "level";
			
			SubLevelMetaData subLevelMeta1 = new SubLevelMetaData();
			subLevelMeta1.fileName = "level";
			
			LevelMetaData levelMeta = new	LevelMetaData();
			levelMeta.subLevels = new	List<SubLevelMetaData>();
			levelMeta.subLevels.Add(subLevelMeta0);
			levelMeta.subLevels.Add(subLevelMeta1);
			
			LevelConfig config = new	LevelConfig();
			config.levels = new List<LevelMetaData>();
			config.levels.Add(levelMeta);
			
			Debug.Log(JsonConvert.SerializeObject(config));
		}
	}
}
