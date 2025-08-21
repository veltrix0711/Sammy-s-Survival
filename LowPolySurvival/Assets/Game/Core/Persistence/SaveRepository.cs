using System.IO;
using UnityEngine;

namespace LowPolySurvival.Game.Core.Persistence
{
	public sealed class SaveRepository : MonoBehaviour
	{
		[SerializeField] private string saveFileName = "save.json";

		public string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

		public void Write(string content)
		{
			var dir = Path.GetDirectoryName(SavePath);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			File.WriteAllText(SavePath, content);
		}

		public string Read()
		{
			return File.Exists(SavePath) ? File.ReadAllText(SavePath) : string.Empty;
		}
	}
}
