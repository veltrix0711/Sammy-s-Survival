using UnityEngine;
using System.Linq;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class CraftingSystem : MonoBehaviour
	{
		[SerializeField] private InventorySystem inventory;
		[SerializeField] private float benchSearchRadius = 3f;

		public bool CanCraft(RecipeDefinition recipe, System.Func<string, bool> hasKnowledge)
		{
			if (recipe == null) return false;
			var token = recipe.RequiredKnowledgeToken;
			if (!string.IsNullOrEmpty(token) && (hasKnowledge == null || !hasKnowledge(token)))
				return false;
			if (inventory == null) return false;
			// Check inventory quantities
			foreach (var s in recipe.Inputs)
			{
				if (s.item == null || s.quantity <= 0) return false;
				if (inventory.GetCount(s.item) < s.quantity) return false;
			}
			return true;
		}

		public bool Craft(RecipeDefinition recipe, Bench bench = null)
		{
			if (recipe == null) return false;
			if (inventory == null) return false;
			// Find bench nearby if not provided
			if (bench == null && benchSearchRadius > 0f)
			{
				var benches = UnityEngine.Object.FindObjectsByType<Bench>(FindObjectsSortMode.None);
				bench = benches
					.Where(b => (b.transform.position - transform.position).sqrMagnitude <= benchSearchRadius * benchSearchRadius)
					.OrderBy(b => (b.transform.position - transform.position).sqrMagnitude)
					.FirstOrDefault();
			}
			var speed = bench != null ? bench.SpeedMultiplier : 1f;
			var time = recipe.BaseSeconds / Mathf.Max(0.1f, speed);
			// Consume inputs
			foreach (var s in recipe.Inputs)
			{
				if (!inventory.Remove(s.item, s.quantity))
				{
					Debug.LogWarning("Craft aborted: missing inputs");
					return false;
				}
			}
			// Produce outputs
			foreach (var s in recipe.Outputs)
			{
				if (s.item != null && s.quantity > 0) inventory.Add(s.item, s.quantity);
			}
			Debug.Log($"Crafted {string.Join(",", System.Array.ConvertAll(recipe.Outputs, o => o.item != null ? o.item.DisplayName : "?"))} in ~{time:0.0}s {(bench!=null?"@bench":"@anywhere")}");
			return true;
		}
	}
}
