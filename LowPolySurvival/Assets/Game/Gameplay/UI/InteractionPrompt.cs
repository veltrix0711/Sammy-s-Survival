using UnityEngine;
using LowPolySurvival.Game.Gameplay.Systems.Interaction;

namespace LowPolySurvival.Game.Gameplay.UI
{
	[DefaultExecutionOrder(350)]
	public sealed class InteractionPrompt : MonoBehaviour
	{
		[SerializeField] private Camera cam;
		[SerializeField] private float range = 6f;
		[SerializeField] private LayerMask mask = ~0;
		[SerializeField] private GUIStyle style;
		private string currentText = "";

		private void Awake()
		{
			if (cam == null) cam = Camera.main;
			if (style == null)
			{
				style = new GUIStyle(GUI.skin.label);
				style.fontSize = 16; style.normal.textColor = Color.white;
			}
		}

		private void Update()
		{
			if (cam == null) cam = Camera.main;
			currentText = "";
			if (cam == null) return;
			var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			if (Physics.Raycast(ray, out var hit, range, mask, QueryTriggerInteraction.Ignore))
			{
				var it = hit.collider != null ? hit.collider.GetComponentInParent<IInteractable>() : null;
				if (it != null)
				{
					string prompt = it.GetPrompt();
					currentText = $"E: Use {prompt}\nX: Cut {prompt}";
				}
			}
		}

		private void OnGUI()
		{
			if (string.IsNullOrEmpty(currentText)) return;
			const int w = 320; const int h = 48;
			int x = (Screen.width - w) / 2;
			int y = Screen.height - 100;
			GUI.Box(new Rect(x - 8, y - 8, w + 16, h + 16), "");
			GUI.Label(new Rect(x, y, w, h), currentText, style);
		}
	}
}


