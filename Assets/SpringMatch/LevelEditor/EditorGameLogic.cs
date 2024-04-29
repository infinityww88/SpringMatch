using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpringMatch;

namespace SpringMatchEditor {
	
	public class EditorGameLogic : MonoBehaviour
	{
		private Level currLevel = null;
	
		protected void Awake()
		{
			GameLogic.Pending = false;
			Screen.SetResolution(450, 900, false);
		}
	
		// Start is called before the first frame update
		void Start()
		{
			Level.Inst.Load(SpringMatchEditor.LevelEditor.CurrEditLevel);
			//Level.Inst.Load("level_1.json");
			currLevel = Level.Inst;
		}
		
		public void ToEditor() {
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
	}

}

