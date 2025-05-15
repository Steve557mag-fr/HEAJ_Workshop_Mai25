using UnityEngine;

public class EnigmaInteraction : BaseInteraction
{
    [SerializeField] EnigmaValidator Validator;

    public void Refresh()
    {
        if (Validator.IsValid())
        {
            Trigger();
        }
    }

}
