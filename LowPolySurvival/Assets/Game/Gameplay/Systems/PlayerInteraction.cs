using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class PlayerInteraction : MonoBehaviour
	{
		[SerializeField] private InventorySystem inventory;
		[SerializeField] private DemoClothBandage clothBandage;
		[SerializeField] private ItemDefinition clothDef;
		[SerializeField] private float interactRange = 3f;

		private Camera cam;

		private void Awake()
		{
			cam = Camera.main;
			if (cam == null)
			{
				var camGo = new GameObject("Main Camera");
				cam = camGo.AddComponent<Camera>();
				cam.tag = "MainCamera";
				cam.transform.position = transform.position + new Vector3(0, 1.7f, -4f);
				cam.transform.LookAt(transform.position + Vector3.forward * 5f);
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				clothBandage?.TryMakeBandage();
			}

			if (Input.GetKeyDown(KeyCode.X))
			{
				TryCut();
			}
		}

		private void TryCut()
		{
			if (cam == null || inventory == null || clothDef == null) return;
			var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			if (Physics.Raycast(ray, out var hit, interactRange))
			{
				if (hit.collider != null && (hit.collider.CompareTag("ClothSource") || hit.collider.GetComponent<ClothSource>() != null))
				{
					inventory.Add(clothDef, 1);
				}
			}
		}
	}
}
