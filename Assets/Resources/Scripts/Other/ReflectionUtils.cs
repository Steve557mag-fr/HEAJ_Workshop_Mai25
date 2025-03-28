using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class ReflectionUtils
{


    /// <summary>
    /// Give all classes derived from the T class
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    /// <returns>array of all classes derived by T</returns>
    public static System.Type[] GetClassesOf<T>()
    {
        List<System.Type> result = new List<System.Type>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach(var assembly in assemblies)
        {
            foreach (var tp in assembly.GetTypes()) {

                if (tp.IsSubclassOf(typeof(CardBase))) result.Add(tp);   
            }
        }
        return result.ToArray(); 
    }

}
