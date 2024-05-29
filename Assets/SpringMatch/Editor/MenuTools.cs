using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class MenuTools
{
	[MenuItem("Tools/Generate Aes Key IV")]
	public static void GenerateAesKeyIV() {
		using (AesManaged myAes = new AesManaged()) {
			myAes.GenerateIV();
			myAes.GenerateKey();
			System.IO.File.WriteAllBytes("Assets/aes_key.bytes", myAes.Key);
			System.IO.File.WriteAllBytes("Assets/aes_iv.bytes", myAes.IV);
		}
	}
}
