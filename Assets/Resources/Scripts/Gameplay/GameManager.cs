using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    internal List<CraftDataObject> QueryAvailableCrafts()
    {
        return new();
    }

    internal static GameManager Get()
    {
        return FindFirstObjectByType<GameManager>();
    }
}
