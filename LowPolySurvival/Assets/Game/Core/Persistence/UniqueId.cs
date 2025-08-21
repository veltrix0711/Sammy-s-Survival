using UnityEngine;

namespace LowPolySurvival.Game.Core.Persistence
{
	[ExecuteAlways]
	public sealed class UniqueId : MonoBehaviour
	{
		[SerializeField] private string id;
		public string Id => id;

		private void OnValidate()
		{
			EnsureId();
		}

		private void Awake()
		{
			EnsureId();
		}

		private void EnsureId()
		{
			if (string.IsNullOrEmpty(id))
			{
				id = System.Guid.NewGuid().ToString("N");
			}
		}
	}
}
