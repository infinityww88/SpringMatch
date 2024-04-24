using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SpringMatch.UI {
	
	public class LevelProgress : MonoBehaviour
	{
		public Image _level0;
		public Image _level1;
		
		public void NexLevel() {
			_level0.gameObject.SetActive(false);
			_level1.gameObject.SetActive(true);
		}

		public void Restart() {
			_level0.gameObject.SetActive(true);
			_level1.gameObject.SetActive(false);
		}
	}

}
