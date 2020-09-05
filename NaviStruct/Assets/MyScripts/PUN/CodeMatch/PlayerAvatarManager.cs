using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using Photon.Realtime;
using System.Collections;
using System.Linq;

public class PlayerAvatarManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private MonoBehaviour[] local_Scripts;   
    [SerializeField]
    private GameObject avatar_Mesh;
    [SerializeField]
    private SkinnedMeshRenderer mesh_Transparent;
    [SerializeField]
    private GameObject[] hand_Prefabs;    

    private bool isVREnabled;
    private bool isAREnabled;
    private bool stateChangeStart;    

    private GameObject playground;  
    private GameObject arRig;
    private GameObject[] arChildren;
    private GameObject vrRig;
    private GameObject standaloneRig;
    private GameObject vr_Canvas;
    private Canvas name_Canvas;

    private XRController[] controllers;    

    public float transparency_Value = 0;
    public float cutout_Value = 0;
    public float speed_Value = 2;

    public struct AssignRig
    {
        public GameObject player_Rig;

        public void Attach(GameObject activeRig, GameObject deactiveRig1, GameObject deactiveRig2)
        {
            activeRig.SetActive(true);
            deactiveRig1.SetActive(false);
            deactiveRig2.SetActive(false);

            player_Rig = activeRig;
        }
    }

    public AssignRig rig;

    private void Start()
    {
        isVREnabled = MasterManager.ClassReference.IsVRSupport;
        isAREnabled = MasterManager.ClassReference.IsARSupport;
        playground = MasterManager.ClassReference.Playground;       

        arRig = GameObject.Find("AR_Rig");
        vrRig = GameObject.Find("VR_Rig");
        standaloneRig = GameObject.Find("Standalone_Rig");
        vr_Canvas = GameObject.Find("VR_Canvas");
        name_Canvas = this.GetComponentInChildren<Canvas>();
        
        SetXRActiveObjects();
    }

    private void SetXRActiveObjects()
    {                      
        this.transform.SetParent(playground.transform);

        if (!photonView.IsMine)
        {
            if (avatar_Mesh != null)
                avatar_Mesh.SetActive(true);

            if (mesh_Transparent != null)
                mesh_Transparent.gameObject.SetActive(true);

            for (int i = 0; i < local_Scripts.Length; i++)
            {
                local_Scripts[i].enabled = false;
            }
        }
        else
        {
            if (avatar_Mesh != null)
                avatar_Mesh.SetActive(false);

            if (mesh_Transparent != null)
                mesh_Transparent.gameObject.SetActive(false);

            if (isAREnabled)
            {
                rig.Attach(arRig, vrRig, standaloneRig);                
                arChildren = this.GetComponentsInChildren<GameObject>(true);                
               
                vr_Canvas.SetActive(false);
                playground.SetActive(false);

                for (int i = 0; i < arChildren.Length; i++)
                {
                    if(!arChildren[i].activeInHierarchy)
                        arChildren[i].gameObject.SetActive(true);                    
                }                
            }       
            else
            {
                playground.SetActive(true);                

                if (isVREnabled)
                {
                    rig.Attach(vrRig, arRig, standaloneRig);                  
                    vr_Canvas.SetActive(true);
                    controllers = vrRig.GetComponentsInChildren<XRController>();

                    for (int i = 0; i < controllers.Length; i++)
                    {
                        controllers[i].modelPrefab = hand_Prefabs[i].transform;
                        controllers[i].animateModel = true;
                        controllers[i].modelSelectTransition = "ToPoint";
                        controllers[i].modelDeSelectTransition = "ToIdle";
                    }
                }
                else
                {
                    rig.Attach(standaloneRig, vrRig, arRig);      
                    vr_Canvas.SetActive(false);
                }                
            }
        }
    }

    private void Update()
    {        
        if(stateChangeStart)
            photonView.RPC("Disappear", RpcTarget.All, true); 
        else
            photonView.RPC("Disappear", RpcTarget.All, false);
    }

    [PunRPC]
    public void Disappear(bool value)
    {        
        StartCoroutine(ChangeDisappearValues(value));        
    }

    public IEnumerator ChangeDisappearValues(bool isActive)
    {
        if (mesh_Transparent != null && avatar_Mesh != null)
        {
            float i = 0f;
            float time = 3f;
            float rate = (1.0f / time) * speed_Value;

            bool temp = false;

            if (isActive && !temp)
            {
                this.photonView.enabled = false;
                while (i < 1)
                {
                    i += Time.deltaTime * rate;       
                    
                    transparency_Value = transparency_Value < 0.5f ? transparency_Value + Mathf.Lerp(0, 0.5f, i) : 0.5f;
                    mesh_Transparent.material.SetFloat("_Transparency", transparency_Value);
                    name_Canvas.enabled = false;

                    yield return null;
                }

                if (transparency_Value == .5f)
                {
                    avatar_Mesh.SetActive(false);

                    cutout_Value = cutout_Value < 1f ? cutout_Value + Mathf.Lerp(0, 1f, Time.deltaTime * rate) : 1f;
                    mesh_Transparent.material.SetFloat("_CutoutThresh", cutout_Value);
                }

                yield return new WaitForSeconds(3f);

                this.photonView.enabled = true;
                temp = true;
                i = 0f;
            }
            
            if(temp)
            {
                while (i < 1)
                {
                    i += Time.deltaTime * rate;
                    cutout_Value = cutout_Value > 0f ? Mathf.Lerp(cutout_Value, 0, i) : 0f;
                    
                    mesh_Transparent.material.SetFloat("_CutoutThresh", cutout_Value);                    

                    yield return null;
                }

                if (cutout_Value == 0)
                {
                    if (!photonView.IsMine)
                        avatar_Mesh.SetActive(true);                    

                    transparency_Value = transparency_Value > 0 ? transparency_Value - Mathf.Lerp(0.5f, 0, Time.deltaTime) : 0f;
                    mesh_Transparent.material.SetFloat("_Transparency", transparency_Value);
                    name_Canvas.enabled = true;
                }

                yield return new WaitForSeconds(3f);

                temp = false;
            }    
        }
    }

    public void SetAvatarParent(bool isChange)
    {
        stateChangeStart = isChange;

        if (isChange)
        {                        
            this.transform.SetParent(null);
        }                       
        else
        {
            this.transform.SetParent(playground.transform);                        
        }
                                      
    }
}
