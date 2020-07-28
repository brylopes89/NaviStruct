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
    private List<GameObject> arChildObjects = new List<GameObject>();
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

        if (MasterManager.ClassReference.Playground == null)
            MasterManager.ClassReference.Playground = playground;

        if (isVREnabled)
            controllers = GameObject.Find("VR_Rig").GetComponentsInChildren<XRController>();    
        
        stateChangeTimer = stateChangeWaitTime;
        SetXRActiveObjects();
    }

    private void SetXRActiveObjects()
    {
        if (!isAREnabled)
        {
            //if (!playground.activeInHierarchy)
            //    playground.SetActive(true);
            this.transform.SetParent(playground.transform);
        }
        else
        {
            //if (playground.activeInHierarchy)
            //    playground.SetActive(false);
        }

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
                arRig.SetActive(true);
                standaloneRig.SetActive(false);
                vrRig.SetActive(false);                    

                for(int i = 0; i < arRig.transform.childCount; i++)
                {
                    arChildObjects.Add(arRig.transform.GetChild(i).gameObject);

                    for (int a = 0; a < arChildObjects.Count; a++)
                    {
                        if (!arChildObjects[a].activeInHierarchy)
                            arChildObjects[a].SetActive(!arChildObjects[a].activeSelf);
                    }
                }                
            }       
            else if (isVREnabled)
            {
                vrRig.SetActive(true);
                playground.SetActive(true);
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
                playground.SetActive(true);
                arRig.SetActive(false);
                vrRig.SetActive(false);
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
