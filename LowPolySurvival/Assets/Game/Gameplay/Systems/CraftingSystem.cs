using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class CraftingSystem : MonoBehaviour
	{
		public bool CanCraft(RecipeDefinition recipe, System.Func<string, bool> hasKnowledge)
		{
			if (recipe == null) return false;
			var token = recipe.RequiredKnowledgeToken;
			if (!string.IsNullOrEmpty(token) && (hasKnowledge == null || !hasKnowledge(token)))
				return false;
			// TODO: check inventory quantities
			return true;
		}

		public bool Craft(RecipeDefinition recipe)
		{
			if (recipe == null) return false;
			// TODO: remove inputs and add outputs to inventory; apply time scaling by benches
			return true;
		}
	}
}
