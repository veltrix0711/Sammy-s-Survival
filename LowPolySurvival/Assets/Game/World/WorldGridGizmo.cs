using UnityEngine;

namespace LowPolySurvival.Game.World
{
	[ExecuteAlways]
	public sealed class WorldGridGizmo : MonoBehaviour
	{
		[SerializeField] private float cellSize = 10f;
		[SerializeField] private int halfCells = 20;
		[SerializeField] private Color color = new Color(0f, 1f, 1f, 0.3f);

		private void OnDrawGizmos()
		{
			Gizmos.color = color;
			for (int x = -halfCells; x <= halfCells; x++)
			{
				Vector3 a = new Vector3(x * cellSize, 0, -halfCells * cellSize);
				Vector3 b = new Vector3(x * cellSize, 0, halfCells * cellSize);
				Gizmos.DrawLine(a, b);
			}
			for (int z = -halfCells; z <= halfCells; z++)
			{
				Vector3 a = new Vector3(-halfCells * cellSize, 0, z * cellSize);
				Vector3 b = new Vector3(halfCells * cellSize, 0, z * cellSize);
				Gizmos.DrawLine(a, b);
			}
		}
	}
}


