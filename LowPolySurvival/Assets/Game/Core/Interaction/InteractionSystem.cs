using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolySurvival.Game.Core.Interaction
{
	public sealed class InteractionSystem : MonoBehaviour
	{
		private readonly Dictionary<string, Action<GameObject>> verbToAction = new Dictionary<string, Action<GameObject>>(StringComparer.OrdinalIgnoreCase);

		public void RegisterVerb(string verb, Action<GameObject> handler)
		{
			if (string.IsNullOrWhiteSpace(verb) || handler == null) return;
			verbToAction[verb] = handler;
		}

		public bool TryInvoke(string verb, GameObject target)
		{
			if (verbToAction.TryGetValue(verb, out var action))
			{
				action.Invoke(target);
				return true;
			}
			return false;
		}
	}
}
