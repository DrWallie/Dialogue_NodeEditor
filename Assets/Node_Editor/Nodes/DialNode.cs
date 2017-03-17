using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class DialNode : Node
{
	//public enum CalcType { Add, Substract, Multiply, Divide }
	//public CalcType type = CalcType.Add;
    public int OutputCount = 1;
    //public static GUIStyle plusButton;

    public static DialNode Create (Rect NodeRect)
	{//This function has to be registered in Node_Editor.ContextCallback
		DialNode node = ScriptableObject.CreateInstance <DialNode> ();
		node.name = "Dialogue Node";
		node.rect = NodeRect;
		
		NodeInput.Create (node, "Input 1", typeof (string));
        NodeOutput.Create (node, "Output 1", typeof (string));

		node.Init ();
		return node;
	}

	public override void DrawNode ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical ();

        //plusButton = new GUIStyle(GUI.skin.button);
        //plusButton.normal.textColor = new Color(0.3f, 0.3f, 0.3f);
        
        #region Deprecated
        /*if (Inputs[0].connection != null)
        {
            GUILayout.Label(Inputs[0].name);
        }*/

        /*else
        {
            Input1Val = EditorGUILayout.FloatField(Input1Val);
        }*/

        /*if (Event.current.type == EventType.Repaint)
        {
            Inputs[0].SetRect(GUILayoutUtility.GetLastRect());
        }*/
        /*if (Inputs[1].connection != null)
        {
            GUILayout.Label(Inputs[1].name);
        }*/
        /*else
        {
            Input2Val = EditorGUILayout.FloatField(Input2Val);
        }*/

        /*if (Event.current.type == EventType.Repaint)
        {
            Inputs[1].SetRect(GUILayoutUtility.GetLastRect());
        }*/

        /*if (GUILayout.Button(new GUIContent("+", "Adds extra options"), plusButton))
        {
            
        }*/
        #endregion

        GUILayout.EndVertical ();
		GUILayout.BeginVertical ();

        Inputs[0].DisplayLayout();
        Outputs[0].DisplayLayout ();
		//We take that this time, because it has a GuiStyle to aligned to the right

		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();

        //GUILayout.Button(new GUIContent("+", "Adds extra options"), plusButton);
        if (GUI.Button(new Rect(175, 0, 25, 25), "+")) //Dial Node is standaard 200 bij 100
        {
            OutputCount += 1;
            Debug.Log(OutputCount);
        }
        if (GUI.Button(new Rect(150, 0, 25, 25), "-"))
        {
            OutputCount -= 1;
            Debug.Log(OutputCount);
        }
        //type = (CalcType)EditorGUILayout.EnumPopup (new GUIContent ("Calculation Type", "The type of calculation performed on Input 1 and Input 2"), type);

        if (GUI.changed)
        {
            //Node_Editor.editor.RecalculateFrom(this);
        }
	}

    public override bool Calculate ()
    {
        return true;
    }

    public override void OnDelete () 
	{
		base.OnDelete ();
		// Always call this if we want our custom OnDelete operations!
		// Else you can leave this out
	}
}