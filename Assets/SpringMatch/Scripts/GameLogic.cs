using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Coffee.UIExtensions;

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
		[SerializeField]
		private UIParticle mergeEffect;
		[SerializeField]
		private UIParticle levelPassEffect;
		
		private Level currLevel = null;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			Screen.SetResolution(450, 900, false);
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			//MsgBus.onSpringMerge += OnSpringMerge;
			MsgBus.onLevelPass += OnLevelPass;
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			MsgBus.onLevelPass -= OnLevelPass;
			//MsgBus.onSpringMerge -= OnSpringMerge;
		}
		
		public void OnSpringMerge() {
			//mergeEffect.Play();
		}
		
		public void OnLevelPass() {
			levelPassEffect.Play();
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
			currLevel.Done();
			var token = gameObject.GetCancellationTokenOnDestroy();
			var nextLevel = Instantiate(levelPrefab, right.position, right.rotation);
			nextLevel.Load();
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
		
		public void PlayEliminateEffect(Vector3 pos) {
			Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				mergeEffect.rectTransform.parent.GetComponent<RectTransform>(),
				screenPos, null, out Vector2 localPosition);
			mergeEffect.rectTransform.anchoredPosition = localPosition;
			mergeEffect.Play();
		}
	}

}
