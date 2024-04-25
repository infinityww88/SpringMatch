using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class SettingManager : MonoBehaviour
	{
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
