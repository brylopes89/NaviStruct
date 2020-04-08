using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshes : MonoBehaviour
{
    public List<Material> showMats = new List<Material>();

    // Start is called before the first frame update
    public void CombineMeshFilters()
    {
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;        
        
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(false);
        CombineInstance[] combineFilters = new CombineInstance[meshFilters.Length];
        
        Mesh finalMesh = new Mesh();

        int i = 0;
        while (i < meshFilters.Length)
        {
            //Debug.Log(meshFilters[i].gameObject.transform.name);
            combineFilters[i].mesh = meshFilters[i].sharedMesh;
            combineFilters[i].transform = meshFilters[i].transform.localToWorldMatrix;            
            
            i++;
        }

        finalMesh.CombineMeshes(combineFilters);

        transform.GetComponent<MeshFilter>().sharedMesh = finalMesh;

        transform.rotation = oldRot;
        transform.position = oldPos;
        
        //saveMesh();
    }

    public void AdvancedMerge()
    {
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        // All our children (and us)
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(false);

        // All the meshes in our children (just a big list)
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false); // <-- you can optimize this
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                {
                    materials.Add(localMat);
                    showMats.Add(localMat);
                }                    
        }

        // Each material will have a mesh for it.
        List<Mesh>submeshes = new List<Mesh>();

        foreach (Material material in materials)
        {
            // Make a combiner for each (sub)mesh that is mapped to the right material.
            List<CombineInstance> combiners = new List<CombineInstance>();
            foreach (MeshFilter filter in filters)
            {
                if (filter.transform == transform) continue;
                // The filter doesn't know what materials are involved, get the renderer.
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();  // <-- (Easy optimization is possible here, give it a try!)
                if (renderer == null)
                {
                    Debug.LogError(filter.name + " has no MeshRenderer");
                    continue;
                }

                // Let's see if their materials are the one we want right now.
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)
                        continue;
                    // This submesh is the material we're looking for right now.
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex;
                    ci.transform = filter.transform.localToWorldMatrix;
                    combiners.Add(ci);                    
                }
            }
            // Flatten into a single mesh.
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(), true);
            submeshes.Add(mesh);
            
        }

        // The final mesh: combine all the material-specific meshes as independent submeshes.
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            finalCombiners.Add(ci);
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
        transform.GetComponent<MeshFilter>().sharedMesh = finalMesh;
        

        transform.rotation = oldRot;
        transform.position = oldPos;
        Debug.Log("Final mesh has " + submeshes.Count + " materials.");
    }

    void saveMesh()
    {
        Debug.Log("Saving Mesh?");
        Mesh m1 = transform.GetComponent<MeshFilter>().mesh;
        //AssetDatabase.CreateAsset(m1, "Assets/MyModels/" + transform.name + ".asset"); // saves to "assets/"
        //AssetDatabase.SaveAssets();
    }

}
