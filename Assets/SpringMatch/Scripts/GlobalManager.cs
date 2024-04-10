using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SpringMatch {
	public class GlobalManager : MonoBehaviour
	{
		private static GlobalManager _instance;
	
		public static GlobalManager Inst => _instance;
	
		//public GameObject sphereColliderPrefab;
	
		//public ObjectPool<GameObject> sphereColliderPool;
	
		// Start is called before the first frame update
		/*
		void Awake()
		{
		_instance = this;
		sphereColliderPool = new ObjectPool<GameObject>(NewSphereCollider,
		OnGetSphereCollider,
		OnReleaseSphereCollider,
		defaultCapacity: 100);
		}
    
		GameObject NewSphereCollider() {
		return Instantiate(sphereColliderPrefab);
		}
	
		void OnGetSphereCollider(GameObject go) {
		go.SetActive(true);
		}
	
		void OnReleaseSphereCollider(GameObject go) {
		go.SetActive(false);
		go.transform.SetParent(null);
		go.transform.position = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.transform.rotation = Quaternion.identity;
		}
		*/
	}
}
