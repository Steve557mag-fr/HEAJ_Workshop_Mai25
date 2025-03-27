using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

internal class ReflectionUtils
{

    /// <summary>
    /// Give all classes derived from the T class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>array of all classes derived by T</returns>
    internal static T[] GetClassesOf<T>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(c => c.IsSubclassOf(typeof(T)))
            .ToArray() as T[];
    }

}
