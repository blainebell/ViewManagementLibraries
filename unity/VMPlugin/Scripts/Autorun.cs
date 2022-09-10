#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Autorun
{
    static Autorun()
    {
        if (!Application.isPlaying) // isEditor
        {
            EditorApplication.update += RunUntil;
        }
    }
    static void RunUntil()
    {
        ViewManager vm = ViewManager.Instance;
        if (vm != null)
        {
            if (vm.checkLicenseOnStart)
            {
                vm.CheckLicense();
                EditorApplication.update -= RunUntil;
            }
        }
    }
}
#endif
