using System.Collections.Generic;
using UnityEngine;

namespace LowPolySurvival.Game.Core.Persistence
{
	public sealed class GameState : MonoBehaviour, ISavable
	{
		[SerializeField] private SaveRepository repository;
		[SerializeField] private MonoBehaviour[] savableComponents; // assign components that implement ISavable

		[System.Serializable]
		private class SaveBundle
		{
			public Dictionary<string, string> subsystemJson = new Dictionary<string, string>();
			public Dictionary<string, string> identifiedJson = new Dictionary<string, string>();
		}

		public void Save()
		{
			var bundle = new SaveBundle();
			foreach (var mb in savableComponents)
			{
				if (mb is ISavable s)
				{
					bundle.subsystemJson[mb.GetType().FullName] = s.SaveToJson();
				}
			}
			// Discover identified savables in scene
			foreach (var idSav in FindObjectsOfType<MonoBehaviour>(true))
			{
				if (idSav is IIdentifiedSavable isv)
				{
					var key = isv.GetSaveKey();
					if (!string.IsNullOrEmpty(key)) bundle.identifiedJson[key] = isv.SaveToJson();
				}
			}
			var json = JsonUtility.ToJson(bundle);
			repository.Write(json);
		}

		public void Load()
		{
			var json = repository.Read();
			if (string.IsNullOrEmpty(json)) return;
			var bundle = JsonUtility.FromJson<SaveBundle>(json);
			foreach (var mb in savableComponents)
			{
				if (mb is ISavable s && bundle.subsystemJson.TryGetValue(mb.GetType().FullName, out var sub))
				{
					s.LoadFromJson(sub);
				}
			}
			// Restore identified savables
			foreach (var idSav in FindObjectsOfType<MonoBehaviour>(true))
			{
				if (idSav is IIdentifiedSavable isv)
				{
					var key = isv.GetSaveKey();
					if (!string.IsNullOrEmpty(key) && bundle.identifiedJson.TryGetValue(key, out var j))
						isv.LoadFromJson(j);
				}
			}
		}

		public string SaveToJson()
		{
			return repository.Read();
		}

		public void LoadFromJson(string json)
		{
			repository.Write(json);
		}
	}
}
