using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;
using LowPolySurvival.Game.Gameplay.Systems;
using LowPolySurvival.Game.Core.Persistence;

namespace LowPolySurvival.Game.Gameplay.Demo
{
	public sealed class DemoHud : MonoBehaviour
	{
		[SerializeField] private InventorySystem inventory;
		[SerializeField] private CraftingSystem crafting;
		[SerializeField] private DemoClothBandage clothBandage;
		[SerializeField] private GameState gameState;
		[SerializeField] private ItemDefinition cloth;
		[SerializeField] private ItemDefinition bandage;

		private void OnGUI()
		{
			const int w = 240;
			int x = 16, y = 16, h = 28, pad = 6;
			GUI.Box(new Rect(x - 8, y - 8, w + 16, 200), "Demo");
			GUI.Label(new Rect(x, y, w, h), $"Cloth: {inventory.GetCount(cloth)}"); y += h + pad;
			GUI.Label(new Rect(x, y, w, h), $"Bandage: {inventory.GetCount(bandage)}"); y += h + pad;
			if (GUI.Button(new Rect(x, y, w, h), "+1 Cloth")) { inventory.Add(cloth, 1); } y += h + pad;
			if (GUI.Button(new Rect(x, y, w, h), "Craft Bandage")) { clothBandage.TryMakeBandage(); } y += h + pad;
			if (GUI.Button(new Rect(x, y, w, h), "Save")) { gameState.Save(); } y += h + pad;
			if (GUI.Button(new Rect(x, y, w, h), "Load")) { gameState.Load(); } y += h + pad;
		}
	}
}
