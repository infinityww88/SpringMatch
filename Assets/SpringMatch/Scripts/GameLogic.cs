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

namespace SpringMatch.UI {
	
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
		private RectTransform failedDialog;
		[SerializeField]
		private LevelProgress levelProgress;
		[SerializeField]
		private TextMeshProUGUI dateLabel;
		[SerializeField]
		private string[] levels;
		
		private int currLevelIndex = 0;
		
		private Level currLevel = null;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			//Screen.SetResolution(450, 900, false);
			var dt = System.DateTime.Now;
			dateLabel.text = $"{dt.Month}-{dt.Day}";
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
			LoadCameraView();
		}
		
		void LoadCameraView() {
			var path = Path.GetFullPath("cameraConfig.json");
			if (!File.Exists(path)) {
				return;
			}
			var cameraConfig = JsonUtility.FromJson<CameraConfig>(File.ReadAllText(path));
			
			Camera.main.transform.rotation =
				cameraConfig.HorzRotation * cameraConfig.VertRotation * cameraConfig.cameraRotation;
			Camera.main.transform.position = cameraConfig.cameraPosition;
		}
		
		public void OnLevelPass() {
			SwitchLevel();
		}
		
		public void OnLevelFail() {
			failedDialog.gameObject.SetActive(true);
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
			
			if (currLevelIndex == 1) {
				levelProgress.ToLevel2();
			} else if (currLevelIndex == 2) {
				levelProgress.ToLevel3();
			}
			
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
			Level.Inst.Shift3ToExtra();
		}
		
		public void Home() {
			SceneManager.LoadScene("Loading");
		}
		
		public void Restart() {
			SceneManager.LoadScene("Play");
		}
	}
}
