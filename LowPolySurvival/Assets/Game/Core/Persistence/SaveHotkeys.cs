using UnityEngine;

namespace LowPolySurvival.Game.Core.Persistence
{
	[DefaultExecutionOrder(300)]
	public sealed class SaveHotkeys : MonoBehaviour
	{
		[SerializeField] private GameState gameState;
		[SerializeField] private KeyCode saveKey = KeyCode.F5;
		[SerializeField] private KeyCode loadKey = KeyCode.F9;
		[SerializeField] private float debounceSeconds = 0.2f;
		private float nextAllowedTime;

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
			if (Time.unscaledTime < nextAllowedTime) return;
			if (Input.GetKeyDown(saveKey))
			{
				Debug.Log("SaveHotkeys: Saving game...");
				gameState.Save();
				nextAllowedTime = Time.unscaledTime + debounceSeconds;
			}
			if (Input.GetKeyDown(loadKey))
			{
				Debug.Log("SaveHotkeys: Loading game...");
				gameState.Load();
				nextAllowedTime = Time.unscaledTime + debounceSeconds;
			}
		}
	}
}


