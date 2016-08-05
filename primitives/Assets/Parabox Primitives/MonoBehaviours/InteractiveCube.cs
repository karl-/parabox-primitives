using UnityEngine;
using System.Collections;

namespace Parabox.InteractivePrimitives
{
	public class InteractiveCube : InteractivePrimitive
	{
		public override void ToMesh()
		{
			Mesh m = new Mesh();

			m.name = "Cube";

			m.vertices = new Vector3[]
			{
				new Vector3(-.5f, -.5f, -.5f),
				new Vector3( .5f, -.5f, -.5f),
				new Vector3(-.5f,  .5f, -.5f),
				new Vector3( .5f,  .5f, -.5f),

				new Vector3( .5f, -.5f, -.5f),
				new Vector3( .5f, -.5f,  .5f),
				new Vector3( .5f,  .5f, -.5f),
				new Vector3( .5f,  .5f,  .5f),

				new Vector3( .5f, -.5f,  .5f),
				new Vector3(-.5f, -.5f,  .5f),
				new Vector3( .5f,  .5f,  .5f),
				new Vector3(-.5f,  .5f,  .5f),

				new Vector3(-.5f, -.5f,  .5f),
				new Vector3(-.5f, -.5f, -.5f),
				new Vector3(-.5f,  .5f,  .5f),
				new Vector3(-.5f,  .5f, -.5f),

				new Vector3(-.5f, -.5f, -.5f),
				new Vector3( .5f, -.5f, -.5f),
				new Vector3(-.5f, -.5f,  .5f),
				new Vector3( .5f, -.5f,  .5f),

				new Vector3(-.5f,  .5f, -.5f),
				new Vector3( .5f,  .5f, -.5f),
				new Vector3(-.5f,  .5f,  .5f),
				new Vector3( .5f,  .5f,  .5f),

			};

			m.triangles = new int[]
			{
				2, 1, 0,
				2, 3, 1,

				6, 5, 4,
				6, 7, 5,

				10, 9, 8,
				10, 11 ,9,

				14, 13, 12,
				14, 15, 13,

				16, 17, 18,
				17, 19, 18,

				22, 21, 20,
				22, 23, 21
			};

			m.uv = new Vector2[m.vertices.Length];

			m.RecalculateNormals();
			m.Optimize();

			GetComponent<MeshFilter>().sharedMesh = m;
			GetComponent<MeshRenderer>().sharedMaterial = DefaultDiffuse;
		}

		public override void OnFinishDragSizing()
		{
			float 	w = Mathf.Abs(transform.localScale.x),
					h = Mathf.Abs(transform.localScale.y),
					d = Mathf.Abs(transform.localScale.z);
			
			base.OnFinishDragSizing();

			GetComponent<MeshFilter>().sharedMesh.uv = new Vector2[]
			{
				new Vector2(-.5f*w, -.5f*h),
				new Vector2( .5f*w, -.5f*h),	// front
				new Vector2(-.5f*w,  .5f*h),
				new Vector2( .5f*w,  .5f*h),

				new Vector2(-.5f*d, -.5f*h),	// right
				new Vector2( .5f*d, -.5f*h),
				new Vector2(-.5f*d,  .5f*h),
				new Vector2( .5f*d,  .5f*h),
				
				new Vector2(-.5f*w, -.5f*h),	// back
				new Vector2( .5f*w, -.5f*h),
				new Vector2(-.5f*w,  .5f*h),
				new Vector2( .5f*w,  .5f*h),
				
				new Vector2(-.5f*d, -.5f*h),	// left
				new Vector2( .5f*d, -.5f*h),
				new Vector2(-.5f*d,  .5f*h),
				new Vector2( .5f*d,  .5f*h),
				
				new Vector2( .5f*w, -.5f*d),
				new Vector2(-.5f*w, -.5f*d),	// bottom
				new Vector2( .5f*w,  .5f*d),	
				new Vector2(-.5f*w,  .5f*d),

				new Vector2(-.5f*w, -.5f*d),	// top
				new Vector2( .5f*w, -.5f*d),
				new Vector2(-.5f*w,  .5f*d),
				new Vector2( .5f*w,  .5f*d),
			};
		}

	}
}
