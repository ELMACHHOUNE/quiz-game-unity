using UnityEngine;
using UnityEditor;

public class DumpHierarchy
{
    public static void Run()
    {
        GameObject prefab = Resources.Load<GameObject>("CharacterPS-01");
        if (prefab != null)
        {
            Debug.Log("--- HIERARCHY START ---");
            Dump(prefab.transform, "");
            Debug.Log("--- HIERARCHY END ---");
        }
        else
        {
            Debug.LogError("Could not load CharacterPS-01 prefab!");
        }
    }

    static void Dump(Transform t, string indent)
    {
        Debug.Log(indent + t.name);
        foreach (Transform child in t)
        {
            Dump(child, indent + "  ");
        }
    }
}
