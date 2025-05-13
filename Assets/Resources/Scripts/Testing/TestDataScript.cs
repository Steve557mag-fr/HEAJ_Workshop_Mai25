using UnityEngine;

[RequireComponent (typeof(DataSystem))]
public class TestDataScript : MonoBehaviour
{

    [SerializeField] string saveName;

    void Start()
    {
        DataSystem.instance.Load(saveName);
        print($"version: {DataSystem.instance.GetData<string>("version")}");
        DataSystem.instance.Save(saveName);
    }
}
