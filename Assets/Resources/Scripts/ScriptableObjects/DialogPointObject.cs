using Articy.Test;
using Articy.Unity;
using UnityEngine;


public enum StartType
{
    ENTRY_POINT,
    CHARACTER_NAME
}

[CreateAssetMenu(fileName = "DialogPoint", menuName = "")]
public class DialogPointObject : ScriptableObject
{

    public StartType startType;
    public string characterName;

    [ArticyTypeConstraint(typeof(Dialogue), typeof(Hub))]
    public ArticyRef entryPoint;


    public void Run()
    {
        switch (startType)
        {
            case StartType.ENTRY_POINT:
                NarrationSystem.Get().StartWith(entryPoint.GetObject());
                break;

            case StartType.CHARACTER_NAME:
                NarrationSystem.Get().StartWith(characterName);
                break;

        }
    }

}
