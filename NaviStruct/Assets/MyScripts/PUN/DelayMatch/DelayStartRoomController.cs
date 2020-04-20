using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DelayStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int waitingRoomSceneIndex; //Number for the build index to the Waiting Room scene

    [SerializeField]
    private Button loader;        

    public override void OnEnable()
    {
        //register to photon callback functions
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        //unregister to photon callback functions
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() //Callback function for when we successfully create or join a room
    {
        Debug.Log("Joined Room");
        StartWaitingRoom();
    }

    private void StartWaitingRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Going To Waiting Room");
            StartCoroutine(LoadAsynchronously(waitingRoomSceneIndex));
        }
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loader.image.fillAmount = progress;            
            yield return null;
        }
    }
}
