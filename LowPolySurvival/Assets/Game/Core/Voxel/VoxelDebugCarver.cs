using UnityEngine;

namespace LowPolySurvival.Game.Core.Voxel
{
	public sealed class VoxelDebugCarver : MonoBehaviour
	{
		[SerializeField] private VoxelWorld world;
		[SerializeField] private float radius = 0.75f;
		[SerializeField] private LayerMask terrainMask = ~0;
		private Camera cam;

		private void Awake()
		{
			cam = Camera.main;
			if (world == null) world = UnityEngine.Object.FindFirstObjectByType<VoxelWorld>();
		}

		private void Update()
		{
			if (cam == null) cam = Camera.main;
			if (cam == null || world == null) return;
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit, 100f, terrainMask))
			{
				if (Input.GetMouseButtonDown(0))
				{
					world.DigAt(hit.point, radius);
					Debug.DrawRay(hit.point, Vector3.up, Color.red, 0.5f);
				}
				if (Input.GetMouseButtonDown(1))
				{
					world.PlaceAt(hit.point, radius);
					Debug.DrawRay(hit.point, Vector3.up, Color.green, 0.5f);
				}
			}
		}
	}
}
