using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Linq;
using System.IO;

public class MenuTools
{
	[MenuItem("Tools/Generate Aes Key Iv")]
	public static void GenerateAesKeyIV() {
		using (AesManaged myAes = new AesManaged()) {
			myAes.GenerateIV();
			myAes.GenerateKey();
			System.IO.File.WriteAllBytes("Assets/aes_key.bytes", myAes.Key);
			System.IO.File.WriteAllBytes("Assets/aes_iv.bytes", myAes.IV);
		}
	}
	
	[MenuItem("Tools/EncryptFiles")]
	public static void EncryptFiels() {
		var path = Path.Combine(Application.persistentDataPath, "levels");
		string[] files = System.IO.Directory.GetFiles(path);
		Debug.Log($"{files.Length}");
		byte[] aseKey = File.ReadAllBytes("Assets/SpringMatch/Config/aes_key.bytes");
		byte[] aseIV = File.ReadAllBytes("Assets/SpringMatch/Config/aes_iv.bytes");

		//if (Directory.Exists(@"C:\Users\13687\wsl\ss\hello")) {
		//	Directory.Delete(@"C:\Users\13687\wsl\ss\hello", true);
		//}
		foreach (var fn in files) { 
			var text = File.ReadAllText(fn);
			var data = Utils.Encrypt(text, aseKey, aseIV);
			File.WriteAllBytes(fn, data);
		}
	}
}
