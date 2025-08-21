using UnityEditor;
using UnityEngine;
using System.Linq;
using LowPolySurvival.Game.Gameplay.Data;
using System.IO;

public class WeaponViewerWindow : EditorWindow
{
	private WeaponDefinition weapon;
	private GameObject previewRoot;
	private GameObject weaponInstance;
	private Vector2 scroll;
	private AddonDefinition pendingAddon;
	private string variantsFolder = "Assets/Content/Prefabs/WeaponVariants";
	private Transform selectedMount;
	private EditorAttachedAddonMarker selectedAddonMarker;
	private Vector3 editPos;
	private Vector3 editEuler;
	private Vector3 editScale = Vector3.one;

	// Animation preview
	private AnimationClip[] clips;
	private int clipIndex;
	private bool playing;
	private float playhead;
	private double lastTime;

	[MenuItem("Tools/Weapon Viewer")] 
	public static void Open() => GetWindow<WeaponViewerWindow>("Weapon Viewer");

	private void OnDisable()
	{
		Cleanup();
		AnimationMode.StopAnimationMode();
	}

	private void Cleanup()
	{
		if (weaponInstance != null) DestroyImmediate(weaponInstance);
		if (previewRoot != null) DestroyImmediate(previewRoot);
	}

	private void OnGUI()
	{
		weapon = (WeaponDefinition)EditorGUILayout.ObjectField("Weapon", weapon, typeof(WeaponDefinition), false);
		if (weapon != null && GUILayout.Button("Load Weapon Prefab"))
		{
			LoadWeapon();
		}
		EditorGUILayout.Space(6);
		if (weapon != null)
		{
			DrawSlotsUI();
			EditorGUILayout.Space(6);
			DrawAnimationUI();
			EditorGUILayout.Space(6);
			DrawUtilityUI();
			EditorGUILayout.Space(6);
			DrawAttachmentEditor();
		}

		if (playing) Repaint();
	}

	private void LoadWeapon()
	{
		Cleanup();
		previewRoot = new GameObject("WeaponPreviewRoot");
		if (weapon.WeaponPrefab != null)
		{
			weaponInstance = (GameObject)PrefabUtility.InstantiatePrefab(weapon.WeaponPrefab);
			weaponInstance.transform.SetParent(previewRoot.transform);
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;
		}
		// Load clips
		clips = null;
		clipIndex = 0;
		playing = false;
		playhead = 0f;
		if (weapon.AnimatorController != null)
		{
			clips = weapon.AnimatorController.animationClips;
		}
		AnimationMode.StopAnimationMode();
	}

	private void DrawSlotsUI()
	{
		EditorGUILayout.LabelField("Slots", EditorStyles.boldLabel);
		scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(140));
		foreach (var s in weapon.Slots)
		{
			Rect row = EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"{s.slotType}", GUILayout.Width(120));
			EditorGUILayout.LabelField($"Mount: {s.mountPointName}");
			int attachedCount = CountAttachments(s.mountPointName);
			EditorGUILayout.LabelField($"Attached: {attachedCount}", GUILayout.Width(90));
			if (GUILayout.Button("Select", GUILayout.Width(70))) { SelectMount(s.mountPointName); }
			if (GUILayout.Button("Detach", GUILayout.Width(70))) { DetachFromMount(s.mountPointName); }
			EditorGUILayout.EndHorizontal();
			HandlePerRowDrop(row, s);
		}
		EditorGUILayout.EndScrollView();

		EditorGUILayout.Space(6);
		EditorGUILayout.LabelField("Global Addon Drop (auto-slot)", EditorStyles.helpBox);
		var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
		GUI.Box(dropArea, pendingAddon == null ? "Drop AddonDefinition here" : $"Pending: {pendingAddon.DisplayName}");
		HandleAddonDrop(dropArea);
	}

	private void DrawAnimationUI()
	{
		EditorGUILayout.LabelField("Animation Preview", EditorStyles.boldLabel);
		if (clips == null || clips.Length == 0)
		{
			EditorGUILayout.HelpBox("No clips available. Assign an AnimatorController on the WeaponDefinition.", MessageType.Info);
			return;
		}
		string[] names = clips.Select(c => c != null ? c.name : "<null>").ToArray();
		clipIndex = EditorGUILayout.Popup("Clip", clipIndex, names);
		var clip = clips[clipIndex];
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(playing ? "Pause" : "Play", GUILayout.Width(80)))
		{
			playing = !playing;
			lastTime = EditorApplication.timeSinceStartup;
			if (playing) AnimationMode.StartAnimationMode(); else AnimationMode.StopAnimationMode();
		}
		if (GUILayout.Button("Stop", GUILayout.Width(80)))
		{
			playing = false; playhead = 0f; AnimationMode.StopAnimationMode();
		}
		EditorGUILayout.EndHorizontal();
		playhead = EditorGUILayout.Slider("Time", playhead, 0f, clip.length);
		if (weaponInstance != null && clip != null)
		{
			if (playing)
			{
				double now = EditorApplication.timeSinceStartup;
				double dt = now - lastTime; lastTime = now;
				playhead += (float)dt;
				if (playhead > clip.length) playhead = 0f;
			}
			AnimationMode.SampleAnimationClip(weaponInstance, clip, playhead);
		}
	}

	private void SelectMount(string mountName)
	{
		if (weaponInstance == null) return;
		var mount = weaponInstance.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == mountName);
		if (mount != null)
		{
			Selection.activeTransform = mount;
			selectedMount = mount;
			selectedAddonMarker = mount.GetComponentInChildren<EditorAttachedAddonMarker>();
			if (selectedAddonMarker != null)
			{
				var tr = selectedAddonMarker.transform;
				editPos = tr.localPosition;
				editEuler = tr.localEulerAngles;
				editScale = tr.localScale;
			}
		}
	}

	private void DetachFromMount(string mountName)
	{
		if (weaponInstance == null) return;
		var mount = weaponInstance.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == mountName);
		if (mount == null) return;
		for (int i = mount.childCount - 1; i >= 0; i--)
		{
			var c = mount.GetChild(i);
			DestroyImmediate(c.gameObject);
		}
	}

	private void HandleAddonDrop(Rect dropArea)
	{
		var e = Event.current;
		switch (e.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!dropArea.Contains(e.mousePosition)) return;
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (e.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					foreach (var obj in DragAndDrop.objectReferences)
					{
						if (obj is AddonDefinition ad)
						{
							pendingAddon = ad;
							TryAttachAuto(ad);
							break;
						}
					}
				}
				Event.current.Use();
				break;
		}
	}

	private void HandlePerRowDrop(Rect rowRect, WeaponDefinition.Slot s)
	{
		var e = Event.current;
		switch (e.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!rowRect.Contains(e.mousePosition)) return;
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				if (e.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					foreach (var obj in DragAndDrop.objectReferences)
					{
						if (obj is AddonDefinition ad)
						{
							TryAttachToSlot(ad, s);
							break;
						}
					}
				}
				Event.current.Use();
				break;
		}
	}

	private void TryAttachAuto(AddonDefinition addon)
	{
		if (weaponInstance == null || addon == null) return;
		// Find a compatible slot
		var slot = weapon.Slots.FirstOrDefault(s => addon.CompatibleSlotTypes != null && addon.CompatibleSlotTypes.Contains(s.slotType));
		if (string.IsNullOrEmpty(slot.mountPointName))
		{
			EditorUtility.DisplayDialog("No Compatible Slot", "No matching slot for this addon.", "OK");
			return;
		}
		var mount = weaponInstance.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == slot.mountPointName);
		if (mount == null)
		{
			EditorUtility.DisplayDialog("Mount Not Found", $"Child transform '{slot.mountPointName}' not found on weapon prefab.", "OK");
			return;
		}
		if (addon.AddonPrefab == null)
		{
			EditorUtility.DisplayDialog("Addon Prefab Missing", "Assign an Addon Prefab on the AddonDefinition.", "OK");
			return;
		}
		AttachInstance(addon, mount);
		SelectMount(mount.name);
	}

	private void TryAttachToSlot(AddonDefinition addon, WeaponDefinition.Slot slot)
	{
		if (weaponInstance == null || addon == null) return;
		if (addon.CompatibleSlotTypes == null || !addon.CompatibleSlotTypes.Contains(slot.slotType))
		{
			EditorUtility.DisplayDialog("Incompatible", $"Addon does not support slot type '{slot.slotType}'.", "OK");
			return;
		}
		var mount = weaponInstance.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == slot.mountPointName);
		if (mount == null)
		{
			EditorUtility.DisplayDialog("Mount Not Found", $"Child transform '{slot.mountPointName}' not found on weapon prefab.", "OK");
			return;
		}
		if (addon.AddonPrefab == null)
		{
			EditorUtility.DisplayDialog("Addon Prefab Missing", "Assign an Addon Prefab on the AddonDefinition.", "OK");
			return;
		}
		AttachInstance(addon, mount);
		SelectMount(mount.name);
	}

	private void AttachInstance(AddonDefinition addon, Transform mount)
	{
		var inst = (GameObject)PrefabUtility.InstantiatePrefab(addon.AddonPrefab);
		inst.transform.SetParent(mount, false);
		inst.transform.localPosition = addon.LocalPositionOffset;
		inst.transform.localEulerAngles = addon.LocalEulerOffset;
		inst.transform.localScale = addon.LocalScale;
		var marker = inst.AddComponent<EditorAttachedAddonMarker>();
		marker.sourceDefinition = addon;
		Selection.activeGameObject = inst;
	}

	private int CountAttachments(string mountName)
	{
		if (weaponInstance == null) return 0;
		var mount = weaponInstance.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == mountName);
		return mount == null ? 0 : mount.childCount;
	}

	private void DrawUtilityUI()
	{
		EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		variantsFolder = EditorGUILayout.TextField("Variants Folder", variantsFolder);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Clear All Attachments", GUILayout.Height(24))) { ClearAllAttachments(); }
		if (GUILayout.Button("Save As Prefab Variant", GUILayout.Height(24))) { SaveVariant(); }
		EditorGUILayout.EndHorizontal();
	}

	private void DrawAttachmentEditor()
	{
		EditorGUILayout.LabelField("Attachment Editor", EditorStyles.boldLabel);
		if (selectedMount == null)
		{
			EditorGUILayout.HelpBox("Select a mount (Select button) to edit attached addon transforms.", MessageType.Info);
			return;
		}
		selectedAddonMarker = selectedMount.GetComponentInChildren<EditorAttachedAddonMarker>();
		if (selectedAddonMarker == null)
		{
			EditorGUILayout.HelpBox("No addon attached under selected mount.", MessageType.Info);
			return;
		}
		EditorGUILayout.ObjectField("Addon", selectedAddonMarker.sourceDefinition, typeof(AddonDefinition), false);
		editPos = EditorGUILayout.Vector3Field("Local Position", editPos);
		editEuler = EditorGUILayout.Vector3Field("Local Euler", editEuler);
		editScale = EditorGUILayout.Vector3Field("Local Scale", editScale);
		if (GUILayout.Button("Apply To Instance"))
		{
			var tr = selectedAddonMarker.transform;
			Undo.RecordObject(tr, "Edit Addon Transform");
			tr.localPosition = editPos;
			tr.localEulerAngles = editEuler;
			tr.localScale = editScale;
		}
		if (GUILayout.Button("Write Back To AddonDefinition"))
		{
			var ad = selectedAddonMarker.sourceDefinition;
			if (ad != null)
			{
				var so = new SerializedObject(ad);
				so.FindProperty("localPositionOffset").vector3Value = editPos;
				so.FindProperty("localEulerOffset").vector3Value = editEuler;
				so.FindProperty("localScale").vector3Value = editScale;
				so.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(ad);
				AssetDatabase.SaveAssets();
			}
		}
	}

	private void ClearAllAttachments()
	{
		if (weaponInstance == null) return;
		foreach (var s in weapon.Slots)
		{
			DetachFromMount(s.mountPointName);
		}
	}

	private void SaveVariant()
	{
		if (weaponInstance == null || weapon == null) return;
		if (!AssetDatabase.IsValidFolder(variantsFolder))
		{
			Directory.CreateDirectory(variantsFolder);
			AssetDatabase.Refresh();
		}
		string name = weapon.DisplayName + "_Variant";
		string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(variantsFolder, name + ".prefab"));
		var prefab = PrefabUtility.SaveAsPrefabAsset(weaponInstance, path);
		if (prefab != null)
		{
			EditorUtility.DisplayDialog("Saved", path, "OK");
		}
	}
}
