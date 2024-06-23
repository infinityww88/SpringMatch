using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using ScriptableObjectArchitecture;
using Newtonsoft.Json;

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
	
	[MenuItem("Tools/GenerateEditorColors")]
	public static void GenerateEditorColors() {
		var colors = AssetDatabase.LoadAssetAtPath<Collection<Color>>("Assets/SpringMatch/Config/EditorColors.asset");
		string json = JsonConvert.SerializeObject(colors.ToList(), new SpringMatch.ColorConvert());
		File.WriteAllText("Build/LevelEditor/color.json", json);
	}
	
	[MenuItem("Tools/CompileDll")]
	public static void CompileDll() {
		var startDt = System.DateTime.Now;
		EditorApplication.ExecuteMenuItem("HybridCLR/CompileDll/Android");
		Debug.Log($"Compile Android End {startDt} {System.DateTime.Now}");
		
		File.Copy($"./HybridCLRData/HotUpdateDlls/Android/HotUpdateAssembly.dll",
			"./Assets/SpringMatch/Assembly/HotUpdateAssembly.dll.bytes", true);
		Debug.Log("Copy HotUpdateAssembly.dll");
	}
	
	[MenuItem("Tools/Publish")]
	public static void Publish() {
		var dirs = Directory.GetDirectories(@"Bundles\Android\DefaultPackage");
		List<string> ret = new List<string>();
		foreach (var d in dirs) {
			var match = System.Text.RegularExpressions.Regex.Match(d, @".*\d{4}-\d{2}-\d{2}-\d+");
			if (match.Success) {
				ret.Add(d);
			}
		}
		if (ret.Count == 0) {
			Debug.LogError("There is no package folder");
			return;
		}
		ret.Sort((a, b) => {
			var at = new System.DateTimeOffset(Directory.GetCreationTime(a));
			var bt = new System.DateTimeOffset(Directory.GetCreationTime(b));
			return (int)(at.ToUnixTimeSeconds() - bt.ToUnixTimeSeconds());
		});
		var newestFolder = ret.Last();
		string lastFolder = ret.Count == 1 ? null : ret[ret.Count - 2];
		var updateFiles = new List<string>();
		foreach (var fn in Directory.GetFiles(newestFolder)) {
			if (lastFolder == null) {
				updateFiles.Add(fn);
			} else {
				var file = Path.Join(lastFolder, Path.GetFileName(fn));
				if (!File.Exists(file)) {
					updateFiles.Add(fn);
				} else {
					MD5 md5 = MD5.Create();
					using (FileStream fileStream0 = File.Open(fn, FileMode.Open, FileAccess.Read),
						fileStream1 = File.Open(file, FileMode.Open, FileAccess.Read)) {
						var digit0 = md5.ComputeHash(fileStream0);
						var digit1 = md5.ComputeHash(fileStream1);
						if (!digit0.SequenceEqual(digit1)) {
							updateFiles.Add(fn);
						}
					}
				}
			}
		}
		
		if (updateFiles.Count == 0) {
			Debug.Log("No file need to update.");
		} else {
			var outputPath = "Bundles/output";
			if (Directory.Exists(outputPath)) {
				Directory.Delete(outputPath, true);
			}
			Directory.CreateDirectory(outputPath);
		
			foreach (var fn in updateFiles) {
				File.Copy(fn, Path.Join(outputPath, Path.GetFileName(fn)));
				Debug.Log("Add file " + fn);
			}
			
			string cmd = $"aws s3 cp {Path.GetFullPath(outputPath)} s3://public-ce19f4f2-a8cf-4210-8209-82b441412ee0/SpringMatch/Resource/ --acl public-read --recursive";
			Utils.RunCmd(cmd);
			
			Debug.Log("Publish Complete.");
		}
	}
	
	[MenuItem("Tools/UpdateLevelColors")]
	public static void UpdateLevelColors() {
		var colors = AssetDatabase.LoadAssetAtPath<Collection<Color>>("Assets/SpringMatch/Config/EditorColors.asset");
		var files = Directory.GetFiles("C:/Users/13687/Documents/Unity/Mobile/SpringMatchMobile/Build/LevelEditor", "*.json");
		
		files.Foreach(fn => {
			var f = Path.GetFileName(fn);
			if (System.Text.RegularExpressions.Regex.IsMatch(f, @"\d+-\d+\.json")) {
				SpringMatch.LevelData levelData = JsonConvert.DeserializeObject<SpringMatch.LevelData>(File.ReadAllText(fn),
					new SpringMatch.ColorConvert());
				int n = Mathf.Min(levelData.colorNums.Count, colors.Count);
				for (int i = 0; i < n; i++) {
					levelData.colorNums[i].color = colors[i];
				}
				
				var text = JsonConvert.SerializeObject(levelData, new SpringMatch.ColorConvert());
				File.WriteAllText(fn, text);
				Debug.Log("update " + fn);
			}
		});
		Debug.Log("complete.");
	}
	
}
