using System.Collections.Generic;
using UnityEngine;
using LowPolySurvival.Game.Core.Persistence;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class InventorySystemSavable : MonoBehaviour, ISavable
	{
		[SerializeField] private InventorySystem inventory;

		[System.Serializable]
		private class Entry { public string itemId; public int count; public ConditionSet condition; }
		[System.Serializable]
		private class SaveModel { public List<Entry> items = new List<Entry>(); }

		public string SaveToJson()
		{
			var model = new SaveModel();
			foreach (var inst in inventory.DebugEnumerateInstances())
			{
				if (inst == null || inst.Definition == null) continue;
				model.items.Add(new Entry { itemId = inst.Definition.ItemId, count = inst.StackCount, condition = inst.Condition });
			}
			return JsonUtility.ToJson(model);
		}

		public void LoadFromJson(string json)
		{
			var model = string.IsNullOrEmpty(json) ? new SaveModel() : JsonUtility.FromJson<SaveModel>(json);
			inventory.DebugClear();
			var registry = ItemRegistry.Load();
			if (registry == null) return;
			foreach (var e in model.items)
			{
				var def = registry.GetById(e.itemId);
				if (def == null || e.count <= 0) continue;
				// Recreate counts using inventory Add. Condition granularity is not yet per-instance attachable through public API.
				inventory.Add(def, e.count);
			}
		}
	}
}
