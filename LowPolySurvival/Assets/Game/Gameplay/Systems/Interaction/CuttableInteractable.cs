using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems.Interaction
{
	public sealed class CuttableInteractable : InteractableBase
	{
		[SerializeField] private ItemDefinition yieldItem;
		[SerializeField] private int yieldCount = 1;
		[SerializeField] private bool destroyOnCut = false;

		public override bool CanInteract(InteractionVerb verb, GameObject interactor)
		{
			return verb == InteractionVerb.Cut;
		}

		public override bool Interact(InteractionVerb verb, GameObject interactor)
		{
			if (verb != InteractionVerb.Cut) return false;
			var inv = interactor.GetComponent<LowPolySurvival.Game.Gameplay.Systems.InventorySystem>();
			if (inv != null && yieldItem != null)
			{
				inv.Add(yieldItem, Mathf.Max(1, yieldCount));
			}
			if (destroyOnCut) GameObject.Destroy(this.gameObject);
			return true;
		}
	}
}


