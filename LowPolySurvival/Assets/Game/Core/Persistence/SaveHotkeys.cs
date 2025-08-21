using UnityEngine;

namespace LowPolySurvival.Game.Core.Persistence
{
	[DefaultExecutionOrder(300)]
	public sealed class SaveHotkeys : MonoBehaviour
	{
		[SerializeField] private GameState gameState;
		[SerializeField] private KeyCode saveKey = KeyCode.F5;
		[SerializeField] private KeyCode loadKey = KeyCode.F9;

		private void Awake()
		{
			if (gameState == null)
			{
				gameState = UnityEngine.Object.FindFirstObjectByType<GameState>();
			}
		}

		private void Update()
		{
			if (gameState == null) return;
			if (Input.GetKeyDown(saveKey))
			{
				Debug.Log("SaveHotkeys: Saving game...");
				gameState.Save();
			}
			if (Input.GetKeyDown(loadKey))
			{
				Debug.Log("SaveHotkeys: Loading game...");
				gameState.Load();
			}
		}
	}
}


