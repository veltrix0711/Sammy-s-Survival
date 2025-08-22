using UnityEngine;
using System;

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
		[SerializeField] private ItemCategory category = ItemCategory.Generic;
		[SerializeField] private BaseMaterial material = BaseMaterial.Unknown;
		[SerializeField] private ItemCapability capabilities = ItemCapability.None;
		[SerializeField] private ConditionSet defaultCondition;
		[SerializeField] private bool stackable = true;
		[SerializeField] private int maxStack = 10;

		public string ItemId => itemId;
		public string DisplayName => displayName;
		public float WeightKg => weightKg;
		public float VolumeLiters => volumeLiters;
		public string[] Tags => tags;
		public ItemCategory Category => category;
		public BaseMaterial Material => material;
		public ItemCapability Capabilities => capabilities;
		public ConditionSet DefaultCondition => defaultCondition;
		public bool Stackable => stackable;
		public int MaxStack => Mathf.Max(1, maxStack);
	}
}
