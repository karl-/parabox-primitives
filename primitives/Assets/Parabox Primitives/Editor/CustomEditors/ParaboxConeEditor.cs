using UnityEngine;
using UnityEditor;
using System.Collections;
using Parabox.InteractivePrimitives;

[CustomEditor(typeof(InteractiveCone))]
public class ParaboxConeEditor : InteractivePrimitiveEditor
{

#region Menu

	[MenuItem("GameObject/Create Other/Parabox Cone", false, 400)]
	public static void MenuCreateCone()
	{
		GameObject go = new GameObject();
		go.AddComponent<InteractiveCone>().Init();
		Selection.objects = new Object[1] { go };
	}

#endregion
}
