using UnityEngine;

namespace LowPolySurvival.Game.Core.Voxel
{
	public sealed class VoxelDebugCarver : MonoBehaviour
	{
		[SerializeField] private float radius = 0.5f;
		[SerializeField] private LayerMask terrainMask = ~0;
		private Camera cam;

		private void Awake()
		{
			cam = Camera.main;
		}

		private void Update()
		{
			if (cam == null) return;
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit, 100f, terrainMask))
			{
				if (Input.GetMouseButtonDown(0))
				{
					// TODO: call into VoxelWorld.DigAt
					Debug.DrawRay(hit.point, Vector3.up, Color.red, 1f);
				}
				if (Input.GetMouseButtonDown(1))
				{
					// TODO: call into VoxelWorld.PlaceAt
					Debug.DrawRay(hit.point, Vector3.up, Color.green, 1f);
				}
			}
		}
	}
}
