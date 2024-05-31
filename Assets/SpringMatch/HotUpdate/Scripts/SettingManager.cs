using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class SettingManager : MonoBehaviour
	{	
		[SerializeField]
		private UI.ToggleButton soundToggle, vibrateToggle;
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			bool soundOn = PrefsManager.GetBool(PrefsManager.SOUND_ON, true);
			Debug.Log(soundOn);
			bool vibrateOn = PrefsManager.GetBool(PrefsManager.VIBRATE_ON, true);
			soundToggle.SetOn(soundOn);
			vibrateToggle.SetOn(vibrateOn);
		}
		
		public void OnToggleSound(bool val) {
			PrefsManager.SetBool(PrefsManager.SOUND_ON, val);
		}
		
		public void OnToggleVibrate(bool val) {
			PrefsManager.SetBool(PrefsManager.VIBRATE_ON, val);
		}
	
	}

}
