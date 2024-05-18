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
			Global.PendInteract = false;
			Global.GameOver = false;
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			MsgBus.onLevelPass += OnLevelPass;
			MsgBus.onLevelFailed += OnLevelFailed;
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			MsgBus.onLevelPass -= OnLevelPass;
			MsgBus.onLevelFailed -= OnLevelFailed;
		}
		
		public void OnLevelFailed() {
			Debug.Log("Level Failed");
			Global.GameOver = true;
		}
		
		public void OnLevelPass() {
			EffectManager.Inst.PlayLevelPassEffect();
			SwitchLevel().Forget();
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
