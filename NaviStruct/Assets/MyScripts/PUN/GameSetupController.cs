using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameSetupController : MonoBehaviour
{
    //public static GameSetupController gameSetup;

    [SerializeField]
    private int menuSceneIndex;
    [SerializeField]
    private Transform xrParentRig;   
    [SerializeField]
    private Transform[] spawnPoints;

    [HideInInspector]
    public GameObject avatarPrefab;    
    [HideInInspector]
    public GameObject avatarPlayer;    

    private int spawnPicker;   

    private void Start()
    {
        if (MasterManager.ClassReference.GameSetupController == null)
            MasterManager.ClassReference.GameSetupController = this;
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);

        if (XRSettings.enabled)
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_VR"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;
            avatarPlayer.transform.parent = xrParentRig;            
        }
        else
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;            
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
