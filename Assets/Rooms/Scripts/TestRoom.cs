using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;
using DG.Tweening;

namespace CustomRoom {
	
	public class TestRoom : MonoBehaviour
	{
		public Transform target;
		
		private Tweener tweener;
		
		[Button]
		void Tween() {
			tweener = transform.DOMove(target.position, 1).SetAutoKill(false);
		}
		
		[Button]
		void Backwards() {
			tweener.PlayBackwards();
		}
	}
}

