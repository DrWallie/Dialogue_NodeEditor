using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class CalcNode : Node 
{
	public enum CalcType { Add, Substract, Multiply, Divide }
	public CalcType type = CalcType.Add;

	public float Input1Val = 1f;
	public float Input2Val = 1f;

	public static CalcNode Create (Rect NodeRect) 
	{// This function has to be registered in Node_Editor.ContextCallback
		CalcNode node = ScriptableObject.CreateInstance <CalcNode> ();

		node.name = "Calc Node";
		node.rect = NodeRect;
		
		NodeInput.Create (node, "Input 1", typeof (float));
		NodeInput.Create (node, "Input 2", typeof (float));
		
		NodeOutput.Create (node, "Output 1", typeof (float));

		node.Init ();
		return node;
	}

	public override void DrawNode () 
	{
		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical ();

		if (inputs [0].connection != null)
			GUILayout.Label (inputs [0].name);
		else
			Input1Val = EditorGUILayout.FloatField (Input1Val);
		if (Event.current.type == EventType.Repaint) 
			inputs [0].SetRect (GUILayoutUtility.GetLastRect ());
		// --
		if (inputs [1].connection != null)
			GUILayout.Label (inputs [1].name);
		else
			Input2Val = EditorGUILayout.FloatField (Input2Val);
		if (Event.current.type == EventType.Repaint) 
			inputs [1].SetRect (GUILayoutUtility.GetLastRect ());

		GUILayout.EndVertical ();
		GUILayout.BeginVertical ();

		outputs [0].DisplayLayout ();
		// We take that this time, because it has a GuiStyle to aligned to the right :)

		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();

		type = (CalcType)EditorGUILayout.EnumPopup (new GUIContent ("Calculation Type", "The type of calculation performed on Input 1 and Input 2"), type);

		if (GUI.changed)
			Node_Editor.editor.RecalculateFrom (this);
	}

	public override bool Calculate () 
	{
		if (inputs [0].connection != null && inputs [0].connection.value != null) 
			Input1Val = (float)inputs [0].connection.value;
		if (inputs [1].connection != null && inputs [1].connection.value != null) 
			Input2Val = (float)inputs [1].connection.value;

		switch (type) 
		{
		case CalcType.Add:
			outputs [0].value = Input1Val + Input2Val;
			break;
		case CalcType.Substract:
			outputs [0].value = Input1Val - Input2Val;
			break;
		case CalcType.Multiply:
			outputs [0].value = Input1Val * Input2Val;
			break;
		case CalcType.Divide:
			outputs [0].value = Input1Val / Input2Val;
			break;
		}

		return true;
	}

	public override void OnDelete () 
	{
		base.OnDelete ();
		// Always call this if we want our custom OnDelete operations!
		// Else you can leave this out
	}
}
