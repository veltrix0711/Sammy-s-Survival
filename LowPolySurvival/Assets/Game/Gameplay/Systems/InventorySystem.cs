using System.Collections.Generic;
using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;
using System.Linq;

namespace LowPolySurvival.Game.Gameplay.Systems
{
	public sealed class InventorySystem : MonoBehaviour
	{
		private readonly Dictionary<ItemDefinition, List<ItemInstance>> definitionToInstances = new Dictionary<ItemDefinition, List<ItemInstance>>();

		public int GetCount(ItemDefinition definition)
		{
			if (definition == null) return 0;
			if (!definitionToInstances.TryGetValue(definition, out var list) || list == null) return 0;
			int sum = 0;
			foreach (var inst in list) sum += Mathf.Max(0, inst.StackCount);
			return sum;
		}

		public IEnumerable<ItemInstance> GetInstances(ItemDefinition definition)
		{
			if (definition == null) yield break;
			if (!definitionToInstances.TryGetValue(definition, out var list) || list == null) yield break;
			foreach (var inst in list) yield return inst;
		}

		public void Add(ItemDefinition definition, int quantity)
		{
			if (definition == null || quantity <= 0) return;
			Ensure(definition);
			if (definition.Stackable)
			{
				int remaining = quantity;
				var stacks = definitionToInstances[definition];
				int maxStack = definition.MaxStack;
				foreach (var s in stacks)
				{
					if (remaining <= 0) break;
					int canAdd = Mathf.Max(0, maxStack - s.StackCount);
					if (canAdd <= 0) continue;
					int add = Mathf.Min(canAdd, remaining);
					s.StackCount += add;
					remaining -= add;
				}
				while (remaining > 0)
				{
					int add = Mathf.Min(remaining, maxStack);
					var inst = new ItemInstance(definition, definition.DefaultCondition, add);
					definitionToInstances[definition].Add(inst);
					remaining -= add;
				}
			}
			else
			{
				for (int i = 0; i < quantity; i++)
				{
					var inst = new ItemInstance(definition, definition.DefaultCondition, 1);
					definitionToInstances[definition].Add(inst);
				}
			}
		}

		public void AddWithCondition(ItemDefinition definition, int quantity, ConditionSet condition)
		{
			if (definition == null || quantity <= 0) return;
			Ensure(definition);
			if (definition.Stackable)
			{
				int remaining = quantity;
				int maxStack = definition.MaxStack;
				while (remaining > 0)
				{
					int add = Mathf.Min(remaining, maxStack);
					var inst = new ItemInstance(definition, condition, add);
					definitionToInstances[definition].Add(inst);
					remaining -= add;
				}
			}
			else
			{
				for (int i = 0; i < quantity; i++)
				{
					var inst = new ItemInstance(definition, condition, 1);
					definitionToInstances[definition].Add(inst);
				}
			}
		}

		public bool Remove(ItemDefinition definition, int quantity)
		{
			if (definition == null || quantity <= 0) return false;
			if (!definitionToInstances.TryGetValue(definition, out var list) || list == null) return false;
			int have = GetCount(definition);
			if (have < quantity) return false;
			int remaining = quantity;
			foreach (var stack in list.OrderByDescending(s => s.StackCount).ToList())
			{
				if (remaining <= 0) break;
				int take = Mathf.Min(stack.StackCount, remaining);
				stack.StackCount -= take;
				remaining -= take;
				if (stack.StackCount <= 0) list.Remove(stack);
			}
			if (list.Count == 0) definitionToInstances.Remove(definition);
			return true;
		}

		public IEnumerable<KeyValuePair<ItemDefinition, int>> DebugEnumerate()
		{
			foreach (var kv in definitionToInstances)
			{
				yield return new KeyValuePair<ItemDefinition, int>(kv.Key, GetCount(kv.Key));
			}
		}

		public IEnumerable<ItemInstance> DebugEnumerateInstances()
		{
			foreach (var kv in definitionToInstances)
				foreach (var inst in kv.Value)
					yield return inst;
		}

		public void DebugClear()
		{
			definitionToInstances.Clear();
		}

		private void Ensure(ItemDefinition def)
		{
			if (!definitionToInstances.ContainsKey(def)) definitionToInstances[def] = new List<ItemInstance>(4);
		}
	}
}
