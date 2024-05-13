using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Coffee.UIExtensions;
using System.Threading;

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
		private float moveDuration;
		[SerializeField]
		private Level levelPrefab;
		
		private int goldNum;
		private int heartNum;
		
		private Level currLevel = null;
		
		public bool PendInteract { get; set; } = false;
		
		public bool GameOver { get; private set; } = false;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			Screen.SetResolution(450, 900, false);
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
			GameOver = true;
		}
		
		public void OnLevelPass() {
			EffectManager.Inst.PlayLevelPassEffect();
			SwitchLevel();
		}
		
		public void BackToEditor() {
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Level.Inst.Load();
			currLevel = Level.Inst;
		}
		
		[Button]
		public async UniTaskVoid SwitchLevel() {
			await UniTask.Delay(3000);
			Debug.Log("hello, world");
			currLevel.transform.Translate(left.localPosition, Space.Self);
			Camera.main.transform.Translate(left.localPosition, Space.Self);
			var nextLevel = Instantiate(levelPrefab);
			nextLevel.Load();
			
			var token = gameObject.GetCancellationTokenOnDestroy();
			
			await Camera.main.transform.DOLocalMoveX(0, moveDuration)
				.WithCancellation(token);
			Destroy(currLevel.gameObject);
			currLevel = nextLevel;
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
