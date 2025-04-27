using UnityEditor;
using UnityEngine;

public class EditorUtils
{
    public static void GuiLine(int i_height = 1, float padding = 2)
    {
        GUILayout.Space(padding);

        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        
        GUILayout.Space(padding);
    }
}
