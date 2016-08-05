using UnityEngine;
using UnityEditor;
using System.Collections;
using Parabox.InteractivePrimitives;

/**
 * This class is responsible for handling the initial drag input for primitive creation.  Each shape should have an object Editor class that inherits this.
 */
[CustomEditor(typeof(InteractivePrimitive))]
public class InteractivePrimitiveEditor : Editor
{
#region Members
	
	InteractivePrimitive t;
	DragState dragState = DragState.None;
	Vector3 baseStart, baseEnd;
	float height;
#endregion

#region Init / Disable

	void OnEnable()
	{
		t = ((InteractivePrimitive)target);
		if(!t.sizing) return;
		t.GetComponent<Renderer>().enabled = false;
		dragState = DragState.None;

		Tools.current = Tool.Move;
	}
#endregion

	public override void OnInspectorGUI()
	{
		InteractivePrimitive primitive = ((InteractivePrimitive)target);
		if(primitive.hasSubdivisions)
		{
			EditorGUI.BeginChangeCheck();
			
			int subs = primitive.GetSubdivisions();

			subs = EditorGUILayout.IntField("Subdivisions", subs);

			if(EditorGUI.EndChangeCheck())
			{
				primitive.SetSubdivisions( (int) Mathf.Clamp(subs, primitive.MinSubdivisions, primitive.MaxSubdivisions) );
				primitive.SetMeshDimensions(primitive.baseStart, primitive.baseEnd, primitive.height);
				primitive.ToMesh();
				primitive.OnFinishDragSizing();
			}
		}
	}

#region OnSceneGUI

	Plane plane;
	public void OnSceneGUI()
	{
		// For the pretty screenshots
		// DrawBounds();
		// EditorUtility.SetSelectedWireframeHidden(t.renderer, true);

		if(!t.sizing)
			return;

		if(dragState != DragState.None)
			DrawBounds();

		// Force the control id focus
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlID);

		Event e = Event.current;

		if(e.modifiers != (EventModifiers)0 || Tools.current == Tool.View)
			return;

		switch(e.type)
		{
			case EventType.MouseDown:
				Vector3 pivot = SceneView.lastActiveSceneView.pivot;
				plane = new Plane(Vector3.up, pivot);
				/**
				 * First contact - set the base to the mouse intersect
				 */
				if(dragState == DragState.None)
				{
					Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
					if(!InteractivePrimitivesMath.MousePositionOnPlane(ray, plane, ref baseStart))
					{
						Debug.LogWarning("Could not find origin plane.");
						t.sizing = false;
						t.OnFinishDragSizing();
					}

					baseEnd = baseStart;
					height = .01f;

					((InteractivePrimitive)target).GetComponent<Renderer>().enabled = true;
					dragState = DragState.Base;
				}

				break;

			case EventType.MouseDrag:
				if(dragState == DragState.Base)
				{
					Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
					InteractivePrimitivesMath.MousePositionOnPlane(ray, plane, ref baseEnd);
				}
				break;

			case EventType.MouseUp:
			case EventType.Ignore:
				{	
					if(dragState == DragState.Base)
					{
						if(t.baseOnly)
						{
							t.sizing = false;
							t.OnFinishDragSizing();
							return;
						}
						else
						{
							dragState = DragState.Height;
						}
					}
					else
					{
						if(dragState == DragState.Height)
						{
							// Complete
							((InteractivePrimitive)t).sizing = false;
							((InteractivePrimitive)t).OnFinishDragSizing();
							return;
						}
					}
				}
				break;
		}

		switch(dragState)
		{
			case DragState.None:
				return;

			case DragState.Base:
				break;

			case DragState.Height:

				Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
				Vector3 closestPointLine1, closestPointLine2;

				if( InteractivePrimitivesMath.ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, ray.GetPoint(0f), ray.direction, baseEnd, Vector3.up) )
					height = Vector3.Distance(baseEnd, closestPointLine2);

				break;
		}

		t.SetMeshDimensions(baseStart, baseEnd, height);
	}
#endregion

	/**
	 * If you want to do something fancier than this, go ahead and override it :)
	 */
	public virtual void DrawBounds()
	{
		// Draw Wireframe
		Bounds b = t.GetComponent<Renderer>().bounds;

		Vector3 cen = b.center;
		Vector3 ext = b.extents;

		DrawBoundsEdge(cen, -ext.x, -ext.y, -ext.z, HandleUtility.GetHandleSize(cen) * .3f);
		DrawBoundsEdge(cen, -ext.x, -ext.y,  ext.z, HandleUtility.GetHandleSize(cen) * .3f);
		DrawBoundsEdge(cen,  ext.x, -ext.y, -ext.z, HandleUtility.GetHandleSize(cen) * .3f);
		DrawBoundsEdge(cen,  ext.x, -ext.y,  ext.z, HandleUtility.GetHandleSize(cen) * .3f);

		DrawBoundsEdge(cen, -ext.x,  ext.y, -ext.z, HandleUtility.GetHandleSize(cen) * .3f);
		DrawBoundsEdge(cen, -ext.x,  ext.y,  ext.z, HandleUtility.GetHandleSize(cen) * .3f);
		DrawBoundsEdge(cen,  ext.x,  ext.y, -ext.z, HandleUtility.GetHandleSize(cen) * .3f);
		DrawBoundsEdge(cen,  ext.x,  ext.y,  ext.z, HandleUtility.GetHandleSize(cen) * .3f);
	}

	private void DrawBoundsEdge(Vector3 center, float x, float y, float z, float size)
	{
		Vector3 p = center;
		p.x += x;
		p.y += y;
		p.z += z;
		Handles.DrawLine(p, p + ( -(x/Mathf.Abs(x)) * Vector3.right 	* Mathf.Min(size, Mathf.Abs(x))));
		Handles.DrawLine(p, p + ( -(y/Mathf.Abs(y)) * Vector3.up 		* Mathf.Min(size, Mathf.Abs(y))));
		Handles.DrawLine(p, p + ( -(z/Mathf.Abs(z)) * Vector3.forward 	* Mathf.Min(size, Mathf.Abs(z))));
	}
}
