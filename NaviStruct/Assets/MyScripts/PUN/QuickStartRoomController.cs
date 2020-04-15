using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex; //Number for the build index to the multiplay scene

    [SerializeField]
    private Button loader;

    [SerializeField]
    private TextMeshProUGUI roomText;

    [SerializeField]
    private QuickStartLobbyController roomNumber;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() //Callback function for when we successfully create or join a room
    {
        Debug.Log("Joined Room");
        StartGame();
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game");            
            StartCoroutine(LoadAsynchronously(multiplayerSceneIndex));            
        }
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        PhotonNetwork.LoadLevel(sceneIndex);
        AsyncOperation operation = PhotonNetwork._AsyncLevelLoadingOperation;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loader.image.fillAmount = progress;
            roomText.text = "Room #: " + roomNumber.randomRoomNumber;
            yield return null;
        }
    }
}
