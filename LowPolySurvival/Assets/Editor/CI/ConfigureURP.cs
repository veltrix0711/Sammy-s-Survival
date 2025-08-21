using System;
using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public static class ConfigureURP
{
	public static void Run()
	{
		try
		{
			var type = Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
			if (type == null)
			{
				Debug.LogError("URP type not found; ensure package installed");
				EditorApplication.Exit(0);
				return;
			}
			var settingsDir = "Assets/Settings";
			if (!AssetDatabase.IsValidFolder(settingsDir))
			{
				AssetDatabase.CreateFolder("Assets", "Settings");
			}
			var asset = ScriptableObject.CreateInstance(type) as RenderPipelineAsset;
			if (asset == null)
			{
				Debug.LogError("Failed to create URP asset instance");
				EditorApplication.Exit(1);
				return;
			}
			var assetPath = Path.Combine(settingsDir, "URP-Pipeline.asset").Replace('\\','/');
			if (!File.Exists(assetPath))
			{
				AssetDatabase.CreateAsset(asset, assetPath);
			}
			GraphicsSettings.defaultRenderPipeline = asset;
			for (int i = 0; i < QualitySettings.names.Length; i++)
			{
				QualitySettings.SetQualityLevel(i, false);
				QualitySettings.renderPipeline = asset;
			}
			AssetDatabase.SaveAssets();
			Debug.Log("URP configured and assigned.");
			EditorApplication.Exit(0);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			EditorApplication.Exit(1);
		}
	}
}
