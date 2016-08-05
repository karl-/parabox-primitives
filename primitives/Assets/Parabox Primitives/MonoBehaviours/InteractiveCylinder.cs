using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Parabox.InteractivePrimitives
{
	public class InteractiveCylinder : InteractivePrimitive
	{
		public override bool hasSubdivisions { get { return true; } }

		public int m_subdivisions = 12;

		public override void SetSubdivisions(int subdiv)
		{
			m_subdivisions = (int)Mathf.Clamp(subdiv, 3, 64);
		}

		public override int GetSubdivisions()
		{
			return m_subdivisions;
		}

		public override void ToMesh()
		{
			Mesh m = new Mesh();
			int axisDivisions = m_subdivisions;
			
			m.name = "Cylinder";
			float radius = .5f;

			Vector3[] circle = new Vector3[axisDivisions];

			for(int i = 0; i < axisDivisions; i++)
			{
				float theta = ((360f/axisDivisions) * i) * Mathf.Deg2Rad;

				float x = Mathf.Cos(theta) * radius;
				float z = Mathf.Sin(theta) * radius;

				circle[i] = new Vector3(x, 0f, z);
			}

			// Wind body
			axisDivisions++;	// acct for uv seam

			int vertexCount = (axisDivisions*2)+2+(axisDivisions-1)*2;
			Vector3[] v = new Vector3[vertexCount];

			int n = 0;
			for(int i = 0; i < axisDivisions; i++)
			{
				v[n++] = circle[i%(axisDivisions-1)] - Vector3.up*.5f;
				v[n++] = circle[i%(axisDivisions-1)] + Vector3.up*.5f;
			}
			
			// Top, bottom
			for(int i = 0; i < axisDivisions-1; i++)
			{
				v[n+axisDivisions-1] = circle[i] - Vector3.up*.5f;
				v[n++] = circle[i] + Vector3.up*.5f;
			}

			v[vertexCount-2] = -Vector3.up*.5f;
			v[vertexCount-1] =  Vector3.up*.5f;

			// Wind tris (body)
			int[] tris = new int[axisDivisions*6 + (axisDivisions*2)*3];
			n = 0;
			int len = (axisDivisions*2)-1;
			for(int i = 0; i < len; i+=2)
			{				
				tris[n+0] = i+0;
				tris[n+1] = i+1;
				tris[n+2] = i+2;

				tris[n+3] = i+1;
				tris[n+4] = i+3;
				tris[n+5] = i+2;

				n += 6;
			}

			// wind caps
			n = axisDivisions*6;
			len = axisDivisions*2;
			axisDivisions--;
			for(int i = 0; i < axisDivisions; i++)
			{
				tris[n+0] = i+len+0;
				tris[n+1] = vertexCount-1;
				tris[n+2] = i >= axisDivisions-1 ? len : i+len+1;

				tris[n+(axisDivisions*3)+0] = i >= axisDivisions-1 ? len+axisDivisions : i+len+axisDivisions+1;
				tris[n+(axisDivisions*3)+1] = vertexCount-2;
				tris[n+(axisDivisions*3)+2] = i+len+axisDivisions+0;

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

			m.RecalculateNormals();

			m.Optimize();

			GetComponent<MeshFilter>().sharedMesh = m;

			if(GetComponent<MeshRenderer>().sharedMaterial == null)
				GetComponent<MeshRenderer>().sharedMaterial = DefaultDiffuse;
		}

		public override void OnFinishDragSizing()
		{
			// base.OnFinishDragSizing();
			FreezeTransform();

			// Set UVs
			Mesh msh = GetComponent<MeshFilter>().sharedMesh;
			Vector3[] verts = msh.vertices;
			int vertexCount = verts.Length;
			Vector2[] uvs = new Vector2[vertexCount];
			float high = Vector3.Distance(verts[0], verts[1]);
			float step = 0f;

			int n = 0;
			for(int i = 0; i < m_subdivisions+1; i++)
			{	

				uvs[n+0] = new Vector2(step, 0f);
				uvs[n+1] = new Vector2(step, high);
				
				step += Vector3.Distance(verts[n], verts[n+2]);

				n+=2;
			}

			// this is a little lazy :/
			for(int i = (m_subdivisions+1)*2; i < vertexCount; i++)
			{
				uvs[i] = new Vector2(verts[i].x, verts[i].z);
			}

			// ..and fix the normal at the UV seam
			Vector3[] nrm = msh.normals;
			Vector3 avg = (nrm[0] + nrm[m_subdivisions*2]) /2f;

			nrm[0] = avg;
			nrm[1] = avg;
			nrm[m_subdivisions*2] = avg;
			nrm[m_subdivisions*2+1] = avg;
			
			msh.normals = nrm;
			msh.uv = uvs;
		}
	}
}
