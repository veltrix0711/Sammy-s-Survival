using System.Collections.Generic;
using UnityEngine;
using LowPolySurvival.Game.Core.Persistence;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class Container : MonoBehaviour, IIdentifiedSavable
	{
		[SerializeField] private string containerId; // designer-provided unique id per scene
		[SerializeField] private InventorySystem inventory;

		[System.Serializable]
		private class Entry { public string itemId; public int count; }
		[System.Serializable]
		private class SaveModel { public List<Entry> items = new List<Entry>(); }

		private void Awake()
		{
			if (inventory == null) inventory = gameObject.AddComponent<InventorySystem>();
		}

		public string GetSaveKey() => string.IsNullOrEmpty(containerId) ? gameObject.name : containerId;

		public string SaveToJson()
		{
			var model = new SaveModel();
			foreach (var kv in inventory.DebugEnumerate())
			{
				var def = kv.Key;
				if (def == null) continue;
				model.items.Add(new Entry { itemId = def.ItemId, count = kv.Value });
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
				if (def != null && e.count > 0)
					inventory.Add(def, e.count);
			}
		}
	}
}
