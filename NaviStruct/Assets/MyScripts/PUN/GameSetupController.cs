using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.Playables;

public class GameSetupController : MonoBehaviourPunCallbacks
{
    //public static GameSetupController gameSetup;

    [SerializeField]
    private int menuSceneIndex;   
    [SerializeField]
    private Transform[] spawnPoints;    
    [SerializeField]
    private GameObject xrRig;
    [SerializeField]
    private GameObject standaloneRig;
    
    [HideInInspector]
    public GameObject avatarPlayer;  
    
    private GameObject playground;
    private int spawnPicker;    

    private void Awake()
    {
        if (MasterManager.ClassReference.GameSetupController == null)
            MasterManager.ClassReference.GameSetupController = this;
       
        xrRig = GameObject.Find("XR_Rig");
        standaloneRig = GameObject.Find("Standalone_Rig");       

        //CreateSceneObjects();
        CreatePlayer();               
    }
    
    private void CreateSceneObjects()
    {
        Vector3 targetPos = new Vector3(0f, 1.52f, 0f);
        PhotonNetwork.InstantiateSceneObject(Path.Combine("PhotonPrefabs/SceneObjects", "Playground"), 
            targetPos, Quaternion.identity, 0);       
    }

    private void CreatePlayer()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);

        if (XRSettings.enabled)
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_VR"), 
                spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;

            avatarPlayer.transform.parent = xrRig.transform;
        }
        else
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle"),
                spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;
            avatarPlayer.transform.parent = standaloneRig.transform;
        }
        
        MasterManager.ClassReference.Avatar = avatarPlayer;        
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;

        SceneManager.LoadScene(menuSceneIndex);
    }
}
