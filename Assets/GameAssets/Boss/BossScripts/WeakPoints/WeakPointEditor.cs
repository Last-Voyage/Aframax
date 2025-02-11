using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(WeakPointHandler))]
public class WeakPointEditor : Editor
{
    public override void OnInspectorGUI()
    { 
        base.OnInspectorGUI();

        WeakPointHandler weakPointHandler = (WeakPointHandler)target;

        if(GUILayout.Button("Spawn weak point test")){   // for some freakin reason the weak point handler script disables itself
            weakPointHandler.SpawnWeakPointEditor();     // in my test scene so I had to make this, although rather useless
        }            
    }
}

#else

// Put here to trick the game into compiling with this script
public class WeakPointEditor {}

#endif
