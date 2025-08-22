using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Systems.Interaction
{
	public sealed class DoorInteractable : InteractableBase
	{
		[SerializeField] private Transform hinge;
		[SerializeField] private float openAngle = 90f;
		[SerializeField] private float speed = 4f;
		private bool isOpen;
		private float current;

		private void Reset()
		{
			displayName = "Door";
		}

		private void Update()
		{
			if (hinge == null) hinge = this.transform;
			float target = isOpen ? openAngle : 0f;
			current = Mathf.MoveTowards(current, target, speed * Time.deltaTime * openAngle);
			hinge.localRotation = Quaternion.Euler(0, current, 0);
		}

		public override bool CanInteract(InteractionVerb verb, GameObject interactor)
		{
			return verb == InteractionVerb.Open || verb == InteractionVerb.Close || verb == InteractionVerb.Use;
		}

		public override bool Interact(InteractionVerb verb, GameObject interactor)
		{
			if (verb == InteractionVerb.Use) isOpen = !isOpen;
			else if (verb == InteractionVerb.Open) isOpen = true;
			else if (verb == InteractionVerb.Close) isOpen = false;
			return true;
		}
	}
}


