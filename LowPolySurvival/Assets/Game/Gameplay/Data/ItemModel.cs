using System;
using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	public enum ItemCategory
	{
		Generic,
		Weapon,
		Ammo,
		Tool,
		Food,
		Medical,
		Clothing,
		Bag,
		Material,
		Component,
		VehiclePart,
		BuildingPart
	}

	public enum BaseMaterial
	{
		Unknown,
		Metal,
		Wood,
		Cloth,
		Leather,
		Plastic,
		Rubber,
		Stone,
		Glass
	}

	[Flags]
	public enum ItemCapability
	{
		None = 0,
		HasMovingParts = 1 << 0,
		CanOverheat = 1 << 1,
		CanJam = 1 << 2,
		IsEdible = 1 << 3,
		IsDrinkable = 1 << 4,
		IsCuttingEdge = 1 << 5,
		IsBlunt = 1 << 6,
		IsContainer = 1 << 7,
		IsWearable = 1 << 8
	}

	[Serializable]
	public struct ConditionSet
	{
		[Range(0, 1)] public float dirt;      // 0 clean → 1 very dirty
		[Range(0, 1)] public float wetness;   // 0 dry → 1 soaked
		[Range(0, 1)] public float rust;      // for metal
		[Range(0, 1)] public float paintWear; // 0 pristine → 1 bare
		[Range(0, 1)] public float sharpness; // for knives: 0 blunt → 1 razor
		[Range(0, 1)] public float integrity; // general structural health 0 broken → 1 new
		public bool jammed;                    // for firearms/tools with moving parts
	}

	[Serializable]
	public class ItemInstance
	{
		[SerializeField] private string uniqueId;
		[SerializeField] private ItemDefinition definition;
		[SerializeField] private ConditionSet condition;
		[SerializeField] private int stackCount = 1;

		public string UniqueId => uniqueId;
		public ItemDefinition Definition => definition;
		public ConditionSet Condition { get => condition; set => condition = value; }
		public int StackCount { get => stackCount; set => stackCount = Mathf.Max(1, value); }

		public ItemInstance(ItemDefinition def, ConditionSet defaults, int count = 1)
		{
			uniqueId = System.Guid.NewGuid().ToString("N");
			definition = def;
			condition = defaults;
			stackCount = Mathf.Max(1, count);
		}
	}

	public enum CarryMode
	{
		Inventory,
		HandsOnly
	}
}


