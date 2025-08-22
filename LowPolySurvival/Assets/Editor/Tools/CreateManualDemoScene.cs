using UnityEditor;
using UnityEngine;
using System.IO;

using LowPolySurvival.Game.Gameplay.Player;
using LowPolySurvival.Game.Gameplay.Systems;
using LowPolySurvival.Game.Gameplay.UI;
using LowPolySurvival.Game.Core.Persistence;

namespace LowPolySurvival.Game.EditorTools
{
	public static class CreateManualDemoScene
	{
		[MenuItem("Tools/Create Manual Demo Scene (No Bootstrap)")]
		public static void Create()
		{
			var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
				UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
				UnityEditor.SceneManagement.NewSceneMode.Single);

			// Ensure folder
			Directory.CreateDirectory("Assets/Scenes");

			// Ground
			var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
			ground.name = "Ground_Plane_10x10";
			ground.transform.position = Vector3.zero;
			var gcol = ground.GetComponent<MeshCollider>(); if (gcol != null) gcol.convex = false;

			// Player root
			var player = new GameObject("Player");
			player.transform.position = new Vector3(0, 2.5f, 0);
			var cc = player.AddComponent<CharacterController>();
			cc.height = 1.8f; cc.radius = 0.35f; cc.center = new Vector3(0, 0.9f, 0);
			player.AddComponent<SimplePlayerController>();
			var inv = player.AddComponent<InventorySystem>();
			player.AddComponent<InventorySystemSavable>();
			player.AddComponent<PlayerInteraction>();
			player.AddComponent<CraftingSystem>();

			// Save system
			var saveRepo = new GameObject("SaveRepo");
			saveRepo.AddComponent<SaveRepository>();
			var gs = new GameObject("GameState");
			gs.AddComponent<GameState>();
			var hk = new GameObject("SaveHotkeys");
			hk.AddComponent<SaveHotkeys>();

			// UI
			new GameObject("PauseMenu").AddComponent<PauseMenu>();
			new GameObject("InteractionPrompt").AddComponent<InteractionPrompt>();
			new GameObject("InventoryUI").AddComponent<InventoryUI>();
			var recipeUi = new GameObject("RecipeUI").AddComponent<RecipeUI>();
			recipeUi.GetType().GetField("crafting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(recipeUi, player.GetComponent<CraftingSystem>());

			// Bench cube
			var bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
			bench.name = "Bench";
			bench.transform.position = new Vector3(3, 0.5f, 3);
			if (bench.GetComponent<BoxCollider>() == null) bench.AddComponent<BoxCollider>();
			bench.AddComponent<Bench>();

			// Save scene
			var path = "Assets/Scenes/Demo_NoBootstrap.unity";
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, path);
			EditorUtility.DisplayDialog("Scene Created", path, "OK");
		}
	}
}


