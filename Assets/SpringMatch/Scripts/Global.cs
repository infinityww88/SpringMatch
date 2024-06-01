using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public static class Global
	{
		public enum EGameState {
			Ready,
			Play,
			Pause,
			Over
		}
		//public static string CDN = "https://d18ajfcy3fib2i.cloudfront.net";
		public static string CDN = "https://public-ce19f4f2-a8cf-4210-8209-82b441412ee0.s3.ap-northeast-1.amazonaws.com";
		public static bool PendInteract = false;
		public static EGameState GameState = EGameState.Ready;
	}
}

