using UnityEditor;
using UnityEngine;
using System.Linq;
using LowPolySurvival.Game.Gameplay.Data;

public class WeaponViewerWindow : EditorWindow
{
	private WeaponDefinition weapon;
	private GameObject previewRoot;
	private GameObject weaponInstance;
	private Vector2 scroll;

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
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"{s.slotType}", GUILayout.Width(120));
			EditorGUILayout.LabelField($"Mount: {s.mountPointName}");
			if (GUILayout.Button("Select Mount", GUILayout.Width(100)))
			{
				SelectMount(s.mountPointName);
			}
			if (GUILayout.Button("Detach", GUILayout.Width(80)))
			{
				DetachFromMount(s.mountPointName);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();

		EditorGUILayout.Space(6);
		EditorGUILayout.LabelField("Drag AddonDefinition here to attach", EditorStyles.helpBox);
		var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
		GUI.Box(dropArea, "Drop Addon");
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
		if (mount != null) Selection.activeTransform = mount;
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
							TryAttach(ad);
							break;
						}
					}
				}
				Event.current.Use();
				break;
		}
	}

	private void TryAttach(AddonDefinition addon)
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
		var inst = (GameObject)PrefabUtility.InstantiatePrefab(addon.AddonPrefab);
		inst.transform.SetParent(mount, false);
		inst.transform.localPosition = addon.LocalPositionOffset;
		inst.transform.localEulerAngles = addon.LocalEulerOffset;
		inst.transform.localScale = addon.LocalScale;
		Selection.activeGameObject = inst;
	}
}
