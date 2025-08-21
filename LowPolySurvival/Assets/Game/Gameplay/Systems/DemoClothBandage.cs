using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class DemoClothBandage : MonoBehaviour
	{
		[SerializeField] private InventorySystem inventory;
		[SerializeField] private ItemDefinition cloth;
		[SerializeField] private ItemDefinition bandage;
		[SerializeField] private int clothPerBandage = 1;

		public bool TryMakeBandage()
		{
			if (inventory.GetCount(cloth) < clothPerBandage) return false;
			if (!inventory.Remove(cloth, clothPerBandage)) return false;
			inventory.Add(bandage, 1);
			return true;
		}
	}
}
