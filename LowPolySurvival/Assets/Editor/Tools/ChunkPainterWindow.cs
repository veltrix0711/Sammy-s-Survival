using UnityEditor;
using UnityEngine;
using System.IO;

public class ChunkPainterWindow : EditorWindow
{
	private string worldRoot = "Assets/Scenes/World/Biomes/Temperate/Chunks";
	private int chunkX = 0, chunkY = 0;
	private int size = 16;
	private float tileScale = 1f;
	private Material assignMaterial;
	
	[MenuItem("Tools/Chunk Painter")] 
	public static void Open() => GetWindow<ChunkPainterWindow>("Chunk Painter");

	private void OnGUI()
	{
		worldRoot = EditorGUILayout.TextField("Chunks Root", worldRoot);
		EditorGUILayout.BeginHorizontal();
		chunkX = EditorGUILayout.IntField("Chunk X", chunkX);
		chunkY = EditorGUILayout.IntField("Chunk Y", chunkY);
		EditorGUILayout.EndHorizontal();
		size = EditorGUILayout.IntSlider("Size", size, 8, 64);
		tileScale = EditorGUILayout.Slider("Tile Scale", tileScale, 0.25f, 2f);
		assignMaterial = (Material)EditorGUILayout.ObjectField("Assign Material", assignMaterial, typeof(Material), false);
		if (GUILayout.Button("Open/Create Chunk Scene"))
		{
			OpenOrCreateChunk();
		}
		if (GUILayout.Button("Paint Debug Grid"))
		{
			PaintGrid();
		}
		if (GUILayout.Button("Create Static Voxel Chunk (Editor)"))
		{
			CreateStaticChunk();
		}
		EditorGUILayout.Space(8);
		GUILayout.Label("Fallback Ground", EditorStyles.boldLabel);
		if (GUILayout.Button("Create Simple Ground Plane (10x10m)"))
		{
			CreateSimpleGroundPlane();
		}
	}

	private string ChunkPath() => Path.Combine(worldRoot, $"Chunk_{chunkX}_{chunkY}.unity");

	private void OpenOrCreateChunk()
	{
		var path = ChunkPath();
		var dir = Path.GetDirectoryName(path);
		if (!AssetDatabase.IsValidFolder(dir))
		{
			EditorUtility.DisplayDialog("Missing Folder", "Create biomes/chunks folders first.", "OK");
			return;
		}
		if (!File.Exists(path))
		{
			var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, path);
		}
		UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Single);
	}

	private void PaintGrid()
	{
		var parent = GameObject.Find("DebugGrid");
		if (parent == null) parent = new GameObject("DebugGrid");
		foreach (Transform c in parent.transform) DestroyImmediate(c.gameObject);
		for (int x = 0; x < size; x++)
		{
			for (int z = 0; z < size; z++)
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
				go.transform.SetParent(parent.transform);
				go.transform.localScale = new Vector3(tileScale, tileScale, 1);
				go.transform.rotation = Quaternion.Euler(90, 0, 0);
				go.transform.position = new Vector3(x * tileScale, 0.01f, z * tileScale);
				go.name = $"Tile_{x}_{z}";
			}
		}
		EditorUtility.SetDirty(parent);
	}

	private void CreateStaticChunk()
	{
		var go = new GameObject("StaticChunk");
		var chunk = go.AddComponent<LowPolySurvival.Game.Core.Voxel.VoxelChunk>();
		chunk.Initialize(new Vector3Int(16, 8, 16), 1f);
		chunk.GenerateFlatFill(1);
		chunk.RebuildMesh();
		if (assignMaterial != null)
		{
			var mr = go.GetComponent<MeshRenderer>();
			mr.sharedMaterial = assignMaterial;
		}
		Selection.activeGameObject = go;
	}

	private void CreateSimpleGroundPlane()
	{
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.name = "Ground_Plane_10x10";
		plane.transform.position = Vector3.zero;
		var col = plane.GetComponent<MeshCollider>();
		if (col != null) { col.convex = false; }
		Selection.activeGameObject = plane;
	}
}
