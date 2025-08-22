using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;
using LowPolySurvival.Game.Gameplay.Systems.Interaction;

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
				TryVerb(InteractionVerb.Use);
			}

			if (Input.GetKeyDown(KeyCode.X))
			{
				TryVerb(InteractionVerb.Cut);
			}

			if (Input.GetKeyDown(KeyCode.F))
			{
				TryVerb(InteractionVerb.Take);
			}
		}

		private void TryVerb(InteractionVerb verb)
		{
			if (cam == null) return;
			var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.cyan, 0.25f);
			if (!Physics.Raycast(ray, out var hit, interactRange, raycastMask, QueryTriggerInteraction.Ignore)) return;
			var t = hit.collider != null ? hit.collider.transform : null;
			if (t == null) return;
			var interactable = t.GetComponentInParent<IInteractable>();
			if (interactable != null)
			{
				if (interactable.CanInteract(verb, this.gameObject))
				{
					interactable.Interact(verb, this.gameObject);
					return;
				}
			}
			// Fallback: legacy cloth cut for demo
			if (verb == InteractionVerb.Cut && inventory != null && clothDef != null)
			{
				var src = t.GetComponentInParent<ClothSource>();
				if (src != null)
				{
					inventory.Add(clothDef, 1);
					Debug.Log("Cut cloth: +1");
				}
			}
		}
	}
}
