using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class SettingManager : MonoBehaviour
	{
		[SerializeField]
		private UI.ToggleButton soundToggle, musicToggle, vibrateToggle;
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			Debug.Log($"sound {soundToggle} music {musicToggle} vibrate {vibrateToggle}");
			soundToggle.On = SDKManager.GetPrefsInt("SoundOn", 1) == 1;
			musicToggle.On = SDKManager.GetPrefsInt("MusicOn", 1) == 1;
			vibrateToggle.On = SDKManager.GetPrefsInt("VibrateOn", 1) == 1;
		}
		
		public void OnToggleSound(bool val) {
			EffectManager.Inst.EnableSound(val);
		}
		
		public void OnToggleMusic(bool val) {
			EffectManager.Inst.EnableMusic(val);
		}
		
		public void OnToggleVibrate(bool val) {
			EffectManager.Inst.EnableVibrate(val);
		}
	}

}
