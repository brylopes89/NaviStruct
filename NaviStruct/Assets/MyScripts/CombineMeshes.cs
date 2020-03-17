using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshes : MonoBehaviour
{
    GameObject highlight;
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            Debug.Log(meshFilters[i].gameObject.transform.name);
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.active = false;
            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        saveMesh();
    }
    // Update is called once per frame
    void saveMesh()
    {
        Debug.Log("Saving Mesh?");
        Mesh m1 = transform.GetComponent<MeshFilter>().mesh;
        AssetDatabase.CreateAsset(m1, "Assets/Resources/Prefabs/" + transform.name + ".asset"); // saves to "assets/"
        AssetDatabase.SaveAssets();
    }
}
