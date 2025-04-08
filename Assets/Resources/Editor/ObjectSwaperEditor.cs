using UnityEditor;
using UnityEngine;

public class ObjectSwaperEditor : EditorWindow
{

    ObjectSwap source;

    [MenuItem("Tools/Objects Swaper #o")]
    public static void ShowWindow()
    {
        GetWindow<ObjectSwaperEditor>();
    }

    void SwapNow()
    {

    }

    private void OnGUI()
    {
        GUILayout.Label("Source: ");
        source = (ObjectSwap) EditorGUILayout.EnumPopup(source);
        if(source == ObjectSwap.BY_NAME) EditorGUILayout.Field
        GUILayout.Space(10);

        GUILayout.Label("Object: ");


    }

}

public enum ObjectSwap
{
    BY_NAME,
    BY_PREFAB
}
