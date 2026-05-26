using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PackageSetup
{
    static AddRequest request1;
    static AddRequest request2;
    static int step = 0;

    public static void Run()
    {
        request1 = Client.Add("com.unity.2d.animation");
        EditorApplication.update += Progress;
    }

    static void Progress()
    {
        if (step == 0)
        {
            if (request1.IsCompleted)
            {
                if (request1.Status == StatusCode.Success)
                    Debug.Log("Added com.unity.2d.animation");
                else if (request1.Status >= StatusCode.Failure)
                    Debug.Log("Failed to add animation: " + request1.Error.message);

                step = 1;
                request2 = Client.Add("com.unity.2d.psdimporter");
            }
        }
        else if (step == 1)
        {
            if (request2.IsCompleted)
            {
                if (request2.Status == StatusCode.Success)
                    Debug.Log("Added com.unity.2d.psdimporter");
                else if (request2.Status >= StatusCode.Failure)
                    Debug.Log("Failed to add psdimporter: " + request2.Error.message);

                EditorApplication.update -= Progress;
                EditorApplication.Exit(0);
            }
        }
    }
}
