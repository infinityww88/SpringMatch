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
		}
	}

}
