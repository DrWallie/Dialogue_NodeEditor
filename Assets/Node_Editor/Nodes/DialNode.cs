using UnityEngine;

[System.Serializable]
public class DialNode : Node
{
	//public enum CalcType { Add, Substract, Multiply, Divide }
	//public CalcType type = CalcType.Add;
    public int outputCount = 1;
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
        
        #region Deprecated
        /*if (inputs[0].connection != null)
        {
            GUILayout.Label(inputs[0].name);
        }*/

        /*else
        {
            Input1Val = EditorGUILayout.FloatField(Input1Val);
        }*/

        /*if (Event.current.type == EventType.Repaint)
        {
            inputs[0].SetRect(GUILayoutUtility.GetLastRect());
        }*/
        /*if (inputs[1].connection != null)
        {
            GUILayout.Label(inputs[1].name);
        }*/
        /*else
        {
            Input2Val = EditorGUILayout.FloatField(Input2Val);
        }*/

        /*if (Event.current.type == EventType.Repaint)
        {
            inputs[1].SetRect(GUILayoutUtility.GetLastRect());
        }*/

        /*if (GUILayout.Button(new GUIContent("+", "Adds extra options"), plusButton))
        {
            
        }*/
        #endregion

        GUILayout.EndVertical ();
		GUILayout.BeginVertical ();

        inputs[0].DisplayLayout ();
        for(int i = 0; i <= outputCount - 1; i++)
        {
            outputs[i].DisplayLayout();
        }
        //outputs[0].DisplayLayout ();
		//We take that this time, because it has a GuiStyle to aligned to the right

		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();

        //GUILayout.Button(new GUIContent("+", "Adds extra options"), plusButton);
        if (GUI.Button(new Rect(175, 0, 25, 25), "+")) //Dial Node is standaard 200 bij 100
        {
            outputCount += 1;
            outputs[outputCount-1].DisplayLayout();
            DialNode node = ScriptableObject.CreateInstance<DialNode>();
            NodeOutput.Create(node, "Output 1", typeof(string));
            Debug.Log(outputCount);
        }

        if (GUI.Button(new Rect(150, 0, 25, 25), "-"))
        {
            outputCount -= 1;
            DialNode node = ScriptableObject.CreateInstance<DialNode>();
            NodeOutput.Create(node, "Output 1", typeof(string));
            Debug.Log(outputCount);
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