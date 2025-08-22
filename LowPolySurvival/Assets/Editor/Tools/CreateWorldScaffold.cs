using UnityEditor;
using UnityEngine;
using System.IO;

namespace LowPolySurvival.Game.EditorTools
{
	public static class CreateWorldScaffold
	{
		[MenuItem("Tools/Create World Scaffold (Open World)")]
		public static void Create()
		{
			Directory.CreateDirectory("Assets/Scenes");
			Directory.CreateDirectory("Assets/Content/Materials");

			var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
				UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
				UnityEditor.SceneManagement.NewSceneMode.Single);

			// World root
			var root = new GameObject("WorldRoot");
			var env = new GameObject("Environment");
			env.transform.SetParent(root.transform);
			var groundParent = new GameObject("Ground");
			groundParent.transform.SetParent(env.transform);

			// Directional light
			if (Object.FindFirstObjectByType<Light>() == null)
			{
				var sun = new GameObject("Directional Light");
				var light = sun.AddComponent<Light>();
				light.type = LightType.Directional;
				light.intensity = 1f;
				sun.transform.rotation = Quaternion.Euler(50, -30, 0);
			}

			// Ground material (GPU instancing)
			string matPath = "Assets/Content/Materials/Ground_Instanced.mat";
			Material groundMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
			if (groundMat == null)
			{
				var shader = Shader.Find("Universal Render Pipeline/Lit");
				if (shader == null) shader = Shader.Find("Standard");
				groundMat = new Material(shader);
				groundMat.enableInstancing = true;
				AssetDatabase.CreateAsset(groundMat, matPath);
				AssetDatabase.SaveAssets();
			}

			// Build a starter grid of tiles (3x3 of 100m each => 300m square)
			int tiles = 3; float tileSizeMeters = 100f; // Plane is 10m at scale=1 => need localScale (10,1,10) for 100m
			float half = (tiles * tileSizeMeters) * 0.5f;
			for (int x = 0; x < tiles; x++)
			{
				for (int z = 0; z < tiles; z++)
				{
					var tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
					tile.name = $"GroundTile_{x}_{z}";
					tile.transform.SetParent(groundParent.transform);
					tile.transform.localScale = new Vector3(10, 1, 10); // ≈100m
					float px = -half + (x + 0.5f) * tileSizeMeters;
					float pz = -half + (z + 0.5f) * tileSizeMeters;
					tile.transform.position = new Vector3(px, 0, pz);
					var mr = tile.GetComponent<MeshRenderer>();
					if (mr != null) mr.sharedMaterial = groundMat;
					// Mark static and occlusion-friendly
					GameObjectUtility.SetStaticEditorFlags(tile, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic);
				}
			}

			// Add world grid gizmo helper
			var gizmo = new GameObject("WorldGridGizmo");
			gizmo.transform.SetParent(root.transform);
			gizmo.AddComponent<LowPolySurvival.Game.World.WorldGridGizmo>();

			var path = "Assets/Scenes/World_Main.unity";
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, path);
			EditorUtility.DisplayDialog("World Created", path, "OK");
		}
	}
}


