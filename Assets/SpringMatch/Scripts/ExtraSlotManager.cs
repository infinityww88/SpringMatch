﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	
	public class ExtraSlotManager : MonoBehaviour
	{
		public static ExtraSlotManager Inst { get; private set; }
	
		[SerializeField]
		private float _footWidth = 2;
		[SerializeField]
		private float _height = 2;
		
		private ExtraSlot[] _extraSlot;
	
		// Start is called before the first frame update
		void Awake()
		{
			Inst = this;
			_extraSlot = GetComponentsInChildren<ExtraSlot>();
		}
		
		public void AddSprings(Spring[] springs) {
			int n = Mathf.Min(springs.Length, 3);
			for (int i = 0; i < n; i++) {
				_extraSlot[i].Spring = springs[i];
				springs[i].SetExtra(i, GetSlotFoot0Pos(i), GetSlotFoot1Pos(i), _height);
			}
		}
    
		public Vector3 GetSlotPos(int index) {
			return _extraSlot[index].transform.position;
		}
	
		public Vector3 GetSlotFoot0Pos(int index) {
			return GetSlotPos(index) + Vector3.left * _footWidth / 2;
		}
	
		public Vector3 GetSlotFoot1Pos(int index) {
			return GetSlotPos(index) + Vector3.right * _footWidth / 2;
		}
	
		public float Height => _height;
	}

}
