using Articy.Test;
using Articy.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "HintCraftData", menuName = "Scriptable Objects/HintCraftData")]
public class HintCraftData : ScriptableObject
{
    public HintObject[] hints;

    [ArticyTypeConstraint(typeof(Hub))]
    public ArticyRef dialogResult;

    public string characterResult;
    public string characterStateResult;

}
