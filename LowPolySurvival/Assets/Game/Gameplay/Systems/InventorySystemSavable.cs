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
		private class Entry { public string itemId; public int count; }
		[System.Serializable]
		private class SaveModel { public List<Entry> items = new List<Entry>(); }

		public string SaveToJson()
		{
			var model = new SaveModel();
			// TODO: replace with real iteration of InventorySystem storage
			return JsonUtility.ToJson(model);
		}

		public void LoadFromJson(string json)
		{
			var model = string.IsNullOrEmpty(json) ? new SaveModel() : JsonUtility.FromJson<SaveModel>(json);
			// TODO: clear and rebuild inventory from model using a definition registry
		}
	}
}
