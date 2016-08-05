using UnityEngine;
using System.Collections;

public class DynamicCube : MonoBehaviour
{
	Plane ground = new Plane(Vector3.up, Vector3.zero);
	GameObject preview;

	void Start()
	{
		preview = GameObject.CreatePrimitive(PrimitiveType.Cube);
		preview.GetComponent<Renderer>().enabled = false;
	}

	enum DragState
	{
		None,
		Base,
		Height
	}
	DragState dragState = DragState.None;

	Vector3 origin, baseEnd;
	float height = 0f;

	void OnGUI()
	{
		GUILayout.Label(dragState.ToString());
	}

	void Update()
	{
		// Handle mouse input
		if(Input.GetMouseButtonDown(0))
		{
			if(dragState != DragState.Height)
			{
				if(!MousePositionOnPlane(Input.mousePosition, ground, ref origin))
					return;

				baseEnd = origin;
				height = .1f;

				dragState = DragState.Base;
				preview.GetComponent<Renderer>().enabled = true;
			}
		}

		if(Input.GetMouseButtonUp(0) && dragState != DragState.None)
		{
			switch(dragState)
			{
				case DragState.Base:
					dragState = DragState.Height;
					break;

				case DragState.Height:
					dragState = DragState.None;
					preview.GetComponent<Renderer>().enabled = false;

					CopyPreviewMesh();

					return;
			}
		}

		switch(dragState)
		{
			case DragState.None:
				preview.GetComponent<Renderer>().enabled = false;
				return;

			case DragState.Base:
				MousePositionOnPlane(Input.mousePosition, ground, ref baseEnd );
				break;

			case DragState.Height:

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 closestPointLine1, closestPointLine2;

				if( ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, ray.GetPoint(0f), ray.direction, baseEnd, Vector3.up) )
				{
					height = Vector3.Distance(baseEnd, closestPointLine2);
				}
				break;
		}

		UpdatePreviewMesh(origin, baseEnd, height);
	}

	void UpdatePreviewMesh(Vector3 origin, Vector3 end, float height)
	{
		preview.transform.position = (origin + (baseEnd-origin)/2f) + Vector3.up * (height/2f);
		preview.transform.localScale = new Vector3( end.x-origin.x, height, end.z-origin.z);
	}

	void CopyPreviewMesh()
	{
		Mesh pm = preview.GetComponent<MeshFilter>().sharedMesh;
		Vector3 scale = preview.transform.localScale;
		scale.x = Mathf.Abs(scale.x);
		scale.y = Mathf.Abs(scale.y);
		scale.z = Mathf.Abs(scale.z);

		Mesh m = new Mesh();
		m.name = "Cube";
		Vector3[] v = pm.vertices;
		for(int i = 0; i < v.Length; i++)
			v[i] = Vector3.Scale(v[i], scale);
		m.vertices = v;
		m.triangles = pm.triangles;
		m.uv = pm.uv;
		m.RecalculateNormals();
		m.Optimize();


		GameObject newGo = new GameObject();
		newGo.AddComponent<MeshFilter>().sharedMesh = m;
		newGo.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
		newGo.transform.position = preview.transform.position;
	}

	bool MousePositionOnPlane(Vector2 mpos, Plane plane, ref Vector3 pos)
	{
		float dist; 
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if( !ground.Raycast(ray, out dist ) )
			return false;

		pos = ray.GetPoint(dist);
		return true;
	}

	// http://wiki.unity3d.com/index.php?title=3d_Math_functions
	//Two non-parallel lines which may or may not touch each other have a point on each line which are closest
	//to each other. This function finds those two points. If the lines are not parallel, the function 
	//outputs true, otherwise false.
	public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
 
		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;
 
		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);
 
		float d = a*e - b*b;
 
		//lines are not parallel
		if(d != 0.0f){
 
			Vector3 r = linePoint1 - linePoint2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);
 
			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;
 
			closestPointLine1 = linePoint1 + lineVec1 * s;
			closestPointLine2 = linePoint2 + lineVec2 * t;
 
			return true;
		}
 
		else{
			return false;
		}
	}	
}
