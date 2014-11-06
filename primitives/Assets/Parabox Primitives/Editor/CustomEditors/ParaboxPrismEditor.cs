using UnityEngine;
using UnityEditor;
using System.Collections;
using Parabox.InteractivePrimitives;

[CustomEditor(typeof(InteractivePrism))]
public class ParaboxPrismEditor : InteractivePrimitiveEditor
{

#region Menu

	[MenuItem("GameObject/Create Other/Parabox Prism", false, 400)]
	public static void MenuCreateCube()
	{
		GameObject go = new GameObject();
		go.AddComponent<InteractivePrism>().Init();
		Selection.objects = new Object[1] { go };
	}
#endregion

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}