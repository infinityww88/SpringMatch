using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using SpringMatch.UI;
using System;

namespace SpringMatch {
	
	public class GameLogic : MonoBehaviour
	{
		public static GameLogic Inst;

		[SerializeField]
		private Transform left;
		[SerializeField]
		private Transform right;
		[SerializeField]
		private float moveDuration;
		[SerializeField]
		private Level levelPrefab;

		[SerializeField]
		private LevelProgress levelProgress;
		[SerializeField]
		private TextMeshProUGUI dateLabel;
		[SerializeField]
		private string[] levels;
		[SerializeField]
		private GameObject settingDialogHome;
		[SerializeField]
		private GameObject settingDialogInGame;
		
		[SerializeField]
		private Button startGameButton, levelPassButton;
		
		[SerializeField]
		private TextMeshProUGUI levelPassInfo;
		
		[SerializeField]
		private GameObject recoverDialog;
		
		[SerializeField]
		private GameObject passDialog;
		
		[SerializeField]
		private GameObject failDialog;
		
		[SerializeField]
		private RectTransform numInfoRoot;
		
		[SerializeField]
		private Tips tips;
		
		public static bool Pending { get; set; } = true;
		
		private int currLevelIndex = 0;
		
		private Level currLevel = null;
		
		private int failedNum = 0;
		
		public bool GameStart { get; set; }
		
		private DateTimeOffset lastPassTime = DateTimeOffset.MinValue;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			var dt = System.DateTime.Now;
			dateLabel.text = $"{dt.Month}-{dt.Day}";
			
			var s = PlayerPrefs.GetString("lastPassTime", "");
			if (s != "") {
				lastPassTime = DateTimeOffset.Parse(s);
			}
		}
		
		[Button]
		void Test(int secOffset) {
			var now = DateTimeOffset.Now;
			var z = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
			z = z.AddSeconds(secOffset);
			PlayerPrefs.SetString("lastPassTime", z.ToString());
		}
		
		private bool LevelPassToday() {
			var now = DateTimeOffset.Now;
			var z = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
			return lastPassTime >= z;
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			Level.OnLevelPass += OnLevelPass;
			Level.OnLevelFail += OnLevelFail;
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			Level.OnLevelPass -= OnLevelPass;
			Level.OnLevelFail -= OnLevelFail;
		}
		
		public void BackToEditor() {
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			//Level.Inst.Load(SpringMatchEditor.LevelEditor.CurrEditLevel);
			Level.Inst.Load(levels[currLevelIndex]);
			currLevel = Level.Inst;
			//LoadCameraView();
		}
		
		void LoadCameraView() {
			var path = Path.GetFullPath("cameraConfig.json");
			if (!File.Exists(path)) {
				return;
			}
			/*
			var cameraConfig = JsonUtility.FromJson<CameraConfig>(File.ReadAllText(path));
			
			Camera.main.transform.rotation =
				cameraConfig.HorzRotation * cameraConfig.VertRotation * cameraConfig.cameraRotation;
			Camera.main.transform.position = cameraConfig.cameraPosition;
			*/
		}
		
		public void OnLevelPass() {
			if (currLevelIndex == 0) {
				SwitchLevel();
			}
			else {
				passDialog.SetActive(true);
				lastPassTime = DateTimeOffset.Now;
				PlayerPrefs.SetString("lastPassTime", DateTimeOffset.Now.ToString());
			}
		}
		
		public void OnLevelFail() {
			Pending = true;
			if (failedNum == 0) {
				recoverDialog.SetActive(true);
			} else {
				failDialog.SetActive(true);
			}
			failedNum++;
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			await UniTask.WaitForSeconds(2f);
			currLevel.Done();
			var token = gameObject.GetCancellationTokenOnDestroy();
			var nextLevel = Instantiate(levelPrefab);
			//nextLevel.Load(SpringMatchEditor.LevelEditor.CurrEditLevel);
			currLevelIndex++;
			if (currLevelIndex >= levels.Length) {
				Debug.Log("LevelPass");
				return;
			}
			
			levelProgress.NexLevel();
			
			nextLevel.Load(levels[currLevelIndex]);
			
			var offset = left.transform.position.x;
			currLevel.transform.Translate(Vector3.right * offset, Space.World);
			Camera.main.transform.Translate(Vector3.right * offset, Space.World);
			await Camera.main.transform.DOMoveX(0, moveDuration).SetEase(Ease.Linear).WithCancellation(token);
			Destroy(currLevel.gameObject);
			currLevel = nextLevel;
		}
		
		public void Revoke(ItemButton button) {
			if (Level.Inst.RestoreLastPickupSpring()) {
				button.UseItem();
			}
		}
		
		public void Shift(ItemButton button) {
			if (Level.Inst.Shift3ToExtra()) {
				button.UseItem();
			}
		}
		
		public void Random(ItemButton button) {
			Level.Inst.RandomAllSpringTypes();
			button.UseItem();
		}
		
		public void Recover() {
			Pending = false;
			Level.Inst.Shift3ToExtra();
		}
		
		public void Home() {
			SceneManager.LoadScene("Play");
		}
		
		[Button]
		public void Restart() {
			currLevelIndex = 0;
			
			Pending = false;
			failedNum = 0;
			Destroy(currLevel.gameObject);
			Utils.ClearChildren(numInfoRoot);
			Instantiate(levelPrefab);
			Level.Inst.Load(levels[currLevelIndex]);
			currLevel = Level.Inst;
			levelProgress.Restart();
		}
		
		public void ShowSetting() {
			GameObject o = GameStart ? settingDialogInGame : settingDialogHome;
			o.SetActive(true);
		}
		
		[Button]
		public void ShowTips(string info) {
			tips.ShowTips(info);
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (GameStart) {
				return;
			}
			if (LevelPassToday()) {
				startGameButton.gameObject.SetActive(false);
				levelPassButton.gameObject.SetActive(true);
				var dt = DateTimeOffset.Now;
				var z = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, dt.Offset);
				z = z.AddDays(1);
				var d = z - dt;
				levelPassInfo.text = $"{d.Hours:D2}:{d.Minutes:D2}:{d.Seconds:D2} left";
			}
			else {
				startGameButton.gameObject.SetActive(true);
				levelPassButton.gameObject.SetActive(false);
			}
		}
	}
}
