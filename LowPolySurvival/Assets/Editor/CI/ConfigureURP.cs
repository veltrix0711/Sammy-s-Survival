using System;
using System.IO;
using UnityEditor;
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
				UnityEngine.Debug.LogError("URP type not found; ensure package installed");
				UnityEditor.EditorApplication.Exit(2);
				return;
			}
			var settingsDir = "Assets/Settings";
			if (!AssetDatabase.IsValidFolder(settingsDir))
			{
				AssetDatabase.CreateFolder("Assets", "Settings");
			}
			var asset = ScriptableObject.CreateInstance(type) as UnityEngine.Rendering.RenderPipelineAsset;
			if (asset == null)
			{
				UnityEngine.Debug.LogError("Failed to create URP asset instance");
				UnityEditor.EditorApplication.Exit(3);
				return;
			}
			var assetPath = Path.Combine(settingsDir, "URP-Pipeline.asset").Replace('\\','/');
			if (!File.Exists(assetPath))
			{
				AssetDatabase.CreateAsset(asset, assetPath);
			}
			UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline = asset;
			UnityEngine.QualitySettings.renderPipeline = asset;
			AssetDatabase.SaveAssets();
			UnityEngine.Debug.Log("URP configured and assigned.");
			UnityEditor.EditorApplication.Exit(0);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError(ex);
			UnityEditor.EditorApplication.Exit(4);
		}
	}
}
