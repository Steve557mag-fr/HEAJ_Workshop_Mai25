using Articy.Test;
using Articy.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogPoint", menuName = "")]
public class DialogPointObject : ScriptableObject
{

    [ArticyTypeConstraint(typeof(Dialogue))]
    public ArticyRef entryPoint;

    public void Run()
    {
        NarrationSystem.Get().StartWith(entryPoint);
    }

}
