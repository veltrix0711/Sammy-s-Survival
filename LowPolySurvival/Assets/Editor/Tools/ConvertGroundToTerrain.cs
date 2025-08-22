using UnityEditor;
using UnityEngine;
using System.IO;

namespace LowPolySurvival.Game.EditorTools
{
	public static class ConvertGroundToTerrain
	{
		[MenuItem("Tools/Convert Ground Tiles To Terrain")] 
		public static void Convert()
		{
			// Find parent named "Ground" or gather all GroundTile_* objects
			var groundParent = GameObject.Find("Ground");
			if (groundParent == null)
			{
				var guess = GameObject.FindObjectsOfType<MeshRenderer>();
				Bounds? b = null;
				foreach (var mr in guess)
				{
					if (mr.gameObject.name.StartsWith("GroundTile_"))
					{
						if (b == null) b = mr.bounds; else b = Encapsulate(b.Value, mr.bounds);
					}
				}
				if (b == null)
				{
					EditorUtility.DisplayDialog("No Ground Tiles Found", "Could not find objects named 'Ground' or 'GroundTile_*'.", "OK");
					return;
				}
				CreateTerrainFromBounds(b.Value);
				return;
			}

			// Compute bounds from children renderers
			var renderers = groundParent.GetComponentsInChildren<MeshRenderer>();
			if (renderers == null || renderers.Length == 0)
			{
				EditorUtility.DisplayDialog("No Mesh Renderers", "'Ground' has no mesh renderers to infer size from.", "OK");
				return;
			}
			Bounds total = renderers[0].bounds;
			for (int i = 1; i < renderers.Length; i++) total.Encapsulate(renderers[i].bounds);
			CreateTerrainFromBounds(total);

			// Optionally remove ground tiles
			if (EditorUtility.DisplayDialog("Remove Tiles?", "Remove existing ground tiles after creating the Terrain?", "Yes", "No"))
			{
				Undo.DestroyObjectImmediate(groundParent);
			}
		}

		private static Bounds Encapsulate(Bounds a, Bounds b)
		{
			a.Encapsulate(b);
			return a;
		}

		private static void CreateTerrainFromBounds(Bounds b)
		{
			// Prepare directories
			Directory.CreateDirectory("Assets/Content/Terrain");
			// Create TerrainData
			var td = new TerrainData();
			// Size: width (x), height (y), length (z)
			float width = Mathf.Max(10f, b.size.x);
			float length = Mathf.Max(10f, b.size.z);
			float height = 200f; // default height range
			td.size = new Vector3(width, height, length);
			// Reasonable heightmap resolution
			td.heightmapResolution = 513;
			td.alphamapResolution = 512;
			AssetDatabase.CreateAsset(td, "Assets/Content/Terrain/WorldTerrainData.asset");
			AssetDatabase.SaveAssets();

			// Create Terrain GameObject placed at bounds min.xz
			var go = Terrain.CreateTerrainGameObject(td);
			go.name = "Terrain";
			go.transform.position = new Vector3(b.min.x, 0f, b.min.z);
			// Create a simple TerrainLayer if none exists
			string layerPath = "Assets/Content/Terrain/DefaultTerrainLayer.terrainlayer";
			TerrainLayer layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
			if (layer == null)
			{
				layer = new TerrainLayer();
				// Create a small gray texture
				var tex = new Texture2D(2, 2);
				ex.SetPixels(new[] { new Color(0.5f,0.5f,0.5f,1f), new Color(0.5f,0.5f,0.5f,1f), new Color(0.5f,0.5f,0.5f,1f), new Color(0.5f,0.5f,0.5f,1f)});
				ex.Apply();
				string texPath = "Assets/Content/Terrain/DefaultTerrainAlbedo.png";
				File.WriteAllBytes(texPath, tex.EncodeToPNG());
				AssetDatabase.ImportAsset(texPath);
				var aTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
				layer.diffuseTexture = aTex;
				AssetDatabase.CreateAsset(layer, layerPath);
				AssetDatabase.SaveAssets();
			}
			var t = go.GetComponent<Terrain>();
			t.terrainData.terrainLayers = new TerrainLayer[] { layer };
			// Mark static for batching/occlusion
			GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.NavigationStatic);

			EditorUtility.DisplayDialog("Terrain Created", "Terrain created matching ground tile bounds.", "OK");
		}
	}
}


