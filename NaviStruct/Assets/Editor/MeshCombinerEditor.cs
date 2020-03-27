using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(CombineMeshes))]
public class MeshCombinerEditor : Editor
{
    private void OnSceneGUI()
    {
        CombineMeshes cm = target as CombineMeshes;
        if (Handles.Button(cm.transform.position + Vector3.up * 5, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CylinderHandleCap))        
            cm.AdvancedMerge();         
    }
}
