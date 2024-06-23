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
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.Init();
		#endif
		DontDestroyOnLoad(gameObject);
	}
	
	[Command]
	public void Vibrate() {
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.Vibrate();
		#endif
	}
	
	[Command]
	public void VibrateMS(int ms) {
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.VibrateAndroid(ms);
		#endif
	}
	
	[Command]
	public void VibrateNope() {
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.VibrateNope();
		#endif
	}
	
	[Command]
	public void VibratePeek() {
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.VibratePeek();
		#endif
	}
	
	[Command]
	public void VibratePop() {
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.VibratePop();
		#endif
	}
	
	[Command("VibratePat")]
	public void VibratePattern(long[] pattern, int repeat) {
		#if UNITY_ANDROID || UNITY_IOS
		Vibration.VibrateAndroid(pattern, repeat);
		#endif
	}

	public async UniTask VibrateEliminate() {
		#if UNITY_ANDROID || UNITY_IOS
		for (int i = 0; i < num; i++) {
			Vibration.VibrateAndroid(duration);
			await UniTask.WaitForSeconds(interval / 1000f);
		}
		#endif
	}
	
	[Command]
	public async UniTask VibrateEliminate(int interval, int duration, int num) {
		#if UNITY_ANDROID || UNITY_IOS
		for (int i = 0; i < num; i++) {
			Vibration.VibrateAndroid(duration);
			await UniTask.WaitForSeconds(interval / 1000f);
		}
		#endif
	}
}
