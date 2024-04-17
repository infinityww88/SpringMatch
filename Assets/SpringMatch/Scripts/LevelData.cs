using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace SpringMatch {
	
	public class ColorConvert : JsonConverter<Color> {
		public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer) {
			writer.WriteValue("#" + ColorUtility.ToHtmlStringRGB(value));
		}
		public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer) {
			string value = (string)reader.Value;
			var ret = ColorUtility.TryParseHtmlString(value, out Color c);
			return c;
		}
	}
	
	public class CameraConfig {
		public Vector3 cameraPosition;
		public Quaternion cameraRotation;
		public Quaternion HorzRotation;
		public Quaternion VertRotation;
	}
	
	public class SpringData {
		public int x0;
		public int y0;
		public int x1;
		public int y1;
		public int heightStep;
		public bool hideWhenCovered;
		public int followNum;
		public bool IsHole => followNum > 0;
	}
	
	public class ColorNums {
		[JsonConverter(typeof(ColorConvert))]
		public Color color;
		public int num;
	}

	public class LevelData
	{
		public int row;
		public int col;
		public List<ColorNums> colorNums;
		public List<SpringData> springs = new List<SpringData>();
	}
}
