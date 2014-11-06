using UnityEngine;
using UnityEditor;
using System.Collections;
using Parabox.InteractivePrimitives;

[CustomEditor(typeof(InteractivePlane))]
public class ParaboxPlaneEditor : InteractivePrimitiveEditor
{

#region Menu

	[MenuItem("GameObject/Create Other/Parabox Plane", false, 400)]
	public static void MenuCreatePlane()
	{
		GameObject go = new GameObject();
		go.AddComponent<InteractivePlane>().Init();
		Selection.objects = new Object[1] { go };
	}
#endregion

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}