using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static T GetOrAddComponent<T>(this GameObject obj) where T : MonoBehaviour {
        T comp = obj.GetComponent<T>();

        if(comp == null) {
            comp = obj.AddComponent<T>();
        }

        return comp;
    }
}