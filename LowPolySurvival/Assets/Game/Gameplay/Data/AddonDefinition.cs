using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	[CreateAssetMenu(fileName = "AddonDefinition", menuName = "LowPolySurvival/Addon", order = 3)]
	public sealed class AddonDefinition : ScriptableObject
	{
		[SerializeField] private string addonId;
		[SerializeField] private string displayName;
		[SerializeField] private string[] compatibleSlotTypes; // e.g., Muzzle, Optic
		[SerializeField] private GameObject addonPrefab;       // visual prefab to attach
		[SerializeField] private Vector3 localPositionOffset;  // default offsets when attached
		[SerializeField] private Vector3 localEulerOffset;
		[SerializeField] private Vector3 localScale = Vector3.one;
		[SerializeField] private float weightDeltaKg = 0.1f;
		[SerializeField] private float recoilMultiplier = 1f;
		[SerializeField] private float spreadMultiplier = 1f;

		public string AddonId => addonId;
		public string DisplayName => displayName;
		public string[] CompatibleSlotTypes => compatibleSlotTypes;
		public GameObject AddonPrefab => addonPrefab;
		public Vector3 LocalPositionOffset => localPositionOffset;
		public Vector3 LocalEulerOffset => localEulerOffset;
		public Vector3 LocalScale => localScale;
		public float WeightDeltaKg => weightDeltaKg;
		public float RecoilMultiplier => Mathf.Max(0.1f, recoilMultiplier);
		public float SpreadMultiplier => Mathf.Max(0.1f, spreadMultiplier);
	}
}
