using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpringMatch.UI {
	
	public class Loading : MonoBehaviour
	{
		[SerializeField]
		private LevelProgress _levelProgress;
		
		[SerializeField]
		private Button _settingButton, _navButton, _recordButton, _playButton;
		
		[SerializeField]
		private TextMeshProUGUI _declareNote;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			//_levelProgress.
		}
	}

}
