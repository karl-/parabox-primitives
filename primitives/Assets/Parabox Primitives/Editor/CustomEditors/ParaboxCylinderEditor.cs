using UnityEngine;
using UnityEditor;
using System.Collections;
using Parabox.InteractivePrimitives;

[CustomEditor(typeof(InteractiveCylinder))]
public class ParaboxCylinderEditor : InteractivePrimitiveEditor
{

#region Menu

	[MenuItem("GameObject/Create Other/Parabox Cylinder", false, 400)]
	public static void MenuCreateCylinder()
	{
		GameObject go = new GameObject();
		go.AddComponent<InteractiveCylinder>().Init();
		Selection.objects = new Object[1] { go };
	}

#endregion
}