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
		private float randomSpringInterval = 0.5f;
		[SerializeField]
		private int randomSpringTimes = 5;
		[SerializeField]
		private UI.LevelProgress levelProgress2, levelProgress3;
		[SerializeField]
		private IntVariable refillLiveInterval;
		[SerializeField]
		private VisualTweenSequence.TweenSequence startPlayEffect;
		[SerializeField]
		private TextAsset ase_key, ase_iv;
		[SerializeField]
		private Collection<Object> groundMats;
		[SerializeField]
		private GameObject ground;
		[SerializeField]
		private Transform numInfoRoot;
		
		[SerializeField]
		private LevelPrefabConfig levelPrefabs;
		
		[SerializeField]
		private UI.ItemButton revokeButton, shiftButton, randomButton;
		
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
			Global.PendInteract = false;
			Global.GameState = Global.EGameState.Ready;
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
			Global.GameState =	Global.EGameState.Play;
			startPlayEffect.Play();
			EffectManager.Inst.PlayStartPlay();
			Camera.main.transform.DOMove(Level.Inst.CameraPlayPos, 0.5f);
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			MsgBus.onLevelPass += OnLevelPass;
			MsgBus.onLevelFailed += OnLevelFailed;
			MsgBus.onElimiteStringStart += onElimiteString;
			MsgBus.onInvalidPick += onInvalidPick;
			MsgBus.onValidPick += onValidPick;
			MsgBus.onToSlot += OnToSlot;
			MsgBus.onRevoke += OnRevoke;
			MsgBus.onShift += OnShift;
			MsgBus.onPickup += OnPickup;
			MsgBus.onPurchaseSuccess += OnPurchaseSuccess;
			MsgBus.onPurchaseFailed += OnPurchaseFailed;
		}
		
		void OnPickup(Spring spring) {
			if (Global.GameState == Global.EGameState.Ready && Global.PendInteract == false) {
				StartPlay();
			}
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			MsgBus.onLevelPass -= OnLevelPass;
			MsgBus.onLevelFailed -= OnLevelFailed;
			MsgBus.onElimiteStringStart -= onElimiteString;
			MsgBus.onInvalidPick -= onInvalidPick;
			MsgBus.onToSlot -= OnToSlot;
			MsgBus.onRevoke -= OnRevoke;
			MsgBus.onShift -= OnShift;
			MsgBus.onPickup -= OnPickup;
			MsgBus.onPurchaseSuccess -= OnPurchaseSuccess;
			MsgBus.onPurchaseFailed -= OnPurchaseFailed;
		}
		
		public void OnLevelFailed() {
			UI.UIVariable.Inst.outOfSpaceDialog.gameObject.SetActive(true);
		}
		
		public void OnLevelPass() {
			SwitchLevel().Forget();
		}
		
		bool VibrateOn => PrefsManager.GetBool(PrefsManager.VIBRATE_ON, true);
		
		public void onElimiteString(Spring spring) {
			if (VibrateOn) {
				VibrationManager.Inst.VibrateEliminate();
			}
		}
		
		public void onInvalidPick(Spring spring) {
			
		}
		
		public void onValidPick(Spring spring) {
			if (VibrateOn) {
				VibrationManager.Inst.VibratePop();
			}
		}
		
		public void OnToSlot(Spring spring) {
		}
		
		public void OnRevoke(Spring spring) {
		}
		
		public void OnShift(int num) {
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
			},
			() => {
				UI.UIVariable.Inst.ShowToast("AD is not available now. Try again later.");
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
		
		string ReadLevelsFile(string path) {
			var fullPath = Path.Join(Application.persistentDataPath, "levels", path);
			var data = File.ReadAllBytes(fullPath);
			return Utils.Decrypt(data, ase_key.bytes, ase_iv.bytes);
		}
		
		void LoadLevelConfig() {
			var text = ReadLevelsFile("levelConfig.json");
			levelConfig = JsonConvert.DeserializeObject<LevelConfig>(text);
		}
		
		public void SetCamera() {
			Camera.main.transform.position = Level.Inst.CameraStartPos;
		}
		
		public void NextLevel() {
			Replay();
		}
		
		public void DoubleGold() {
			AdManager.Inst.ShowRewardedAd(() => {
				RewardGold(UI.UIVariable.Inst.levelPassGoldReward.Value);
			},
			() => {
				UI.UIVariable.Inst.ShowToast("AD is not available now. Try again later.");
			});
		}
		
		[Button]
		void RewardGold(int num) {
			PrefsManager.Inst.GoldNum += num;
			UI.UIVariable.Inst.rewardGoldEffect.GetComponent<VisualTweenSequence.TweenSequence>().Play();
		}
		
		private Level LoadSubLevel() {
			Utils.ClearChildren(numInfoRoot);
			var levelIndex = PrefsManager.Inst.LevelIndex;
			levelIndex %= levelConfig.levels.Count;
			var levelMeta = levelConfig.levels[levelIndex];
			var subLevelMeta = levelMeta.subLevels[subLevelIndex];
			var text = ReadLevelsFile($"{subLevelMeta.fileName}.json");
			var levelData = JsonConvert.DeserializeObject<LevelData>(text);
			var level = LoadLevelAsset(levelData.row, levelData.col);
			level.LoadData(levelData);
			
			levelIndex = PrefsManager.Inst.LevelIndex;
			int matIndex = levelIndex % groundMats.Count;
			Material mat = (Material)groundMats[matIndex];
			ground.GetComponent<Renderer>().material = mat;
			
			return level;
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			bool subPass = true;
			
			if (subLevelIndex + 1 == SubLevelNum) {
				subPass = false;
			}
			
			EffectManager.Inst.PlayLevelPassEffect(subPass);
			
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
			numInfoRoot.gameObject.SetActive(false);
			
			var token = gameObject.GetCancellationTokenOnDestroy();
			
			await Camera.main.transform.DOMove(nextLevel.CameraPlayPos, moveDuration)
				.WithCancellation(token);
			Destroy(currLevel.gameObject);
			numInfoRoot.gameObject.SetActive(true);
			currLevel = nextLevel;
		}
		
		public void OnPurchaseSuccess(string productId) {
			UI.UIVariable.Inst.shopBundleGet.OnPurchaseSuccess(productId);
			UI.UIVariable.Inst.shopGoldGet.OnPurchaseSuccess(productId);
			UI.UIVariable.Inst.shopDialog.SetActive(false);
		}
		
		public void OnPurchaseFailed(string productId, int errorCode) {
			if (errorCode != 0) {
				UI.UIVariable.Inst.ShowToast("Purchase Failed. Please try again.");
			}
		}
		
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
			DOTween.Kill(this);
		}
		
		public void RandomSprings() {
			if (Level.Inst.RemainSpring() == 0) {
				return;
			}
			var springs = Level.Inst.GetAllSprings();
			Level.Inst.RandomType(springs);
			var oldState = Global.GameState;
			Global.GameState =	Global.EGameState.Pause;
			DOTween.Sequence()
				.AppendCallback(() => Level.Inst.RandomType(springs))
				.AppendInterval(randomSpringInterval)
				.SetLoops(randomSpringTimes, LoopType.Restart)
				.SetId(this)
				.OnComplete(() => Global.GameState = oldState);
		}
		
		public void RevokeSpring() {
			if (!Level.Inst.HasLastPickSpring) {
				return;
			}
			Level.Inst.RestoreLastPickupSpring();
		}
		
		public void Shift3Spring() {
			if (SlotManager.Inst.UsedSlotsNum == 0) {
				return;
			}
			Level.Inst.Shift3ToExtra();
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (Global.GameState == Global.EGameState.Play) {
				revokeButton.Valid = Level.Inst.HasLastPickSpring;
				shiftButton.Valid = SlotManager.Inst.UsedSlotsNum > 0;
				randomButton.Valid = Level.Inst.RemainSpring() > 0;
			}
		}
	}
}
