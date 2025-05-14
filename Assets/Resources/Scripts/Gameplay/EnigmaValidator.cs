using UnityEngine;

[CreateAssetMenu(fileName = "EnigmaValidator", menuName = "Scriptable Objects/EnigmaValidator")]
public class EnigmaValidator : ScriptableObject
{

    public virtual bool IsValid()
    {

        return true;
    }
}
