using UnityEngine;

namespace LowPolySurvival.Game.Core.Voxel
{
	public sealed class VoxelWorld : MonoBehaviour
	{
		[Header("World Settings")]
		[SerializeField] private Vector3Int chunkDimensions = new Vector3Int(16, 128, 16);
		[SerializeField] private int viewDistanceChunks = 6;
        [SerializeField] private float voxelSize = 1f;
        [SerializeField] private bool generateOnPlay = true;
        private VoxelChunk chunk;

		public Vector3Int ChunkDimensions => chunkDimensions;
		public int ViewDistanceChunks => viewDistanceChunks;

		private void Awake()
		{
			if (generateOnPlay)
			{
				BuildDemoChunkIfEmpty();
			}
		}

		public void BuildDemoChunkIfEmpty()
		{
			// For demo, create one chunk and flat-fill
			if (chunk == null)
			{
				var go = new GameObject("Chunk_0_0");
				go.transform.SetParent(transform);
				go.transform.position = Vector3.zero;
				chunk = go.AddComponent<VoxelChunk>();
				chunk.Initialize(new Vector3Int(Mathf.Max(4, chunkDimensions.x), Mathf.Max(8, chunkDimensions.y/4), Mathf.Max(4, chunkDimensions.z)), voxelSize);
				// Make a shallow platform near ground for visibility
				chunk.GenerateFlatFill(1);
				chunk.RebuildMesh();
			}
		}

		public void DigAt(Vector3 worldPosition, float radius)
		{
			if (chunk == null) return;
			var dims = new Vector3Int(chunkDimensions.x, Mathf.Max(8, chunkDimensions.y/2), chunkDimensions.z);
			Vector3 local = worldPosition - chunk.transform.position;
			int rx = Mathf.FloorToInt(local.x / voxelSize);
			int ry = Mathf.FloorToInt(local.y / voxelSize);
			int rz = Mathf.FloorToInt(local.z / voxelSize);
			int r = Mathf.CeilToInt(radius / voxelSize);
			for (int x=rx-r;x<=rx+r;x++)
			for (int y=ry-r;y<=ry+r;y++)
			for (int z=rz-r;z<=rz+r;z++)
			{
				if (!chunk.InBounds(x,y,z)) continue;
				var p = new Vector3((x+0.5f)*voxelSize,(y+0.5f)*voxelSize,(z+0.5f)*voxelSize);
				if ((p - local).sqrMagnitude <= radius*radius) chunk.SetSolid(x,y,z,false);
			}
			chunk.RebuildMesh();
		}

		public void PlaceAt(Vector3 worldPosition, float radius)
		{
			if (chunk == null) return;
			Vector3 local = worldPosition - chunk.transform.position;
			int rx = Mathf.FloorToInt(local.x / voxelSize);
			int ry = Mathf.FloorToInt(local.y / voxelSize);
			int rz = Mathf.FloorToInt(local.z / voxelSize);
			int r = Mathf.CeilToInt(radius / voxelSize);
			for (int x=rx-r;x<=rx+r;x++)
			for (int y=ry-r;y<=ry+r;y++)
			for (int z=rz-r;z<=rz+r;z++)
			{
				if (!chunk.InBounds(x,y,z)) continue;
				var p = new Vector3((x+0.5f)*voxelSize,(y+0.5f)*voxelSize,(z+0.5f)*voxelSize);
				if ((p - local).sqrMagnitude <= radius*radius) chunk.SetSolid(x,y,z,true);
			}
			chunk.RebuildMesh();
		}

		public VoxelChunk GetChunk() => chunk;
	}
}
