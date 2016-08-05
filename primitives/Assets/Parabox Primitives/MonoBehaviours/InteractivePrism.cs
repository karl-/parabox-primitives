using UnityEngine;
using System.Collections;

namespace Parabox.InteractivePrimitives
{
	public class InteractivePrism : InteractivePrimitive
	{
		public override void ToMesh()
		{
			Mesh m = new Mesh();
			m.vertices = new Vector3[18]
			{
				new Vector3(-.5f, -.5f, -.5f),	// Bottom
				new Vector3( .5f, -.5f, -.5f),
				new Vector3(-.5f, -.5f,  .5f),
				new Vector3( .5f, -.5f,  .5f),

				new Vector3( .5f, -.5f, -.5f),	// Right
				new Vector3( .5f, -.5f,  .5f),
				new Vector3(  0f,  .5f, -.5f),
				new Vector3(  0f,  .5f,  .5f),

				new Vector3(  0f,  .5f, -.5f), 	// Left
				new Vector3(  0f,  .5f,  .5f),
				new Vector3(-.5f, -.5f, -.5f),
				new Vector3(-.5f, -.5f,  .5f),

				new Vector3(-.5f, -.5f, -.5f),	// Front
				new Vector3( .5f, -.5f, -.5f),
				new Vector3(  0f,  .5f, -.5f),
				
				new Vector3(-.5f, -.5f,  .5f),	// Back
				new Vector3( .5f, -.5f,  .5f),
				new Vector3(  0f,  .5f,  .5f),
			};

			m.uv = new Vector2[18];

			m.triangles = new int[24]
			{
				0, 1, 2, 1, 3, 2,
				6, 5, 4, 6, 7, 5,
				10, 9, 8, 10, 11, 9,
				14, 13, 12,
				15, 16, 17
			};

			m.RecalculateNormals();
			GetComponent<MeshFilter>().sharedMesh = m;
			GetComponent<MeshRenderer>().sharedMaterial = DefaultDiffuse;
		}

		public override void OnFinishDragSizing()
		{
			float 	w = transform.localScale.x,
					h = transform.localScale.y,
					d = transform.localScale.z,
					hyp = 1.414f;

			base.OnFinishDragSizing();

			// Mesh m = GetComponent<MeshFilter>().sharedMesh;

			Vector2[] uvs = new Vector2[]
			{
				new Vector2( .5f*w, -.5f*d),
				new Vector2(-.5f*w, -.5f*d),
				new Vector2( .5f*w,  .5f*d),
				new Vector2(-.5f*w,  .5f*d),

				new Vector2(hyp*h, -.5f*d),
				new Vector2(hyp*h, 	.5f*d),
				new Vector2(0f,    -.5f*d),
				new Vector2(0f,  	.5f*d),

				new Vector2( 0f, 	-.5f*d),
				new Vector2( 0f, 	 .5f*d),
				new Vector2(-hyp*h, -.5f*d),
				new Vector2(-hyp*h,  .5f*d),

				new Vector2(-.5f*w, -.5f*h),
				new Vector2( .5f*w, -.5f*h),
				new Vector2(  0f, 	 .5f*h),
				
				new Vector2( .5f*w, -.5f*h),
				new Vector2(-.5f*w, -.5f*h),
				new Vector2(  0f, 	 .5f*h),
			};
			GetComponent<MeshFilter>().sharedMesh.uv = uvs;
		}
	}
}
