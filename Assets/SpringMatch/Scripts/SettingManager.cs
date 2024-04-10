using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class SettingManager : MonoBehaviour
	{
		public void OnToggleSound(bool val) {
			Debug.Log($"Setting sound {val}");
		}
		
		public void OnToggleMusic(bool val) {
			Debug.Log($"Setting music {val}");
		}
		
		public void OnToggleVibrate(bool val) {
			Debug.Log($"Setting vibrate {val}");
		}
		
		public void OnExitGame() {
			Debug.Log($"Exit Game");
		}
	}

}
