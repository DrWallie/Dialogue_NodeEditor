using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public abstract class Node : ScriptableObject
{
	public Rect rect = new Rect ();

	public List<NodeInput> inputs = new List<NodeInput> ();
	public List<NodeOutput> outputs = new List<NodeOutput> ();

	/// <summary>
	/// Init this node. Has to be called when creating a child node of this
	/// </summary>
	protected void Init () 
	{
		Calculate ();
		Node_Editor.editor.nodeCanvas.nodes.Add (this);
		if (!String.IsNullOrEmpty (AssetDatabase.GetAssetPath (Node_Editor.editor.nodeCanvas)))
		{
			AssetDatabase.AddObjectToAsset (this, Node_Editor.editor.nodeCanvas);
            for (int inCnt = 0; inCnt < inputs.Count; inCnt++)
            {
                AssetDatabase.AddObjectToAsset(inputs[inCnt], this);
            }

            for (int outCnt = 0; outCnt < outputs.Count; outCnt++)
            {
                AssetDatabase.AddObjectToAsset(outputs[outCnt], this);
            }
			AssetDatabase.ImportAsset (Node_Editor.editor.openedCanvasPath);
			AssetDatabase.Refresh ();

		}

	}

	/// <summary>
	/// Function implemented by the children to draw the node
	/// </summary>
	public abstract void DrawNode ();

	/// <summary>
	/// Function implemented by the children to calculate their outputs
	/// Should return Success/Fail
	/// </summary>
	public abstract bool Calculate ();

	/// <summary>
	/// Draws the node curves as well as the knobs	
	/// </summary>
	public void DrawConnectors () 
	{
		for (int outCnt = 0; outCnt < outputs.Count; outCnt++) 
		{
			NodeOutput output = outputs [outCnt];
			for (int conCnt = 0; conCnt < output.connections.Count; conCnt++) 
			{
                if (output.connections[conCnt] != null)
                {
                    Node_Editor.DrawNodeCurve(output.GetKnob().center, output.connections[conCnt].GetKnob().center);
                }
                else
                {
                    output.connections.RemoveAt(conCnt);
                }

			}
			GUI.DrawTexture (output.GetKnob (), Node_Editor.ConnectorKnob);
		}
		for (int inCnt = 0; inCnt < inputs.Count; inCnt++) 
		{
            //GUI.DrawTexture (inputs [inCnt].GetKnob (), Node_Editor.ConnectorKnob);
            GUI.DrawTextureWithTexCoords(inputs[inCnt].GetKnob(), Node_Editor.ConnectorKnob, new Rect(1, 0, -1, 1));

        }
	}

	/// <summary>
	/// Callback when the node is deleted. Extendable by the child node, but always call base.OnDelete when overriding !!
	/// </summary>
	public virtual void OnDelete () 
	{
		for (int outCnt = 0; outCnt < outputs.Count; outCnt++) 
		{
			NodeOutput output = outputs [outCnt];
            for (int conCnt = 0; conCnt < output.connections.Count; conCnt++)
            {
                output.connections[outCnt].connection = null;
            }

		}
		for (int inCnt = 0; inCnt < inputs.Count; inCnt++) 
		{
            if (inputs[inCnt].connection != null)
            {
                inputs[inCnt].connection.connections.Remove(inputs[inCnt]);
            }

		}

		DestroyImmediate (this, true);
		if (!String.IsNullOrEmpty (Node_Editor.editor.openedCanvasPath)) 
		{
			AssetDatabase.ImportAsset (Node_Editor.editor.openedCanvasPath);
			AssetDatabase.Refresh ();
		}

	}

	#region Member Functions

	/// <summary>
	/// Checks if there are no unassigned and no null-value inputs.
	/// </summary>
	public bool allinputsReady () 
	{
		for (int inCnt = 0; inCnt < inputs.Count; inCnt++) 
		{
            if (inputs[inCnt].connection == null || inputs[inCnt].connection.value == null)
            {
                return false;
            }

		}
		return true;
	}

	/// <summary>
	/// Checks if there are any unassigned inputs.
	/// </summary>
	public bool hasNullinputs () 
	{
		for (int inCnt = 0; inCnt < inputs.Count; inCnt++) 
		{
            if (inputs[inCnt].connection == null)
            {
                return true;
            }

		}
		return false;
	}

	public bool hasNullInputValues () 
	{
		for (int inCnt = 0; inCnt < inputs.Count; inCnt++) 
		{
            if (inputs[inCnt].connection != null && inputs[inCnt].connection.value == null)
            {
                return true;
            }

		}
		return false;
	}

    /*public bool hasNullOutputValues()
    {
        for (int outCnt = 0; outCnt < inputs.Count; outCnt++)
        {
            if (outputs[outCnt].connections != null && outputs[outCnt].connection.value == null)
            {
                return true;
            }

        }
        return false;
    }*/

    /// <summary>
    /// Returns the input knob that is at the position on this node or null
    /// </summary>
    public NodeInput GetInputAtPos (Vector2 pos) 
	{
		for (int inCnt = 0; inCnt < inputs.Count; inCnt++) 
		{ // Search for an input at the position
            if (inputs[inCnt].GetKnob().Contains(new Vector3(pos.x, pos.y)))
            {
                return inputs[inCnt];
            }

		}
		return null;
	}

	/// <summary>
	/// Returns the output knob that is at the position on this node or null
	/// </summary>
	public NodeOutput GetOutputAtPos (Vector2 pos) 
	{
		for (int outCnt = 0; outCnt < outputs.Count; outCnt++) 
		{ // Search for an output at the position
            if (outputs[outCnt].GetKnob().Contains(new Vector3(pos.x, pos.y)))
            {
                return outputs[outCnt];
            }

		}
		return null;
	}

	/// <summary>
	/// Recursively checks whether this node is a child of the other node
	/// </summary>
	public bool isChildOf (Node otherNode)
	{
        if (otherNode == null)
        {
            return false;
        }

		for (int cnt = 0; cnt < inputs.Count; cnt++) 
		{
			if (inputs [cnt].connection != null) 
			{
                if (inputs[cnt].connection.body == otherNode)
                {
                    return true;
                }
                else if (inputs[cnt].connection.body.isChildOf(otherNode)) // Recursively searching
                {
                    return true;
                }

			}
		}
		return false;
	}
    #endregion

    #region static Functions
    /// <summary>
    /// Check if an output and an input can be connected (same type, ...)
    /// </summary>
    public static bool CanApplyConnection(NodeOutput output, NodeInput input)
    {
        if (input == null || output == null)
        {
            return false;
        }

        if (input.body == output.body || input.connection == output)
        {
            return false;
        }

        if (input.type != output.type)
        {
            return false;
        }

		if (output.body.isChildOf (input.body)) 
		{
			Node_Editor.editor.ShowNotification (new GUIContent ("Recursion detected!"));
			return false;
		}
		return true;
	}
	/// <summary>
	/// Applies a connection between output and input. 'CanApplyConnection' has to be checked before
	/// </summary>
	public static void ApplyConnection (NodeOutput output, NodeInput input)
	{
		if (input != null && output != null) 
		{
			if (input.connection != null) 
			{
				input.connection.connections.Remove (input);
			}
			input.connection = output;
			output.connections.Add (input);

			Node_Editor.editor.RecalculateFrom (input.body);
		}
	}
	#endregion

}