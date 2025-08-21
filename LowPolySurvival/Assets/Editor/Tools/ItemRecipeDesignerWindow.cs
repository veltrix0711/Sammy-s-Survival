using UnityEditor;
using UnityEngine;
using LowPolySurvival.Game.Gameplay.Data;

public class ItemRecipeDesignerWindow : EditorWindow
{
	private string itemId = "";
	private string itemName = "";
	private float weight = 0.1f;
	private float volume = 0.1f;
	private string folder = "Assets/Content/ScriptableObjects/Items";

	[MenuItem("Tools/Item & Recipe Designer")] 
	public static void Open()
	{
		GetWindow<ItemRecipeDesignerWindow>("Item & Recipe Designer");
	}

	private void OnGUI()
	{
		GUILayout.Label("Create Item", EditorStyles.boldLabel);
		itemId = EditorGUILayout.TextField("Item Id", itemId);
		itemName = EditorGUILayout.TextField("Display Name", itemName);
		weight = EditorGUILayout.FloatField("Weight (kg)", weight);
		volume = EditorGUILayout.FloatField("Volume (L)", volume);
		folder = EditorGUILayout.TextField("Folder", folder);
		if (GUILayout.Button("Create ItemDefinition"))
		{
			CreateItem();
		}

		EditorGUILayout.Space(12);
		GUILayout.Label("Open Registry", EditorStyles.boldLabel);
		if (GUILayout.Button("Select ItemRegistry in Resources"))
		{
			var reg = Resources.Load<ItemRegistry>("ItemRegistry");
			if (reg != null) Selection.activeObject = reg;
		}
	}

	private void CreateItem()
	{
		if (string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(itemName))
		{
			EditorUtility.DisplayDialog("Invalid", "Item Id and Display Name are required.", "OK");
			return;
		}
		if (!AssetDatabase.IsValidFolder(folder))
		{
			EditorUtility.DisplayDialog("Folder Missing", "Folder does not exist: " + folder, "OK");
			return;
		}
		var path = System.IO.Path.Combine(folder, itemName + ".asset");
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
	}
}
