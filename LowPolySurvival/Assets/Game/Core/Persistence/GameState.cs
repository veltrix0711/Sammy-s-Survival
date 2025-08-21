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
			[System.Serializable]
			public class Pair { public string key; public string value; }
			public System.Collections.Generic.List<Pair> subsystem = new System.Collections.Generic.List<Pair>();
			public System.Collections.Generic.List<Pair> identified = new System.Collections.Generic.List<Pair>();
		}

		public void Save()
		{
			try
			{
				var bundle = new SaveBundle();
				foreach (var mb in savableComponents)
				{
					if (mb is ISavable s)
					{
						bundle.subsystem.Add(new SaveBundle.Pair { key = mb.GetType().FullName, value = s.SaveToJson() });
					}
				}
				// Discover identified savables in scene
				foreach (var idSav in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
				{
					if (idSav is IIdentifiedSavable isv)
					{
						var key = isv.GetSaveKey();
						if (!string.IsNullOrEmpty(key)) bundle.identified.Add(new SaveBundle.Pair { key = key, value = isv.SaveToJson() });
					}
				}
				var json = JsonUtility.ToJson(bundle);
				repository.Write(json);
				Debug.Log("GameState: Save completed.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"GameState: Save failed: {ex.Message}\n{ex.StackTrace}");
			}
		}

		public void Load()
		{
			try
			{
				var json = repository.Read();
				if (string.IsNullOrEmpty(json)) return;
				var bundle = JsonUtility.FromJson<SaveBundle>(json);
				var subsystemDict = new System.Collections.Generic.Dictionary<string, string>();
				var identifiedDict = new System.Collections.Generic.Dictionary<string, string>();
				if (bundle?.subsystem != null)
					foreach (var p in bundle.subsystem) if (p != null && !string.IsNullOrEmpty(p.key)) subsystemDict[p.key] = p.value;
				if (bundle?.identified != null)
					foreach (var p in bundle.identified) if (p != null && !string.IsNullOrEmpty(p.key)) identifiedDict[p.key] = p.value;
				foreach (var mb in savableComponents)
				{
					if (mb is ISavable s && subsystemDict.TryGetValue(mb.GetType().FullName, out var sub))
					{
						s.LoadFromJson(sub);
					}
				}
				// Restore identified savables
				foreach (var idSav in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
				{
					if (idSav is IIdentifiedSavable isv)
					{
						var key = isv.GetSaveKey();
						if (!string.IsNullOrEmpty(key) && identifiedDict.TryGetValue(key, out var j))
							isv.LoadFromJson(j);
					}
				}
				Debug.Log("GameState: Load completed.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"GameState: Load failed: {ex.Message}\n{ex.StackTrace}");
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

		private void Awake()
		{
			// Ensure repository exists
			if (repository == null)
			{
				repository = UnityEngine.Object.FindFirstObjectByType<SaveRepository>();
				if (repository == null)
				{
					repository = gameObject.AddComponent<SaveRepository>();
				}
			}
			// Auto-collect savables if array is empty
			if (savableComponents == null || savableComponents.Length == 0)
			{
				var all = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
				var list = new System.Collections.Generic.List<MonoBehaviour>();
				foreach (var mb in all)
				{
					if (mb == null || mb == (MonoBehaviour)this) continue;
					if (mb is IIdentifiedSavable) continue; // handled separately
					if (mb is ISavable) list.Add(mb);
				}
				savableComponents = list.ToArray();
				Debug.Log($"GameState: Auto-collected {savableComponents.Length} savable components.");
			}
		}
	}
}
