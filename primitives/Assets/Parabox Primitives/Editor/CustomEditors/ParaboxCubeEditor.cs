using UnityEngine;
using UnityEditor;
using System.Collections;
using Parabox.InteractivePrimitives;

[CustomEditor(typeof(InteractiveCube))]
public class ParaboxCubeEditor : InteractivePrimitiveEditor
{

#region Menu

	[MenuItem("GameObject/Create Other/Parabox Cube &#c", false, 400)]
	public static void MenuCreateCube()
	{
		GameObject go = new GameObject();
		go.AddComponent<InteractiveCube>().Init();
		Selection.objects = new Object[1] { go };
	}
#endregion

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}