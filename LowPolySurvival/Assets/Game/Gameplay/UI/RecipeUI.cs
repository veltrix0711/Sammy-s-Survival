using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;
using LowPolySurvival.Game.Gameplay.Systems;

namespace LowPolySurvival.Game.Gameplay.UI
{
	public sealed class RecipeUI : MonoBehaviour
	{
		[SerializeField] private CraftingSystem crafting;
		[SerializeField] private RecipeDefinition[] recipes;
		[SerializeField] private bool visible;
		public bool IsVisible => visible;

		private void OnGUI()
		{
			if (!visible) return;
			const int w = 280; int x = 16; int y = Screen.height - 160; int h = 24; int pad = 6;
			GUI.Box(new Rect(x - 8, y - 8, w + 16, 140), "Recipes");
			if (recipes == null || crafting == null) return;
			foreach (var r in recipes)
			{
				if (r == null) continue;
				if (GUI.Button(new Rect(x, y, w, h), $"Craft: {r.name}"))
				{
					crafting.Craft(r);
				}
				y += h + pad;
			}
		}

		public void SetVisible(bool isVisible)
		{
			visible = isVisible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = visible;
			if (visible) UIInputLock.Push(); else UIInputLock.Pop();
		}
	}
}
