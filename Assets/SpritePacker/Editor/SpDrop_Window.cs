using UnityEngine;
using UnityEditor;

public class SpDrop_Window : EditorWindow
{
	public SpAtlas CurrentAtlas;
	
	public static SpDrop_Window Open()
	{
		return EditorWindow.GetWindow<SpDrop_Window>("Drag and Drop");
	}
	
	public void OnEnable()
	{
		Repaint();
	}
	
	public void OnGUI()
	{
		var dropRect = SpHelper.Reserve(position.height - 2.0f);
		
		if (CurrentAtlas != null)
		{
			if (GUI.Button(dropRect, "Add To " + CurrentAtlas.name + "\n(Drag And Drop)") == true)
			{
				Selection.activeObject = CurrentAtlas;
			}
			
			SpHelper.DrawDragAndDropZone(dropRect);
			
			SpHelper.HandleDradAndDrop(CurrentAtlas, dropRect);
		}
		else
		{
			EditorGUI.BeginDisabledGroup(true);
			{
				GUI.Button(dropRect, "No Atlas Selected");
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}