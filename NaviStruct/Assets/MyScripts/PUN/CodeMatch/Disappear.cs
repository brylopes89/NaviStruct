using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public GameObject mesh_Opaque;
    public SkinnedMeshRenderer mesh_Transparent;

    public float transparency_Value = 0;
    public float cutout_Value = 0;
    public float value = 2; 
    private PlayerStateManager state_Manager;

    // Start is called before the first frame update
    void Start()
    {
        state_Manager = this.GetComponent<PlayerStateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state_Manager.IsStateChange)
        {
            transparency_Value = transparency_Value < 0.5f ? transparency_Value + Mathf.Lerp(0, 0.5f, Time.deltaTime * value) : 0.5f;
            mesh_Transparent.material.SetFloat("_Transparency", transparency_Value);

            if (transparency_Value == .5f)
            {
                mesh_Opaque.SetActive(false);

                cutout_Value = cutout_Value < 1f ? cutout_Value + Mathf.Lerp(0, 1f, Time.deltaTime * value) : 1f;
                mesh_Transparent.material.SetFloat("_CutoutThresh", cutout_Value);
            }        
        }
        else
        {
            cutout_Value = cutout_Value > 0f ? cutout_Value - Mathf.Lerp(1f, 0, Time.deltaTime * value) : 0f;
            mesh_Transparent.material.SetFloat("_CutoutThresh", cutout_Value);

            if(cutout_Value == 0)
            {
                mesh_Opaque.SetActive(true);

                transparency_Value = transparency_Value > 0 ? transparency_Value - Mathf.Lerp(0.5f, 0, Time.deltaTime * value) : 0f;
                mesh_Transparent.material.SetFloat("_Transparency", transparency_Value);
            } 
        }
    }


}
