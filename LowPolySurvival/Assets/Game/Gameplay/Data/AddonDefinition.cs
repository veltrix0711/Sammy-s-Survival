using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	[CreateAssetMenu(fileName = "AddonDefinition", menuName = "LowPolySurvival/Addon", order = 3)]
	public sealed class AddonDefinition : ScriptableObject
	{
		[SerializeField] private string addonId;
		[SerializeField] private string displayName;
		[SerializeField] private string[] tags;

		public string AddonId => addonId;
		public string DisplayName => displayName;
		public string[] Tags => tags;
	}
}
