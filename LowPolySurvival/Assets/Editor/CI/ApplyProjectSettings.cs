using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using System.IO;

public static class ApplyProjectSettings
{
	[MenuItem("CI/Apply Settings")]
	public static void Run()
	{
		// Project settings
		PlayerSettings.colorSpace = ColorSpace.Linear;
		EditorSettings.enterPlayModeOptionsEnabled = true;
		EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;

		// Ensure folders
		EnsureFolder("Assets/Scenes/World/Biomes/Temperate/Chunks");
		EnsureFolder("Assets/Scenes/POI");
		EnsureFolder("Assets/Settings");
		EnsureFolder("Assets/Game");
		EnsureFolder("Assets/Content/ScriptableObjects");
		EnsureFolder("Assets/Content/Prefabs");
		EnsureFolder("Assets/Content/Scenes");

		// Create placeholder scenes if missing
		CreateSceneIfMissing("Assets/Scenes/World/Master.unity");
		CreateSceneIfMissing("Assets/Scenes/World/Biomes/Temperate/Temperate_Biome.unity");
		CreateSceneIfMissing("Assets/Scenes/World/Biomes/Temperate/Chunks/Chunk_0_0.unity");
		CreateSceneIfMissing("Assets/Scenes/POI/Safehouse.unity");

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private static void EnsureFolder(string path)
	{
		var parts = path.Split('/');
		var current = parts[0];
		for (int i = 1; i < parts.Length; i++)
		{
			var next = current + "/" + parts[i];
			if (!AssetDatabase.IsValidFolder(next))
			{
				AssetDatabase.CreateFolder(current, parts[i]);
			}
			current = next;
		}
	}

	private static void CreateSceneIfMissing(string assetPath)
	{
		if (File.Exists(assetPath)) return;
		var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		EditorSceneManager.SaveScene(scene, assetPath);
	}
}
