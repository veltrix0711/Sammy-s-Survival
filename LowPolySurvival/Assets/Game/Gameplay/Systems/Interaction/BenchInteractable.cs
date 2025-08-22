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
			if (recipeUI != null) recipeUI.SetVisible(true);
			if (inventoryUI != null) inventoryUI.SetVisible(true);
			return true;
		}
	}
}


