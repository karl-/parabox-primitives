// #define PARABOX_DEBUG

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Parabox.InteractivePrimitives
{	
	/**
	 * InteractivePrimitive is the base class from which any interactive primitive must inherit.  It provides some template
	 * functionality (FreezeTransform) and dictates abstract methods for mesh generation.  It is up to the shape class to 
	 * determine how geometry should be built.
	 */
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public abstract class InteractivePrimitive : MonoBehaviour
	{
		
		/**
		 * Shape member variables exposed in the editor.
		 */
		// public Vector3 size = Vector3.one;
		public Vector3 LocalScale = Vector3.one;
		public Vector3 baseStart, baseEnd;
		public float height;

		/**
		 * Init is responsible for calling ToMesh when reassigning shape properties.
		 */
		public virtual void Init()
		{
			ToMesh();
		}

		/**
		 * ToMesh is responsible for assigning a mesh and material 
		 */
		public abstract void ToMesh();

		/**
		 * This runs when the bounds have changed - each Interactive is responsible for updating the mesh to match.
		 * @param baseStart The start of the base in world space.
		 * @param baseEnd The end of the base in world space.
		 * @param height The height of the bounds.
		 */
		public virtual void SetMeshDimensions(Vector3 baseStart, Vector3 baseEnd, float height)
		{
			this.baseStart = baseStart;
			this.baseEnd = baseEnd;
			this.height = height;

			transform.position = (baseStart + (baseEnd-baseStart)/2f) + Vector3.up * (height/2f);
			transform.localScale = new Vector3( baseEnd.x-baseStart.x, height, baseEnd.z-baseStart.z);
		}

		/**
		 * This is called after the drag sizing event is complete, and a finalized base start, end, and height hprivate Vector3 size = Vector3.one;ave been established.
		 */
		public virtual void OnFinishDragSizing()
		{
			FreezeTransform();
			#if UNITY_EDITOR
			// Unwrapping.GenerateSecondaryUVSet(GetComponent<MeshFilter>().sharedMesh);
			#endif
		}
		
		/**
		 * Gets a reference counted instance of the Default Diffuse material used by Unity's primitives.  Use this by default so
		 * that we don't have to manually create and manage a Material (which would otherwise leak on deletion).
		 */
		public Material DefaultDiffuse
		{
			get
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Material mat = go.GetComponent<Renderer>().sharedMaterial;
				DestroyImmediate(go);
				return mat;
			}
		}

		/**
		 * Some shapes (like Plane) do not require the height parameter to be set.  Overriding this property
		 * tells the shape editor that.
		 */
		public virtual bool baseOnly { get { return false; } }
		
		/**
		 * Some shapes allow the user to modify the subdivision parameter.  Override this if that is in fact
		 * the case.
		 */
		public virtual bool hasSubdivisions { get { return false; } }
		public virtual void SetSubdivisions(int subdiv) {}
		public virtual int GetSubdivisions() { return 0; }
		
		public virtual int MinSubdivisions { get { return 3; } }
		public virtual int MaxSubdivisions { get { return 64; } }

		/**
		 * Used to toggle the use of mouse to size the bounds
		 */
		public bool sizing = true;

		/**
		 * More accurately, "freeze scale", but you get the idea.
		 */
		internal void FreezeTransform()
		{
			Mesh msh = transform.GetComponent<MeshFilter>().sharedMesh;
			Vector3 scale = transform.localScale;
			
			scale.x = Mathf.Abs(scale.x);
			scale.y = Mathf.Abs(scale.y);
			scale.z = Mathf.Abs(scale.z);

			Vector3[] v = msh.vertices;
			for(int i = 0; i < v.Length; i++)
				v[i] = Vector3.Scale(v[i], scale);
			msh.vertices = v;
			
			LocalScale = Vector3.one;

			msh.RecalculateNormals();
			msh.Optimize();
			
			transform.localScale = Vector3.one;
		}

		/**
		 * Clean up leaky meshes.  Don't have to worry the material cause we're using Unity's default diffuse.
		 */
		void OnDestroy()
		{
			DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
		}

#if PARABOX_DEBUG
		/**
		 * Draw the primitive's normals.  Handy for debugging.
		 */
		public void OnDrawGizmos()
		{
			Mesh m = GetComponent<MeshFilter>().sharedMesh;

			if(m == null) return;

			Vector3[] v = m.vertices;
			Vector3[] n = m.normals;
			for(int i = 0; i < v.Length; i++)
			{
				Vector3 ver = transform.TransformPoint(v[i]);
				Vector3 nrm = transform.TransformDirection(n[i]);

				Gizmos.DrawLine(ver, ver+nrm*1f);
			}
		}
#endif
	}
}
