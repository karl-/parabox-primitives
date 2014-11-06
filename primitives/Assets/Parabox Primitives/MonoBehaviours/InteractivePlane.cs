using UnityEngine;
using System.Collections;

namespace Parabox.InteractivePrimitives
{
	public class InteractivePlane : InteractivePrimitive
	{
		public override bool baseOnly { get { return true; } }

		public override void ToMesh()
		{
			Mesh m = new Mesh();
			m.vertices = new Vector3[4]
			{
				new Vector3(-.5f, 0f,  .5f),
				new Vector3( .5f, 0f,  .5f),
				new Vector3(-.5f, 0f, -.5f),
				new Vector3( .5f, 0f, -.5f)
			};

			m.uv = new Vector2[4]
			{
				Vector2.zero,
				Vector2.right,
				Vector2.up,
				Vector2.one
			};

			m.normals = new Vector3[4]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};

			m.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };

			GetComponent<MeshFilter>().sharedMesh = m;
			GetComponent<MeshRenderer>().sharedMaterial = DefaultDiffuse;
		}

		public override void SetMeshDimensions(Vector3 baseStart, Vector3 baseEnd, float height)
		{
			transform.position = (baseStart + (baseEnd-baseStart)/2f);
			transform.localScale = new Vector3( baseEnd.x-baseStart.x, 1f, baseEnd.z-baseStart.z);
		}

		public override void OnFinishDragSizing()
		{
			base.OnFinishDragSizing();
			
			Vector3[] v = GetComponent<MeshFilter>().sharedMesh.vertices;
			GetComponent<MeshFilter>().sharedMesh.uv = new Vector2[4]
			{
				new Vector2(v[0].x, v[0].z),
				new Vector2(v[1].x, v[1].z),
				new Vector2(v[2].x, v[2].z),
				new Vector2(v[3].x, v[3].z)
			};
		}
	}
}