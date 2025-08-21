using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	[CreateAssetMenu(fileName = "WeaponDefinition", menuName = "LowPolySurvival/Weapon", order = 10)]
	public sealed class WeaponDefinition : ScriptableObject
	{
		[System.Serializable]
		public struct Slot
		{
			public string slotType;           // e.g., Muzzle, Optic, Underbarrel
			public string mountPointName;     // child transform name on the weapon prefab
		}

		[SerializeField] private string weaponId;
		[SerializeField] private string displayName;

		// Stats
		[SerializeField] private float baseDamage = 10f;
		[SerializeField] private float fireRate = 5f;          // rounds/sec
		[SerializeField] private float recoil = 1f;            // normalized
		[SerializeField] private float spread = 1f;            // degrees
		[SerializeField] private float baseWeightKg = 3f;

		// Visuals
		[SerializeField] private GameObject weaponPrefab;
		[SerializeField] private RuntimeAnimatorController animatorController;

		// Attachment slots
		[SerializeField] private Slot[] slots;

		public string WeaponId => weaponId;
		public string DisplayName => displayName;
		public float BaseDamage => baseDamage;
		public float FireRate => fireRate;
		public float Recoil => recoil;
		public float Spread => spread;
		public float BaseWeightKg => baseWeightKg;
		public GameObject WeaponPrefab => weaponPrefab;
		public RuntimeAnimatorController AnimatorController => animatorController;
		public Slot[] Slots => slots;
	}
}
