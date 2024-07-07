using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CustomRoom {
	public class RoomCapture : MonoBehaviour
	{
		public Transform roomsRoot;
		public RenderTexture rt;
		public string outputPath;
	
		[Button]
		public void Capture() {
			for (int i = 0; i < roomsRoot.childCount; i++) {
				Transform room = roomsRoot.GetChild(i);
				room.gameObject.SetActive(true);
				Utils.CaptureCamera(Camera.main, rt, System.IO.Path.Combine(outputPath, room.name + ".png"));
				room.GetChild(1).gameObject.SetActive(false);
				Utils.CaptureCamera(Camera.main, rt, System.IO.Path.Combine(outputPath, room.name + "_empty.png"));
				room.GetChild(1).gameObject.SetActive(true);
				room.gameObject.SetActive(false);
			}
		}
	}
}

