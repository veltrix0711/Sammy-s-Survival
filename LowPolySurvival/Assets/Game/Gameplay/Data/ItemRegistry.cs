using System.Collections.Generic;
using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	[CreateAssetMenu(fileName = "ItemRegistry", menuName = "LowPolySurvival/Item Registry", order = 2)]
	public sealed class ItemRegistry : ScriptableObject
	{
		[SerializeField] private List<ItemDefinition> items = new List<ItemDefinition>();
		private Dictionary<string, ItemDefinition> idToItem;

		private void OnEnable()
		{
			Rebuild();
		}

		public void Rebuild()
		{
			idToItem = new Dictionary<string, ItemDefinition>(items.Count);
			foreach (var def in items)
			{
				if (def == null || string.IsNullOrWhiteSpace(def.ItemId)) continue;
				idToItem[def.ItemId] = def;
			}
		}

		public ItemDefinition GetById(string itemId)
		{
			if (string.IsNullOrEmpty(itemId)) return null;
			return idToItem != null && idToItem.TryGetValue(itemId, out var def) ? def : null;
		}

		public IReadOnlyList<ItemDefinition> Items => items;

		// Simple runtime loader
		private static ItemRegistry cached;
		public static ItemRegistry Load()
		{
			if (cached != null) return cached;
			cached = Resources.Load<ItemRegistry>("ItemRegistry");
			if (cached != null) cached.Rebuild();
			return cached;
		}
	}
}
