using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace SpringMatch.UI {
	
	public class Loading : MonoBehaviour
	{
		[SerializeField]
		private Button _settingButton, _navButton, _recordButton, _playButton;
		
		[SerializeField]
		private RectTransform _playButtonPos;
		
		[SerializeField]
		private TextMeshProUGUI _declareNote;
		
		[SerializeField]
		private Image _loadingBar;
		
		[SerializeField]
		private TextMeshProUGUI _loadingText;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Load();
		}
		
		Tween TweenLoadingText() {
			string[] loadingTexts = new string[] {"loading.", "loading..", "loading..."};
			int i = 0;
			return DOTween.Sequence().AppendCallback(() => {
				_loadingText.text = loadingTexts[i];
				i = (i + 1) % loadingTexts.Length;
			})
				.AppendInterval(0.3f)
				.SetTarget(_loadingText)
				.SetLoops(-1);
		}
		
		async UniTaskVoid Load() {
			_loadingBar.fillAmount = 0;
			var textTween = TweenLoadingText();
			await _loadingBar.DOFillAmount(1, 2f).SetTarget(_loadingBar);
			textTween.Kill();
			_loadingText.gameObject.SetActive(false);
			_playButton.gameObject.SetActive(true);
			_playButton.onClick.AddListener(OnPlay);
		}
		
		public void OnPlay() {
			Debug.Log("OnPlay");
			_playButton.onClick.RemoveListener(OnPlay);
			_playButton.onClick.AddListener(OnStartGame);
			_settingButton.gameObject.SetActive(true);
			_declareNote.DOFade(0, 0.3f).OnComplete(() => _declareNote.gameObject.SetActive(false));
			var t0 = _playButton.GetComponent<Transform>().DOMove(_playButtonPos.transform.position, 0.3f);
			var t1 = _navButton.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f);
			var t2 = _recordButton.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f);
		}
		
		public void OnStartGame() {
			Debug.Log("OnStartGame");
			_playButton.GetComponent<RectTransform>().DOAnchorPosY(_playButton.GetComponent<RectTransform>().anchoredPosition.y - 500f, 0.3f);
			_navButton.GetComponent<RectTransform>().DOAnchorPosY(_navButton.GetComponent<RectTransform>().anchoredPosition.y - 500f, 0.3f);
			_recordButton.GetComponent<RectTransform>().DOAnchorPosY(_recordButton.GetComponent<RectTransform>().anchoredPosition.y - 500f, 0.3f);
		}
	}

}
