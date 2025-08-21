using UnityEngine;

namespace LowPolySurvival.Game.Core.Voxel
{
	public sealed class VoxelWorld : MonoBehaviour
	{
		[Header("World Settings")]
		[SerializeField] private Vector3Int chunkDimensions = new Vector3Int(16, 128, 16);
		[SerializeField] private int viewDistanceChunks = 6;

		public Vector3Int ChunkDimensions => chunkDimensions;
		public int ViewDistanceChunks => viewDistanceChunks;

		private void Awake()
		{
			InitializeWorldIfNeeded();
		}

		private void InitializeWorldIfNeeded()
		{
			// TODO: Load or generate initial chunk set asynchronously
		}

		public void DigAt(Vector3 worldPosition, float radius)
		{
			// TODO: Modify voxel data and schedule mesh rebuild for affected chunks
		}

		public void PlaceAt(Vector3 worldPosition, float radius)
		{
			// TODO: Modify voxel data and schedule mesh rebuild for affected chunks
		}
	}
}
