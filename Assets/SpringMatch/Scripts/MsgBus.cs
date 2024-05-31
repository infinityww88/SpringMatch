using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VoxelBusters.EssentialKit;
using VoxelBusters.CoreLibrary;

namespace SpringMatch {
	
	public static class MsgBus
	{
		public static Action onLevelPass;
		public static Action onLevelFailed;
		public static Action<Spring> onElimiteStringStart;
		public static Action<Spring> onElimiteStringEnd;
		public static Action<Spring> onInvalidPick;
		public static Action<Spring> onValidPick;
		public static Action<Spring> onToSlot;
		public static Action<Spring> onRevoke;
		public static Action<Spring> onPickup;
		public static Action<Dictionary<string, IBillingProduct>> onIAPInit;
		public static Action<int> onShift;
		public static Action<string> onPurchaseSuccess;
		public static Action<string, int> onPurchaseFailed;
	}
}

