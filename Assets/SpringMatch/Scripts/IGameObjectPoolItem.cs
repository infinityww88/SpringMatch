using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpringMatch {
	public interface IGameObjectPoolItem
	{
		GameObject OnCreate();
		void OnGet(GameObject o);
		void OnRelease(GameObject o);
	}
}
