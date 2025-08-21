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
		[SerializeField] private MeshCollider meshCollider;
		[SerializeField] private Material material;

		private bool[,,] solid;

		public void Initialize(Vector3Int dims, float size)
		{
			dimensions = dims; voxelSize = size;
			solid = new bool[dims.x, dims.y, dims.z];
			if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
			if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
			if (material == null)
			{
				var shader = Shader.Find("Universal Render Pipeline/Lit");
				if (shader == null) shader = Shader.Find("Standard");
				material = new Material(shader);
				material.SetColor("_BaseColor", new Color(0.6f, 0.8f, 0.6f, 1f));
				var tex = CreateCheckerTexture(32);
				material.SetTexture("_BaseMap", tex);
				material.SetTextureScale("_BaseMap", new Vector2(4,4));
			}
			meshRenderer.sharedMaterial = material;
			gameObject.layer = LayerMask.NameToLayer("Default");
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
			for (int x=0;x<dimensions.x;x++)
			for (int y=0;y<dimensions.y;y++)
			for (int z=0;z<dimensions.z;z++)
			{
				if (!solid[x,y,z]) continue;
				Vector3 basePos = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
				// +Y (top)
				if (y+1>=dimensions.y || !solid[x,y+1,z]) AddQuad(verts, tris, norms, uvs, basePos + new Vector3(0, voxelSize, 0),
					new Vector3(voxelSize,0,0), new Vector3(0,0,voxelSize), Vector3.up);
				// -Y (bottom)
				if (y-1<0 || !solid[x,y-1,z]) AddQuad(verts, tris, norms, uvs, basePos,
					new Vector3(0,0,voxelSize), new Vector3(voxelSize,0,0), Vector3.down);
				// +X (right)
				if (x+1>=dimensions.x || !solid[x+1,y,z]) AddQuad(verts, tris, norms, uvs, basePos + new Vector3(voxelSize,0,0),
					new Vector3(0,0,voxelSize), new Vector3(0,voxelSize,0), Vector3.right);
				// -X (left)
				if (x-1<0 || !solid[x-1,y,z]) AddQuad(verts, tris, norms, uvs, basePos,
					new Vector3(0,voxelSize,0), new Vector3(0,0,voxelSize), Vector3.left);
				// +Z (forward)
				if (z+1>=dimensions.z || !solid[x,y,z+1]) AddQuad(verts, tris, norms, uvs, basePos + new Vector3(0,0,voxelSize),
					new Vector3(voxelSize,0,0), new Vector3(0,voxelSize,0), Vector3.forward);
				// -Z (back)
				if (z-1<0 || !solid[x,y,z-1]) AddQuad(verts, tris, norms, uvs, basePos,
					new Vector3(0,0,voxelSize), new Vector3(voxelSize,0,0), Vector3.back);
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
			if (meshCollider != null) meshCollider.sharedMesh = null; // force refresh
			if (meshCollider != null) meshCollider.sharedMesh = mesh;
			Debug.Log($"VoxelChunk rebuilt: verts={verts.Count} tris={tris.Count/3} mat={(meshRenderer!=null && meshRenderer.sharedMaterial!=null)}");
		}

		private Texture2D CreateCheckerTexture(int size)
		{
			var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
			tex.wrapMode = TextureWrapMode.Repeat;
			tex.filterMode = FilterMode.Point;
			for (int y=0;y<size;y++)
			{
				for (int x=0;x<size;x++)
				{
					bool c = ((x/4)+(y/4)) % 2 == 0;
					tex.SetPixel(x,y, c ? new Color(0.55f,0.75f,0.55f,1f) : new Color(0.45f,0.65f,0.45f,1f));
				}
			}
			tex.Apply();
			return tex;
		}

		private static void AddQuad(List<Vector3> v, List<int> t, List<Vector3> n, List<Vector2> u,
			Vector3 origin, Vector3 axisU, Vector3 axisV, Vector3 normal)
		{
			int vi = v.Count;
			v.Add(origin);
			v.Add(origin + axisU);
			v.Add(origin + axisU + axisV);
			v.Add(origin + axisV);
			n.AddRange(new[]{normal,normal,normal,normal});
			u.AddRange(new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(1,1),new Vector2(0,1)});
			if (Vector3.Dot(Vector3.Cross(axisU, axisV), normal) >= 0)
			{
				t.Add(vi+0); t.Add(vi+2); t.Add(vi+1);
				t.Add(vi+0); t.Add(vi+3); t.Add(vi+2);
			}
			else
			{
				t.Add(vi+0); t.Add(vi+1); t.Add(vi+2);
				t.Add(vi+0); t.Add(vi+2); t.Add(vi+3);
			}
		}
	}
}
