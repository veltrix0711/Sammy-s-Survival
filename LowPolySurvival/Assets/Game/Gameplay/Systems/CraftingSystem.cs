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

		public bool Craft(RecipeDefinition recipe, Bench bench = null)
		{
			if (recipe == null) return false;
			// TODO: remove inputs and add outputs to inventory
			var speed = bench != null ? bench.SpeedMultiplier : 1f;
			var time = recipe.BaseSeconds / Mathf.Max(0.1f, speed);
			// For now just log
			Debug.Log($"Crafted {string.Join(",", System.Array.ConvertAll(recipe.Outputs, o => o.item != null ? o.item.DisplayName : "?"))} in ~{time:0.0}s");
			return true;
		}
	}
}
