using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;
using System;

namespace SpringMatch {
	
	[Serializable]
	public class PoolItem {
		public GameObject prefab;
		public int defaultCapacity = 10;
		public int maxSize = 100;
		public ObjectPool<GameObject> Pool { get; set; }
	}
	
	public class GameObjectsPool : SerializedMonoBehaviour
	{
		public static GameObjectsPool Inst { get; set; }
		
		[SerializeField]
		private Dictionary<string, PoolItem> _items = new	Dictionary<string, PoolItem>();
		
		// Awake is called when the script instance is being loaded.
		protected void Awake()
		{
			Inst = this;
			
			foreach (var entry in _items) {
				string key = entry.Key;
				PoolItem poolItem = entry.Value;
				var pool = new ObjectPool<GameObject>(MakeCreateFunc(key),
					actionOnGet: OnGet,
					actionOnRelease: OnRelease,
					defaultCapacity: 10,
					maxSize: 100);
				poolItem.Pool = pool;
			}
			
		}
		
		[Button]
		public GameObject Get(string key) {
			return _items[key].Pool.Get();
		}
		
		[Button]
		public void Release(string key, GameObject o) {
			_items[key].Pool.Release(o);
		}
		
		Func<GameObject> MakeCreateFunc(string key) {
			return () => {
				return Instantiate(_items[key].prefab);
			};
		}
		
		void OnGet(GameObject o) {
			o.transform.SetParent(null);
			o.SetActive(true);
			o.GetComponent<PoolItemBase>()?.OnGet();
		}
		
		void OnRelease(GameObject o) {
			o.SetActive(false);
			o.transform.SetParent(transform);
			o.GetComponent<PoolItemBase>()?.OnRelease();
		}
	}

}
