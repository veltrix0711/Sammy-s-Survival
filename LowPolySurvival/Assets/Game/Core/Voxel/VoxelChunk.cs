using UnityEngine;
using System.Collections.Generic;

namespace LowPolySurvival.Game.Core.Voxel
{
	public sealed class VoxelChunk : MonoBehaviour
	{
		[SerializeField] private Vector3Int dimensions = new Vector3Int(16, 64, 16);
		[SerializeField] private float voxelSize = 1f;
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshRenderer meshRenderer;

		private bool[,,] solid;

		public void Initialize(Vector3Int dims, float size)
		{
			dimensions = dims; voxelSize = size;
			solid = new bool[dims.x, dims.y, dims.z];
			if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}

		public void SetSolid(int x, int y, int z, bool value)
		{
			if (InBounds(x,y,z)) solid[x, y, z] = value;
		}

		public bool InBounds(int x, int y, int z)
		{
			return x >= 0 && y >= 0 && z >= 0 && x < dimensions.x && y < dimensions.y && z < dimensions.z;
		}

		public void GenerateFlatFill(int height)
		{
			for (int x=0;x<dimensions.x;x++)
			for (int z=0;z<dimensions.z;z++)
			for (int y=0;y<dimensions.y;y++)
				solid[x,y,z] = y < height;
		}

		public void RebuildMesh()
		{
			var verts = new List<Vector3>();
			var tris = new List<int>();
			var norms = new List<Vector3>();
			var uvs = new List<Vector2>();
			// Simple naive meshing: add top faces only for brevity
			for (int x=0;x<dimensions.x;x++)
			for (int y=0;y<dimensions.y;y++)
			for (int z=0;z<dimensions.z;z++)
			{
				if (!solid[x,y,z]) continue;
				// top face if air above
				if (y+1>=dimensions.y || !solid[x,y+1,z])
				{
					int vi = verts.Count;
					Vector3 basePos = new Vector3(x, y+1, z) * voxelSize;
					verts.Add(basePos + new Vector3(0,0,0));
					verts.Add(basePos + new Vector3(voxelSize,0,0));
					verts.Add(basePos + new Vector3(voxelSize,0,voxelSize));
					verts.Add(basePos + new Vector3(0,0,voxelSize));
					norms.AddRange(new[]{Vector3.up,Vector3.up,Vector3.up,Vector3.up});
					uvs.AddRange(new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(1,1),new Vector2(0,1)});
					tris.Add(vi+0); tris.Add(vi+2); tris.Add(vi+1);
					tris.Add(vi+0); tris.Add(vi+3); tris.Add(vi+2);
				}
			}
			var mesh = meshFilter.sharedMesh;
			if (mesh == null) mesh = new Mesh(); else mesh.Clear();
			mesh.indexFormat = verts.Count>65000?UnityEngine.Rendering.IndexFormat.UInt32:UnityEngine.Rendering.IndexFormat.UInt16;
			mesh.SetVertices(verts);
			mesh.SetNormals(norms);
			mesh.SetUVs(0, uvs);
			mesh.SetTriangles(tris, 0);
			mesh.RecalculateBounds();
			meshFilter.sharedMesh = mesh;
		}
	}
}
