using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	[DefaultExecutionOrder(200)]
	public sealed class PlayerInteraction : MonoBehaviour
	{
		[SerializeField] private InventorySystem inventory;
		[SerializeField] private DemoClothBandage clothBandage;
		[SerializeField] private ItemDefinition clothDef;
		[SerializeField] private float interactRange = 6f;
		[SerializeField] private LayerMask raycastMask = ~0;

		private Camera cam;

		private void Awake()
		{
			cam = Camera.main;
		}

		private void Update()
		{
			if (cam == null) cam = Camera.main;
			if (Input.GetKeyDown(KeyCode.E))
			{
				Debug.Log("Pressed E: Craft Bandage");
				clothBandage?.TryMakeBandage();
			}

			if (Input.GetKeyDown(KeyCode.X))
			{
				Debug.Log("Pressed X: TryCut");
				TryCut();
			}
		}

		private void TryCut()
		{
			if (cam == null || inventory == null || clothDef == null) return;
			var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.cyan, 0.25f);
			if (Physics.Raycast(ray, out var hit, interactRange, raycastMask, QueryTriggerInteraction.Ignore))
			{
				var t = hit.collider != null ? hit.collider.transform : null;
				var src = t != null ? t.GetComponentInParent<ClothSource>() : null;
				if (src != null)
				{
					inventory.Add(clothDef, 1);
					Debug.Log("Cut cloth: +1");
				}
			}
		}
	}
}
