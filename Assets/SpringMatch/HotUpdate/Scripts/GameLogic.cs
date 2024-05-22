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
		private UI.LevelProgress levelProgress;
		[SerializeField]
		private IntVariable refillLiveInterval;
		[SerializeField]
		private VisualTweenSequence.TweenSequence startPlayEffect;
		
		private int levelIndex = 0;
		
		private int goldNum;
		private int heartNum;
		
		private Level currLevel = null;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			Global.PendInteract = true;
			Global.GameOver = true;
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
			MsgBus.onShift3 += onShift3;
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
			MsgBus.onShift3 -= onShift3;
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
		
		public void onShift3() {
			Debug.Log($"On Shift3");
		}
		
		public void BackHome() {
			YooAssets.LoadSceneAsync("HotScene_Play");
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
		
		[Button]
		public void Replay() {
			Destroy(currLevel.gameObject);
			var level = Instantiate(levelPrefab);
			var text = File.ReadAllText(Path.Join(Application.persistentDataPath, "levels", "level.json"));
			level.LoadJson(text);
			currLevel = level;
			levelProgress.ToLevel0();
			PrefsManager.Inst.DecHeartNum();
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			await UniTask.Delay(3000);
			if (levelIndex == 0) {
				levelProgress.ToLevel2();
			} else if (levelIndex == 1) {
				levelProgress.ToLevel3();
			}
			levelIndex++;
			
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
