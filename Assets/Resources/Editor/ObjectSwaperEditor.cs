using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSwaperEditor : EditorWindow
{
    Object fromPrefab, toPrefab;
    string fromName;
    ObjectSwap source;

    [MenuItem("Tools/Objects Swaper #o")]
    public static void ShowWindow()
    {
        GetWindow<ObjectSwaperEditor>();
    }

    void Replace(GameObject from)
    {
        GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(toPrefab);
        go.transform.parent = from.transform.parent;
        go.transform.localPosition = from.transform.localPosition;
        go.transform.localRotation = from.transform.localRotation;
        go.transform.localScale = from.transform.localScale;
        DestroyImmediate(from);
    }

    void TrySwap(GameObject g)
    {
        if(g.transform.childCount == 0)
        {

            switch (source)
            {
                case ObjectSwap.BY_NAME:
                    if (g.name == fromName) Replace(g);
                    break;

                case ObjectSwap.BY_PREFAB:
                    var q = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(g);
                    if(q != null) { 
                        if(q == fromPrefab) Replace(g);
                    }
                    break;

                default:
                    break;
            }

            return;
        }
    }

    void SwapNow()
    {
        GameObject[] result = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(var r in result)
        {
            TrySwap(r);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Source: ");
        source = (ObjectSwap) EditorGUILayout.EnumPopup(source);
        if (source == ObjectSwap.BY_NAME) fromName = EditorGUILayout.TextField("name contains: ", fromName);
        if (source == ObjectSwap.BY_PREFAB) fromPrefab = EditorGUILayout.ObjectField("source prefab: ", fromPrefab, typeof(GameObject), false);
        GUILayout.Space(10);

        GUILayout.Label("Destination: ");
        toPrefab = EditorGUILayout.ObjectField("source prefab: ", toPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Apply")) SwapNow();

    }

}

public enum ObjectSwap
{
    BY_NAME,
    BY_PREFAB
}
