using UnityEngine;
using LowPolySurvival.Game.Gameplay.UI;

namespace LowPolySurvival.Game.Gameplay.Systems.Interaction
{
	// Simple bench interaction to toggle crafting+inventory UI
	public sealed class BenchInteractable : InteractableBase
	{
		[SerializeField] private RecipeUI recipeUI;
		[SerializeField] private InventoryUI inventoryUI;

		private void Awake()
		{
			if (recipeUI == null) recipeUI = Object.FindFirstObjectByType<RecipeUI>();
			if (inventoryUI == null) inventoryUI = Object.FindFirstObjectByType<InventoryUI>();
		}

		public override bool CanInteract(InteractionVerb verb, GameObject interactor)
		{
			return verb == InteractionVerb.Use;
		}

		public override bool Interact(InteractionVerb verb, GameObject interactor)
		{
			if (verb != InteractionVerb.Use) return false;
			bool nextVisible = true;
			if (recipeUI != null && inventoryUI != null)
			{
				nextVisible = !(recipeUI.IsVisible || inventoryUI.IsVisible);
			}
			if (recipeUI != null) recipeUI.SetVisible(nextVisible);
			if (inventoryUI != null) inventoryUI.SetVisible(nextVisible);
			// ensure pause menu is not opened here; just UI panels
			return true;
		}
	}
}


