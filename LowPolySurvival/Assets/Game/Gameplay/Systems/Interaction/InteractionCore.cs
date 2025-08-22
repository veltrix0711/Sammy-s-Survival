using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Systems.Interaction
{
	public enum InteractionVerb { Use, Cut, Take, Open, Close }

	public interface IInteractable
	{
		string GetPrompt();
		bool CanInteract(InteractionVerb verb, GameObject interactor);
		bool Interact(InteractionVerb verb, GameObject interactor);
	}

	public abstract class InteractableBase : MonoBehaviour, IInteractable
	{
		[SerializeField] protected string displayName = "Object";
		public virtual string GetPrompt() => displayName;
		public abstract bool CanInteract(InteractionVerb verb, GameObject interactor);
		public abstract bool Interact(InteractionVerb verb, GameObject interactor);
	}
}


