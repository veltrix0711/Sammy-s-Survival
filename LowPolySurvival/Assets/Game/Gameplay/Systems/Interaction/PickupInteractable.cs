using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems.Interaction
{
	public sealed class PickupInteractable : InteractableBase
	{
		[SerializeField] private ItemDefinition item;
		[SerializeField] private int quantity = 1;
		[SerializeField] private bool useCustomCondition = false;
		[SerializeField] private LowPolySurvival.Game.Gameplay.Data.ConditionSet customCondition;

		public override bool CanInteract(InteractionVerb verb, GameObject interactor)
		{
			return verb == InteractionVerb.Take || verb == InteractionVerb.Use;
		}

		public override bool Interact(InteractionVerb verb, GameObject interactor)
		{
			var inv = interactor.GetComponent<LowPolySurvival.Game.Gameplay.Systems.InventorySystem>();
			if (inv == null || item == null) return false;
			int qty = Mathf.Max(1, quantity);
			if (useCustomCondition)
			{
				inv.AddWithCondition(item, qty, customCondition);
			}
			else
			{
				inv.Add(item, qty);
			}
			GameObject.Destroy(this.gameObject);
			return true;
		}
	}
}


