using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.UI
{
	public sealed class ContainerUI : MonoBehaviour
	{
		[SerializeField] private Systems.InventorySystem playerInventory;
		[SerializeField] private Systems.Container container;
		[SerializeField] private ItemDefinition[] quickItems;

		private void OnGUI()
		{
			const int w = 260; int x = Screen.width - w - 16; int y = 16; int h = 24; int pad = 6;
			GUI.Box(new Rect(x - 8, y - 8, w + 16, 200), "Container");
			if (quickItems != null)
			{
				foreach (var def in quickItems)
				{
					if (def == null) continue;
					GUI.Label(new Rect(x, y, w, h), $"{def.DisplayName}");
					if (GUI.Button(new Rect(x + 120, y, 50, h), "Give"))
					{
						if (playerInventory.Remove(def, 1)) container.GetComponent<Systems.InventorySystem>().Add(def, 1);
					}
					if (GUI.Button(new Rect(x + 175, y, 50, h), "Take"))
					{
						if (container.GetComponent<Systems.InventorySystem>().Remove(def, 1)) playerInventory.Add(def, 1);
					}
					y += h + pad;
				}
			}
		}
	}
}
