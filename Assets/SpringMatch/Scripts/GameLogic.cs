using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using SpringMatchEditor;
using System.IO;

namespace SpringMatch {
	
	public class GameLogic : MonoBehaviour
	{
		public static GameLogic Inst;
		[SerializeField]
		private RectTransform _requestRevokeItemDialog;
		[SerializeField]
		private RectTransform _requestShiftItemDialog;
		[SerializeField]
		private RectTransform _requestRandomItemDialog;
		[SerializeField]
		private Transform left;
		[SerializeField]
		private Transform right;
		[SerializeField]
		private float moveDuration;
		[SerializeField]
		private Level levelPrefab;
		
		private Level currLevel = null;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			Screen.SetResolution(450, 900, false);
		}
		
		public void BackToEditor() {
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Level.Inst.Load(LevelEditor.CurrEditLevel);
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
		
		public void PreLoadLevel() {
			
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			currLevel.Done();
			var token = gameObject.GetCancellationTokenOnDestroy();
			var nextLevel = Instantiate(levelPrefab, right.position, right.rotation);
			nextLevel.Load(LevelEditor.CurrEditLevel);
			await currLevel.transform.DOMoveX(left.position.x, moveDuration)
				.WithCancellation(token);
			Destroy(currLevel.gameObject);
			currLevel = nextLevel;
			await currLevel.transform.DOMoveX(0, moveDuration)
				.WithCancellation(token);
		}
		
		public void OnRequestRevokeItem() {
			_requestRevokeItemDialog.gameObject.SetActive(true);
		}
		
		public void OnGetRevokeItem() {
			Debug.Log("Get Revoke Item");
		}
		
		public void OnRequestShiftItem() {
			_requestShiftItemDialog.gameObject.SetActive(true);
		}
		
		public void OnGetShiftItem() {
			Debug.Log("Get Shift Item");
		}
		
		public void OnRequestRandomItem() {
			_requestRandomItemDialog.gameObject.SetActive(true);
		}
		
		public void OnGetRandomItem() {
			Debug.Log("Get Random Item");
		}
	}

}
