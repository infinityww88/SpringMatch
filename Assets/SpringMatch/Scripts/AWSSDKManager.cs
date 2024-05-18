using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;
using Sirenix.OdinInspector;

public static class AWSSDKManager
{
	private const string ACCESS_KEY = "";
	private const string SECRET_KEY = "";
	private const string BUCKET = "";
	
	[Button]
	public static async UniTask DownloadS3Object(string key, string filePath) {
		AmazonS3Config s3Config = new AmazonS3Config();
		s3Config.RegionEndpoint = Amazon.RegionEndpoint.USWest2;
		IAmazonS3 s3Client = new AmazonS3Client(ACCESS_KEY,
			SECRET_KEY,
			s3Config);
		
		var objResp = await s3Client.GetObjectAsync(BUCKET, key).AsUniTask();
		var srcStream = objResp.ResponseStream;
		var directory = Path.GetDirectoryName(filePath);
		Directory.CreateDirectory(directory);
		using var destStream = File.Create(filePath);
		await srcStream.CopyToAsync(destStream).AsUniTask();
	}
}
