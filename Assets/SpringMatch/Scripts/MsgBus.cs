using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpringMatch {
	
	public static class MsgBus
	{
		public static Action onLevelPass;
		public static Action onLevelFailed;
		public static Action onSpringMerge;
	}
}

