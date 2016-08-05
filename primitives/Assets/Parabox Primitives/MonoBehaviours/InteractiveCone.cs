using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Parabox.InteractivePrimitives
{
	public class InteractiveCone : InteractivePrimitive
	{
		public override bool hasSubdivisions { get { return true; } }

		int m_subdivisions = 24;

		public override void SetSubdivisions(int subdiv)
		{
			m_subdivisions = subdiv;
		}

		public override int GetSubdivisions()
		{
			return m_subdivisions;
		}
		
		public override void ToMesh()
		{
			int axisDivisions = m_subdivisions;

			Mesh m = new Mesh();

			m.name = "Cone";
			float radius = .5f;

			axisDivisions++;
			
			Vector3[] circle = new Vector3[axisDivisions];

			for (int i = 0; i < axisDivisions-1; i++)
			{
				float theta = ((360f / (axisDivisions-1)) * i) * Mathf.Deg2Rad;

				float x = Mathf.Cos(theta) * radius;
				float z = Mathf.Sin(theta) * radius;

				circle[i] = new Vector3(x, -.5f, z);
			}

			circle[axisDivisions-1] = circle[0];

			int vertexCount = (axisDivisions * 2) + 2;

			// verts
			Vector3[] v = new Vector3[vertexCount];
			// Vector3[] nrm = new Vector3[vertexCount];

			for (int i = 0; i < axisDivisions; i++)
			{
				v[i] = circle[i];					// bottom for sides
				v[i+axisDivisions] = circle[i];	// bottom for -v.up
			}

			// circle point
			v[vertexCount - 2] = Vector3.up * -.5f;

			// cone point
			v[vertexCount - 1] = Vector3.up * .5f;

			int[] tris = new int[ (axisDivisions * 3) * 2];

			int n = 0;
			int step = axisDivisions * 3;
			for (int i = 0; i < axisDivisions-1; i++)
			{
				// cone sides
				tris[n+0] = i;
				tris[n+1] = vertexCount - 1;
				tris[n+2] = i + 1;

				// // bottom circle
				tris[n+step+0] = i + axisDivisions + 1;
				tris[n+step+1] = vertexCount - 2;
				tris[n+step+2] = i+ axisDivisions;

				n += 3;
			}

			//reassign scale
			LocalScale.x = Mathf.Abs(LocalScale.x);
			LocalScale.y = Mathf.Abs(LocalScale.y);
			LocalScale.z = Mathf.Abs(LocalScale.z);
			
			for(int i = 0; i < v.Length; i++)
				v[i] = Vector3.Scale(v[i], LocalScale);
			

			m.vertices = v;
			m.triangles = tris;
			m.uv = new Vector2[m.vertices.Length];

			GetComponent<MeshFilter>().sharedMesh = m;

			if( GetComponent<MeshRenderer>().sharedMaterial == null )
				GetComponent<MeshRenderer>().sharedMaterial = DefaultDiffuse;

			m.RecalculateNormals();
		}

		public override void OnFinishDragSizing()
		{
			// base.OnFinishDragSizing();	// need to build UVs before assigning the UV2 channel
			FreezeTransform();

			// calc uvs
			Mesh m = GetComponent<MeshFilter>().sharedMesh;
			
			Vector2[] uvs = m.uv;
			Vector3[] v = m.vertices;
			int vertexCount = m.vertexCount;

			uvs[vertexCount-1] = Vector2.zero;
			uvs[vertexCount-2] = Vector2.zero;
			
			for(int i = 0; i < m.vertices.Length; i++)
				uvs[i] = new Vector2(v[i].x, v[i].z);

			m.uv = uvs;
		}
	}
}

