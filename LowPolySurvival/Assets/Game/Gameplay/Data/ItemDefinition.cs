using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	[CreateAssetMenu(fileName = "ItemDefinition", menuName = "LowPolySurvival/Item Definition", order = 0)]
	public sealed class ItemDefinition : ScriptableObject
	{
		[SerializeField] private string itemId;
		[SerializeField] private string displayName;
		[SerializeField] private float weightKg = 0.1f;
		[SerializeField] private float volumeLiters = 0.1f;
		[SerializeField] private string[] tags;

		public string ItemId => itemId;
		public string DisplayName => displayName;
		public float WeightKg => weightKg;
		public float VolumeLiters => volumeLiters;
		public string[] Tags => tags;
	}
}
