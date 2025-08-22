using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Data
{
	[CreateAssetMenu(fileName = "RecipeDefinition", menuName = "LowPolySurvival/Recipe", order = 1)]
	public sealed class RecipeDefinition : ScriptableObject
	{
		[System.Serializable]
		public struct Stack
		{
			public ItemDefinition item;
			public int quantity;
		}

		[SerializeField] private string recipeId;
		[SerializeField] private Stack[] inputs;
		[SerializeField] private Stack[] outputs;
		[SerializeField] private string requiredKnowledgeToken; // empty for basics
		[SerializeField] private float baseSeconds = 5f;
		[SerializeField] private bool requireBench = false;

		public string RecipeId => recipeId;
		public Stack[] Inputs => inputs;
		public Stack[] Outputs => outputs;
		public string RequiredKnowledgeToken => requiredKnowledgeToken;
		public float BaseSeconds => baseSeconds;
		public bool RequireBench => requireBench;
	}
}
