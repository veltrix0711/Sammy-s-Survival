using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using LowPolySurvival.Game.Gameplay.Data;
using LowPolySurvival.Game.Gameplay.Systems;
using LowPolySurvival.Game.Gameplay.Demo;
using LowPolySurvival.Game.Core.Persistence;
using LowPolySurvival.Game.Gameplay.Player;

public static class SetupDemoScene
{
	[MenuItem("CI/Setup Demo Scene")] 
	public static void Run()
	{
		// Ensure item assets
		var itemsDir = "Assets/Content/ScriptableObjects/Items";
		EnsureFolder(itemsDir);
		var cloth = CreateOrLoadItem(Path.Combine(itemsDir, "Cloth.asset"), so =>
		{
			so.FindProperty("itemId").stringValue = "cloth";
			so.FindProperty("displayName").stringValue = "Cloth";
			so.FindProperty("weightKg").floatValue = 0.2f;
			so.FindProperty("volumeLiters").floatValue = 0.5f;
		});
		var bandage = CreateOrLoadItem(Path.Combine(itemsDir, "Bandage.asset"), so =>
		{
			so.FindProperty("itemId").stringValue = "bandage";
			so.FindProperty("displayName").stringValue = "Bandage";
			so.FindProperty("weightKg").floatValue = 0.05f;
			so.FindProperty("volumeLiters").floatValue = 0.1f;
		});
		AssetDatabase.SaveAssets();

		// Ensure ItemRegistry in Resources
		EnsureFolder("Assets/Resources");
		var regPath = "Assets/Resources/ItemRegistry.asset";
		var registry = AssetDatabase.LoadAssetAtPath<ItemRegistry>(regPath);
		if (registry == null)
		{
			registry = ScriptableObject.CreateInstance<ItemRegistry>();
			AssetDatabase.CreateAsset(registry, regPath);
		}
		var soReg = new SerializedObject(registry);
		var itemsProp = soReg.FindProperty("items");
		itemsProp.arraySize = 2;
		itemsProp.GetArrayElementAtIndex(0).objectReferenceValue = cloth;
		itemsProp.GetArrayElementAtIndex(1).objectReferenceValue = bandage;
		soReg.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(registry);

		// Open Safehouse scene
		var scenePath = "Assets/Scenes/POI/Safehouse.unity";
		if (!File.Exists(scenePath))
		{
			EditorUtility.DisplayDialog("Setup Demo Scene", "Safehouse scene not found at " + scenePath, "OK");
			return;
		}
		var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

		// Ground plane for testing
		if (GameObject.Find("Ground") == null)
		{
			var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
			ground.name = "Ground";
			ground.transform.position = Vector3.zero;
		}

		// Create root and components
		var root = GameObject.Find("DemoRoot");
		if (root == null) root = new GameObject("DemoRoot");
		EditorUtility.SetDirty(root);

		var inventory = root.GetComponent<InventorySystem>();
		if (inventory == null) inventory = root.AddComponent<InventorySystem>();
		EditorUtility.SetDirty(inventory);

		var crafting = root.GetComponent<CraftingSystem>();
		if (crafting == null) crafting = root.AddComponent<CraftingSystem>();
		EditorUtility.SetDirty(crafting);

		var demo = root.GetComponent<DemoClothBandage>();
		if (demo == null) demo = root.AddComponent<DemoClothBandage>();
		var demoSO = new SerializedObject(demo);
		demoSO.FindProperty("inventory").objectReferenceValue = inventory;
		demoSO.FindProperty("cloth").objectReferenceValue = cloth;
		demoSO.FindProperty("bandage").objectReferenceValue = bandage;
		demoSO.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(demo);

		var repoGo = GameObject.Find("SaveRepo");
		if (repoGo == null) repoGo = new GameObject("SaveRepo");
		var repo = repoGo.GetComponent<SaveRepository>();
		if (repo == null) repo = repoGo.AddComponent<SaveRepository>();
		EditorUtility.SetDirty(repoGo);
		EditorUtility.SetDirty(repo);

		var invSavable = root.GetComponent<InventorySystemSavable>();
		if (invSavable == null) invSavable = root.AddComponent<InventorySystemSavable>();
		var invSavSO = new SerializedObject(invSavable);
		invSavSO.FindProperty("inventory").objectReferenceValue = inventory;
		invSavSO.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(invSavable);

		var gameStateGo = GameObject.Find("GameState");
		if (gameStateGo == null) gameStateGo = new GameObject("GameState");
		var gameState = gameStateGo.GetComponent<GameState>();
		if (gameState == null) gameState = gameStateGo.AddComponent<GameState>();
		var gsSO = new SerializedObject(gameState);
		gsSO.FindProperty("repository").objectReferenceValue = repo;
		var savablesProp = gsSO.FindProperty("savableComponents");
		savablesProp.arraySize = 1;
		savablesProp.GetArrayElementAtIndex(0).objectReferenceValue = invSavable;
		gsSO.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(gameStateGo);
		EditorUtility.SetDirty(gameState);

		// Player
		var player = GameObject.Find("Player");
		if (player == null)
		{
			player = new GameObject("Player");
			player.transform.position = new Vector3(0, 1.1f, -3f);
			player.AddComponent<CharacterController>();
			player.AddComponent<SimplePlayerController>();
		}
		var pi = player.GetComponent<PlayerInteraction>();
		if (pi == null) pi = player.AddComponent<PlayerInteraction>();
		var piSO = new SerializedObject(pi);
		piSO.FindProperty("inventory").objectReferenceValue = inventory;
		piSO.FindProperty("clothBandage").objectReferenceValue = demo;
		piSO.FindProperty("clothDef").objectReferenceValue = cloth;
		piSO.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(player);
		EditorUtility.SetDirty(pi);

		// HUD
		var hudGo = GameObject.Find("DemoHUD");
		if (hudGo == null) hudGo = new GameObject("DemoHUD");
		var hud = hudGo.GetComponent<DemoHud>();
		if (hud == null) hud = hudGo.AddComponent<DemoHud>();
		var hudSO = new SerializedObject(hud);
		hudSO.FindProperty("inventory").objectReferenceValue = inventory;
		hudSO.FindProperty("crafting").objectReferenceValue = crafting;
		hudSO.FindProperty("clothBandage").objectReferenceValue = demo;
		hudSO.FindProperty("gameState").objectReferenceValue = gameState;
		hudSO.FindProperty("cloth").objectReferenceValue = cloth;
		hudSO.FindProperty("bandage").objectReferenceValue = bandage;
		hudSO.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(hudGo);
		EditorUtility.SetDirty(hud);

		EditorSceneManager.MarkSceneDirty(scene);
		EditorSceneManager.SaveScene(scene);
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

	private static ItemDefinition CreateOrLoadItem(string assetPath, System.Action<SerializedObject> init)
	{
		var existing = AssetDatabase.LoadAssetAtPath<ItemDefinition>(assetPath);
		if (existing != null) return existing;
		var def = ScriptableObject.CreateInstance<ItemDefinition>();
		AssetDatabase.CreateAsset(def, assetPath);
		var so = new SerializedObject(def);
		init?.Invoke(so);
		so.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(def);
		return def;
	}
}
