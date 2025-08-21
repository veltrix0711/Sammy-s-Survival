using UnityEngine;
using LowPolySurvival.Game.Core.Persistence;

namespace LowPolySurvival.Game.Gameplay.UI
{
	[DefaultExecutionOrder(400)]
	public sealed class PauseMenu : MonoBehaviour
	{
		[SerializeField] private GameState gameState;
		[SerializeField] private KeyCode toggleKey = KeyCode.Escape;
		[SerializeField] private bool isOpen;
		[SerializeField] private bool pauseTime = true;

		private void Awake()
		{
			if (gameState == null)
			{
				gameState = Object.FindFirstObjectByType<GameState>();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(toggleKey))
			{
				isOpen = !isOpen;
				Time.timeScale = (isOpen && pauseTime) ? 0f : 1f;
				Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
				Cursor.visible = isOpen;
			}
		}

		private void OnGUI()
		{
			if (!isOpen) return;
			const int w = 300;
			const int h = 200;
			int x = (Screen.width - w) / 2;
			int y = (Screen.height - h) / 2;
			GUI.Box(new Rect(x, y, w, h), "Pause");
			y += 40;
			if (GUI.Button(new Rect(x + 20, y, w - 40, 36), "Save (F5)")) { gameState?.Save(); }
			y += 44;
			if (GUI.Button(new Rect(x + 20, y, w - 40, 36), "Load (F9)")) { gameState?.Load(); }
			y += 44;
			if (GUI.Button(new Rect(x + 20, y, w - 40, 36), "Resume (Esc)")) { isOpen = false; Time.timeScale = 1f; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
		}
	}
}


