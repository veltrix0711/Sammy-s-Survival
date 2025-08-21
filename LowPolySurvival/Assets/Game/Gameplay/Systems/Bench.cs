using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class Bench : MonoBehaviour
	{
		[Tooltip("Craft speed multiplier ( >1 is faster )")]
		[SerializeField] private float speedMultiplier = 1f;
		[Tooltip("Quality multiplier ( >1 improves quality )")]
		[SerializeField] private float qualityMultiplier = 1f;

		public float SpeedMultiplier => Mathf.Max(0.1f, speedMultiplier);
		public float QualityMultiplier => Mathf.Max(0.1f, qualityMultiplier);
	}
}
