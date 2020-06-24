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
    private Transform parentRig;
    [SerializeField]
    private Transform[] spawnPoints;

    [HideInInspector]
    public GameObject avatarPrefab;    
    [HideInInspector]
    public GameObject avatarPlayer;    

    private int spawnPicker;
    public float scaleFactor = 0.006f;

    private void Awake()
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
            avatarPlayer.transform.parent = parentRig;            
        }
        else
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;
        }

        //TODO: Apply scale factor to avatar and character controller
        avatarPlayer.transform.localScale = avatarPlayer.transform.localScale * scaleFactor; 
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
