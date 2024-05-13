﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch.UI {
	
	public class UIVariable : MonoBehaviour
	{
		public static UIVariable Inst;
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
		}
		
		public RectTransform shopDialog;
		public RectTransform itemRequestDialog;
		public RectTransform refillLifeDialog;
		public RectTransform outOfLifeDialog;
		public RectTransform outOfSpaceDialog;
		public RectTransform giveupConfirmDialog;
		public RectTransform SettingDialog;
	}
}
