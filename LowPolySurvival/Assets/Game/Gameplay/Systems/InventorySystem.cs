using System.Collections.Generic;
using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class InventorySystem : MonoBehaviour
	{
		private readonly Dictionary<ItemDefinition, int> itemToCount = new Dictionary<ItemDefinition, int>();

		public int GetCount(ItemDefinition item)
		{
			if (item == null) return 0;
			return itemToCount.TryGetValue(item, out var c) ? c : 0;
		}

		public void Add(ItemDefinition item, int quantity)
		{
			if (item == null || quantity <= 0) return;
			itemToCount[item] = GetCount(item) + quantity;
		}

		public bool Remove(ItemDefinition item, int quantity)
		{
			if (item == null || quantity <= 0) return false;
			var have = GetCount(item);
			if (have < quantity) return false;
			var left = have - quantity;
			if (left == 0) itemToCount.Remove(item); else itemToCount[item] = left;
			return true;
		}

		// Debug/Save helpers
		public IEnumerable<KeyValuePair<ItemDefinition, int>> DebugEnumerate()
		{
			return itemToCount;
		}

		public void DebugClear()
		{
			itemToCount.Clear();
		}
	}
}
