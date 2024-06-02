using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using QFSW.QC;
using Cysharp.Threading.Tasks;

public class VibrationManager : MonoBehaviour
{
	public static VibrationManager Inst { get; private set; }
	
	[SerializeField]
	private int duration = 500, interval = 30, num = 30;
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		if (Inst != null) {
			Destroy(gameObject);
			return;
		}
		Inst = this;
		Vibration.Init();
		DontDestroyOnLoad(gameObject);
	}
	
	[Command]
	public void Vibrate() {
		Vibration.Vibrate();
	}
	
	[Command]
	public void VibrateMS(int ms) {
		Vibration.VibrateAndroid(ms);
	}
	
	[Command]
	public void VibrateNope() {
		Vibration.VibrateNope();
	}
	
	[Command]
	public void VibratePeek() {
		Vibration.VibratePeek();
	}
	
	[Command]
	public void VibratePop() {
		Vibration.VibratePop();
	}
	
	[Command("VibratePat")]
	public void VibratePattern(long[] pattern, int repeat) {
		Vibration.VibrateAndroid(pattern, repeat);
	}

	public async UniTask VibrateEliminate() {
		for (int i = 0; i < num; i++) {
			Vibration.VibrateAndroid(duration);
			await UniTask.WaitForSeconds(interval / 1000f);
		}
	}
	
	[Command]
	public async UniTask VibrateEliminate(int interval, int duration, int num) {
		for (int i = 0; i < num; i++) {
			Vibration.VibrateAndroid(duration);
			await UniTask.WaitForSeconds(interval / 1000f);
		}
	}
}
