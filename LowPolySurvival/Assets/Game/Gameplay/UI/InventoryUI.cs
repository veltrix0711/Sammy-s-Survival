using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.UI
{
	[DefaultExecutionOrder(360)]
	public sealed class InventoryUI : MonoBehaviour
	{
		[SerializeField] private Systems.InventorySystem inventory;
		[SerializeField] private KeyCode toggleKey = KeyCode.I;
		[SerializeField] private bool visible;
		[SerializeField] private ItemDefinition[] watchItems; // optional quick display

		private void Awake()
		{
			if (inventory == null)
			{
				var pi = Object.FindFirstObjectByType<LowPolySurvival.Game.Gameplay.Systems.PlayerInteraction>();
				if (pi != null) inventory = pi.GetComponent<LowPolySurvival.Game.Gameplay.Systems.InventorySystem>();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(toggleKey)) visible = !visible;
		}

		private void OnGUI()
		{
			if (!visible || inventory == null) return;
			const int w = 240; int x = Screen.width - w - 16; int y = Screen.height - 200; int h = 22; int pad = 4;
			GUI.Box(new Rect(x - 8, y - 8, w + 16, 160), "Inventory");
			if (watchItems != null)
			{
				foreach (var it in watchItems)
				{
					if (it == null) continue;
					GUI.Label(new Rect(x, y, w, h), $"{it.DisplayName}: {inventory.GetCount(it)}");
					y += h + pad;
				}
			}
		}

		public void SetVisible(bool isVisible)
		{
			visible = isVisible;
		}
	}
}


