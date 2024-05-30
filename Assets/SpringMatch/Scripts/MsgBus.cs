using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpringMatch {
	
	public static class MsgBus
	{
		public static Action onLevelPass;
		public static Action onLevelFailed;
		public static Action<Spring> onElimiteString;
		public static Action<Spring> onInvalidPick;
		public static Action<Spring> onToSlot;
		public static Action<Spring> onRevoke;
		public static Action<Spring> onPickup;
		public static Action<int> onShift;
	}
}

