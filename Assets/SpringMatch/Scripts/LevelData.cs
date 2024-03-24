using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

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
	
	public struct SpringData {
		public int x0;
		public int y0;
		public int x1;
		public int y1;
		public float height;
		public int type;
	}
	
	public struct SpringColorPattle {
		public int type;
		[JsonConverter(typeof(ColorConvert))]
		public Color color;
	}

	public class LevelData
	{
		public List<SpringData> springs;
	}
}
