using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LowPolySurvival.Game.Gameplay.Data;

public class ItemRecipeDesignerWindow : EditorWindow
{
	private enum Tab { Items, Addons, Recipes, Tools }
	private Tab activeTab;

	// Common
	private string itemsFolder = "Assets/Content/ScriptableObjects/Items";
	private string addonsFolder = "Assets/Content/ScriptableObjects/Addons";
	private string recipesFolder = "Assets/Content/ScriptableObjects/Recipes";
	private string prefabsFolder = "Assets/Content/Prefabs";

	// Items
	private string itemId = "";
	private string itemName = "";
	private float weight = 0.1f;
	private float volume = 0.1f;
	private Vector3 scalePreset = Vector3.one;
	private string[] scalePresetNames = new[] { "1u (1m)", "0.5u (Small)", "2u (Large)" };
	private Vector3[] scalePresetValues = new[] { Vector3.one, Vector3.one * 0.5f, Vector3.one * 2f };
	private GameObject sourcePrefab;

	// Addons
	private string addonId = "";
	private string addonName = "";

	// Recipes
	private string recipeId = "";
	private List<ItemDefinition> recipeInputs = new List<ItemDefinition>();
	private List<int> recipeInputCounts = new List<int>();
	private List<ItemDefinition> recipeOutputs = new List<ItemDefinition>();
	private List<int> recipeOutputCounts = new List<int>();
	private string requiredKnowledge = "";
	private float baseSeconds = 5f;

	[MenuItem("Tools/Item & Recipe Designer")]
	public static void Open()
	{
		GetWindow<ItemRecipeDesignerWindow>("Designer");
	}

	private void OnGUI()
	{
		DrawHeader();
		activeTab = (Tab)GUILayout.Toolbar((int)activeTab, new[] { "Items", "Addons", "Recipes", "Tools" });
		EditorGUILayout.Space(6);
		switch (activeTab)
		{
			case Tab.Items: DrawItemsTab(); break;
			case Tab.Addons: DrawAddonsTab(); break;
			case Tab.Recipes: DrawRecipesTab(); break;
			case Tab.Tools: DrawToolsTab(); break;
		}
	}

	private void DrawHeader()
	{
		EditorGUILayout.LabelField("Folders", EditorStyles.boldLabel);
		itemsFolder = EditorGUILayout.TextField("Items", itemsFolder);
		addonsFolder = EditorGUILayout.TextField("Addons", addonsFolder);
		recipesFolder = EditorGUILayout.TextField("Recipes", recipesFolder);
		prefabsFolder = EditorGUILayout.TextField("Prefabs", prefabsFolder);
		EditorGUILayout.Space(6);
	}

	private void DrawItemsTab()
	{
		EditorGUILayout.LabelField("Create Item", EditorStyles.boldLabel);
		itemId = EditorGUILayout.TextField("Item Id", itemId);
		itemName = EditorGUILayout.TextField("Display Name", itemName);
		weight = EditorGUILayout.FloatField("Weight (kg)", weight);
		volume = EditorGUILayout.FloatField("Volume (L)", volume);
		var sel = GUILayout.SelectionGrid(-1, scalePresetNames, 3);
		if (sel >= 0) scalePreset = scalePresetValues[sel];
		sourcePrefab = (GameObject)EditorGUILayout.ObjectField("Source Prefab (optional)", sourcePrefab, typeof(GameObject), false);
		if (GUILayout.Button("Create ItemDefinition"))
		{
			CreateItem();
		}
	}

	private void DrawAddonsTab()
	{
		EditorGUILayout.LabelField("Create Addon", EditorStyles.boldLabel);
		addonId = EditorGUILayout.TextField("Addon Id", addonId);
		addonName = EditorGUILayout.TextField("Display Name", addonName);
		if (GUILayout.Button("Create AddonDefinition"))
		{
			CreateAddon();
		}
	}

	private void DrawRecipesTab()
	{
		EditorGUILayout.LabelField("Create Recipe", EditorStyles.boldLabel);
		recipeId = EditorGUILayout.TextField("Recipe Id", recipeId);
		baseSeconds = EditorGUILayout.FloatField("Base Seconds", baseSeconds);
		requiredKnowledge = EditorGUILayout.TextField("Knowledge Token", requiredKnowledge);
		EditorGUILayout.Space(6);
		EditorGUILayout.LabelField("Inputs", EditorStyles.boldLabel);
		DrawStacks(recipeInputs, recipeInputCounts);
		EditorGUILayout.Space(6);
		EditorGUILayout.LabelField("Outputs", EditorStyles.boldLabel);
		DrawStacks(recipeOutputs, recipeOutputCounts);
		EditorGUILayout.Space(6);
		if (GUILayout.Button("Create RecipeDefinition"))
		{
			CreateRecipe();
		}
	}

	private void DrawStacks(List<ItemDefinition> defs, List<int> counts)
	{
		int remove = -1;
		for (int i = 0; i < defs.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			defs[i] = (ItemDefinition)EditorGUILayout.ObjectField(defs[i], typeof(ItemDefinition), false);
			if (i >= counts.Count) counts.Add(1);
			counts[i] = EditorGUILayout.IntField(counts[i], GUILayout.Width(60));
			if (GUILayout.Button("-", GUILayout.Width(24))) remove = i;
			EditorGUILayout.EndHorizontal();
		}
		if (remove >= 0) { defs.RemoveAt(remove); counts.RemoveAt(remove); }
		if (GUILayout.Button("+ Add")) { defs.Add(null); counts.Add(1); }
	}

	private void DrawToolsTab()
	{
		EditorGUILayout.LabelField("Registry", EditorStyles.boldLabel);
		if (GUILayout.Button("Open ItemRegistry (Resources)"))
		{
			var reg = Resources.Load<ItemRegistry>("ItemRegistry");
			if (reg != null) Selection.activeObject = reg;
		}
		if (GUILayout.Button("Sync Registry with Items Folder"))
		{
			SyncRegistry();
		}
		EditorGUILayout.Space(6);
		EditorGUILayout.LabelField("Prefab Helpers", EditorStyles.boldLabel);
		if (GUILayout.Button("Create Placeholder Prefab for Selected Item"))
		{
			CreatePlaceholderPrefab();
		}
		EditorGUILayout.Space(6);
		EditorGUILayout.LabelField("Backups", EditorStyles.boldLabel);
		if (GUILayout.Button("Create Docs Backup Snapshot"))
		{
			CreateDocsBackup();
		}
	}

	private void CreateItem()
	{
		if (!ValidateFolder(itemsFolder)) return;
		if (string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(itemName))
		{
			EditorUtility.DisplayDialog("Invalid", "Item Id and Display Name are required.", "OK");
			return;
		}
		var path = Path.Combine(itemsFolder, itemName + ".asset");
		var def = ScriptableObject.CreateInstance<ItemDefinition>();
		AssetDatabase.CreateAsset(def, path);
		var so = new SerializedObject(def);
		so.FindProperty("itemId").stringValue = itemId;
		so.FindProperty("displayName").stringValue = itemName;
		so.FindProperty("weightKg").floatValue = weight;
		so.FindProperty("volumeLiters").floatValue = volume;
		so.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(def);
		AssetDatabase.SaveAssets();
		Selection.activeObject = def;
		SyncRegistry();
		if (sourcePrefab != null) ScaleAndSavePrefab(def, sourcePrefab);
	}

	private void CreateAddon()
	{
		if (!ValidateFolder(addonsFolder)) return;
		if (string.IsNullOrWhiteSpace(addonId) || string.IsNullOrWhiteSpace(addonName))
		{
			EditorUtility.DisplayDialog("Invalid", "Addon Id and Display Name are required.", "OK");
			return;
		}
		var path = Path.Combine(addonsFolder, addonName + ".asset");
		var def = ScriptableObject.CreateInstance<AddonDefinition>();
		AssetDatabase.CreateAsset(def, path);
		var so = new SerializedObject(def);
		so.FindProperty("addonId").stringValue = addonId;
		so.FindProperty("displayName").stringValue = addonName;
		so.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(def);
		AssetDatabase.SaveAssets();
		Selection.activeObject = def;
	}

	private void CreateRecipe()
	{
		if (!ValidateFolder(recipesFolder)) return;
		if (string.IsNullOrWhiteSpace(recipeId))
		{
			EditorUtility.DisplayDialog("Invalid", "Recipe Id is required.", "OK");
			return;
		}
		var path = Path.Combine(recipesFolder, recipeId + ".asset");
		var def = ScriptableObject.CreateInstance<RecipeDefinition>();
		AssetDatabase.CreateAsset(def, path);
		var so = new SerializedObject(def);
		so.FindProperty("recipeId").stringValue = recipeId;
		ApplyStacks(so.FindProperty("inputs"), recipeInputs, recipeInputCounts);
		ApplyStacks(so.FindProperty("outputs"), recipeOutputs, recipeOutputCounts);
		so.FindProperty("requiredKnowledgeToken").stringValue = requiredKnowledge;
		so.FindProperty("baseSeconds").floatValue = baseSeconds;
		so.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(def);
		AssetDatabase.SaveAssets();
		Selection.activeObject = def;
	}

	private void ApplyStacks(SerializedProperty arrayProp, List<ItemDefinition> defs, List<int> counts)
	{
		arrayProp.arraySize = Mathf.Max(0, defs.Count);
		for (int i = 0; i < defs.Count; i++)
		{
			arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative("item").objectReferenceValue = defs[i];
			arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative("quantity").intValue = counts[i];
		}
	}

	private void SyncRegistry()
	{
		var reg = Resources.Load<ItemRegistry>("ItemRegistry");
		if (reg == null)
		{
			EditorUtility.DisplayDialog("Registry Missing", "Create Resources/ItemRegistry.asset first (via CI → Setup Demo Scene).", "OK");
			return;
		}
		var guids = AssetDatabase.FindAssets("t:ItemDefinition", new[] { itemsFolder });
		var list = new List<ItemDefinition>();
		foreach (var g in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(g);
			var item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
			if (item != null) list.Add(item);
		}
		var so = new SerializedObject(reg);
		var itemsProp = so.FindProperty("items");
		itemsProp.arraySize = list.Count;
		for (int i = 0; i < list.Count; i++)
		{
			itemsProp.GetArrayElementAtIndex(i).objectReferenceValue = list[i];
		}
		so.ApplyModifiedPropertiesWithoutUndo();
		EditorUtility.SetDirty(reg);
		AssetDatabase.SaveAssets();
	}

	private void CreatePlaceholderPrefab()
	{
		var obj = Selection.activeObject as ItemDefinition;
		if (obj == null)
		{
			EditorUtility.DisplayDialog("Select Item", "Select an ItemDefinition asset to create a placeholder prefab.", "OK");
			return;
		}
		if (!ValidateFolder(prefabsFolder)) return;
		var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.localScale = scalePreset;
		go.name = obj.DisplayName + "_Placeholder";
		var path = Path.Combine(prefabsFolder, go.name + ".prefab");
		PrefabUtility.SaveAsPrefabAsset(go, path);
		GameObject.DestroyImmediate(go);
		AssetDatabase.SaveAssets();
		Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
	}

	private void CreateDocsBackup()
	{
		var stamp = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
		var backupDir = Path.Combine("Docs", "Backups", stamp);
		Directory.CreateDirectory(backupDir);
		foreach (var file in new[] { "Docs/REFERENCE.md", "Docs/BUILD_PLAN.md", "Docs/NEEDS.md", "Docs/MAP_PIPELINE.md" })
		{
			if (File.Exists(file)) File.Copy(file, Path.Combine(backupDir, Path.GetFileName(file)), true);
		}
		AssetDatabase.Refresh();
		EditorUtility.DisplayDialog("Backup Created", backupDir, "OK");
	}

	private bool ValidateFolder(string path)
	{
		if (AssetDatabase.IsValidFolder(path)) return true;
		EditorUtility.DisplayDialog("Folder Missing", "Folder does not exist: " + path, "OK");
		return false;
	}

	private void ScaleAndSavePrefab(ItemDefinition def, GameObject prefab)
	{
		if (!ValidateFolder(prefabsFolder)) return;
		var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
		go.transform.localScale = scalePreset;
		go.name = def.DisplayName + "_Prefab";
		var path = Path.Combine(prefabsFolder, go.name + ".prefab");
		PrefabUtility.SaveAsPrefabAsset(go, path);
		GameObject.DestroyImmediate(go);
		AssetDatabase.SaveAssets();
	}
}
