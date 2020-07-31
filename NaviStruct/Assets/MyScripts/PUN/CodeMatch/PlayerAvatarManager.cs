using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class PlayerAvatarManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private MonoBehaviour[] localScripts;   
    [SerializeField]
    private GameObject avatarMesh;
    [SerializeField]
    private GameObject[] handPrefabs;
    [SerializeField]
    private float stateChangeWaitTime;    

    private float stateChangeTimer;  

    private bool isStateChange = false;
    private bool isVREnabled;
    private bool isAREnabled;

    private GameObject playground;
    private GameObject arRig;
    private GameObject[] arChildren;
    private GameObject vrRig;
    private GameObject standaloneRig;   
    private XRController[] controllers;       

    private void Start()
    {
        isVREnabled = MasterManager.ClassReference.IsVRSupport;
        isAREnabled = MasterManager.ClassReference.IsARSupport;
        playground = MasterManager.ClassReference.Playground;

        arRig = GameObject.Find("AR_Rig");
        vrRig = GameObject.Find("VR_Rig");
        standaloneRig = GameObject.Find("Standalone_Rig");          

        if (isVREnabled)
            controllers = vrRig.GetComponentsInChildren<XRController>();    
        
        stateChangeTimer = stateChangeWaitTime;
        SetXRActiveObjects();
    }

    private void SetXRActiveObjects()
    {
        if (!isAREnabled)                
            this.transform.SetParent(playground.transform);

        if (!photonView.IsMine)
        {
            if (avatarMesh != null && isVREnabled)
            {
                avatarMesh.SetActive(true);
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].modelPrefab = null;
                    controllers[i].animateModel = false;
                }
            }
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
        }
        else
        {
            if (avatarMesh != null)
                avatarMesh.SetActive(false);

            if (isAREnabled)
            {
                arChildren = this.GetComponentsInChildren<GameObject>(true);
                arRig.SetActive(true);
                standaloneRig.SetActive(false);
                vrRig.SetActive(false);                    

                for(int i = 0; i < arChildren.Length; i++)
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
                    vrRig.SetActive(true);
                    arRig.SetActive(false);
                    standaloneRig.SetActive(false);

                    for (int i = 0; i < controllers.Length; i++)
                    {
                        controllers[i].modelPrefab = handPrefabs[i].transform;
                        controllers[i].animateModel = true;
                        controllers[i].modelSelectTransition = "ToPoint";
                        controllers[i].modelDeSelectTransition = "ToIdle";
                    }
                }
                else
                {
                    standaloneRig.SetActive(true);                    
                    arRig.SetActive(false);
                    vrRig.SetActive(false);
                }                
            }
        }
    }

    private void Update()
    {
        UpdateTimer();       
    }

    private void UpdateTimer()
    {
        if (isStateChange)
        {
            stateChangeTimer -= Time.deltaTime;

            if (stateChangeTimer <= 0)
            {                
                SetAvatarParent(false);
                stateChangeTimer = stateChangeWaitTime;                     
            }
        }
    }   
    
    public void SetAvatarParent(bool isChanging)
    {       
        isStateChange = isChanging;

        if (photonView.IsMine)
        {
            if (isChanging)
                this.transform.SetParent(null);
            else
                this.transform.SetParent(playground.transform);
        }                      
    }   
}
