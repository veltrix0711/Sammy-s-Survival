using UnityEngine;
using LowPolySurvival.Game.Gameplay.Player;
using LowPolySurvival.Game.Gameplay.Systems;
using LowPolySurvival.Game.Core.Persistence;
using LowPolySurvival.Game.Gameplay.UI;

namespace LowPolySurvival.Game.Bootstrap
{
	[DefaultExecutionOrder(-1000)]
	public sealed class AutoBootstrap : MonoBehaviour
	{
		[SerializeField] private bool ensureGround = true;
		[SerializeField] private bool ensurePlayer = true;
		[SerializeField] private bool ensureSaveSystems = true;
		[SerializeField] private bool ensurePauseMenu = true;

		private void Awake()
		{
			if (ensureGround)
			{
				var ground = GameObject.Find("Ground_Plane_10x10");
				if (ground == null)
				{
					var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
					plane.name = "Ground_Plane_10x10";
					plane.transform.position = Vector3.zero;
					var col = plane.GetComponent<MeshCollider>();
					if (col != null) col.convex = false;
				}
			}

			if (ensurePlayer)
			{
				var player = GameObject.Find("Player");
				if (player == null)
				{
					player = new GameObject("Player");
					var cc = player.AddComponent<CharacterController>();
					cc.height = Mathf.Max(1.8f, cc.height);
					cc.radius = Mathf.Max(0.35f, cc.radius);
					cc.center = new Vector3(0, 0.9f, 0);
					player.AddComponent<SimplePlayerController>();
					player.transform.position = new Vector3(0, 2.5f, 0);
				}
				if (player.GetComponent<InventorySystem>() == null)
				{
					player.AddComponent<InventorySystem>();
					player.AddComponent<InventorySystemSavable>();
				}
			}

			if (ensureSaveSystems)
			{
				var gsObj = GameObject.FindFirstObjectByType<GameState>();
				if (gsObj == null)
				{
					var go = new GameObject("GameState");
					go.AddComponent<GameState>();
				}
				if (GameObject.FindFirstObjectByType<SaveHotkeys>() == null)
				{
					var hk = new GameObject("SaveHotkeys");
					hk.AddComponent<SaveHotkeys>();
				}
			}

			if (ensurePauseMenu)
			{
				if (GameObject.FindFirstObjectByType<PauseMenu>() == null)
				{
					var pm = new GameObject("PauseMenu");
					pm.AddComponent<PauseMenu>();
				}
			}

			// Ensure interaction prompt HUD
			if (GameObject.FindFirstObjectByType<InteractionPrompt>() == null)
			{
				var hud = new GameObject("InteractionPrompt");
				hud.AddComponent<InteractionPrompt>();
			}
		}
	}
}


