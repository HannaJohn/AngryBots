using System;  
using System.Collections.Generic;  
using System.IO;  
using UnityEditor;  
using UnityEngine;  
using Object = UnityEngine.Object;  

public class PerformBuild  
{   
	[MenuItem("Automated/Automated Android Build")]  
	static void CommandLineBuildOnCheckinAndroid()  
	{  
		const BuildTarget target = BuildTarget.Android;  
		
		string[] levels = GetBuildScenes();  
		const string locationPathName = "AngryBotsAndroid.apk";  
		const BuildOptions options = BuildOptions.None;  
		
		DeleteStreamingAssets();  
		BuildPipelineBuildAssetBundle(target);  
		BuildPipelineBuildPlayer(levels, locationPathName, target, options);  
	}  
	
	private static string[] GetBuildScenes()  
	{  
		List<string> names = new List<string>();  
		foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)  
		{  
			if (e == null) { continue; }  
			if (e.enabled) { names.Add(e.path); }  
		}  
		return names.ToArray();  
	}  
	
	private static void DeleteStreamingAssets()  
	{  
		// Delete streaming assets (downloaded from source control).  
		string[] filesToDelete = Directory.GetFiles(Application.streamingAssetsPath, "*.unity3d*");  
		foreach (string file in filesToDelete)   
		{  
			File.Delete(file);  
		}  
	}  
	
	private static void BuildPipelineBuildAssetBundle(BuildTarget buildTarget)  
	{  
		string[] assetPaths = AssetDatabase.GetAllAssetPaths();  
		
		string pathName = Application.streamingAssetsPath;  
		foreach (string f in assetPaths)  
		{  
			if (!f.Contains("Master Assets")) { continue; }  
			Object a = Resources.LoadAssetAtPath(f, typeof(Object));  
			if (a == null) { continue; }  
			
			Object[] asset = new Object[1];  
			asset[0] = a;  
			string assetType = a.GetType().Name;  
			if (assetType.Equals("Object")) { continue; }  
			
			string assetName = assetType + "_" + asset[0].name + ".unity3d";  
			string fullName = pathName + "/" + assetName;  
			
			const BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle;  
			
			BuildPipeline.BuildAssetBundle(a, asset, fullName, options, buildTarget);  
		}  
	}  
	
	private static void BuildPipelineBuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)  
	{  
		PlayerSettings.productName = "Angry Bots";  
		PlayerSettings.bundleIdentifier = "com.studiosstevepro.angrybots";  
		PlayerSettings.bundleVersion = "1.0";  
		
		String error = BuildPipeline.BuildPlayer(levels, locationPathName, target, options);  
		if (!String.IsNullOrEmpty(error))  
		{  
			throw new System.Exception("Build failed: " + error);  
		}    
	}  
}  