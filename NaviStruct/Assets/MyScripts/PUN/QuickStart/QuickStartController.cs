using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class QuickStartController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int roomSize;

    [HideInInspector]
    public int randomRoomNumber;
    [HideInInspector]
    public bool quickSelected;

    private AnimationController animController;

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;
    }

    public void QuickStartOnClick()//Paired to the Quick Start button
    {
        quickSelected = true;
        //quickStartButton.SetActive(false);         
        PhotonNetwork.JoinRandomRoom(); //First tries to join an existing room       
    }

    // Update is called once per frame
    public override void OnJoinRandomFailed(short returnCode, string message) //Callback function for when you cannot connect to a room
    {
        //Debug.Log(message);
        StartCoroutine(animController.FadeStatusText(message));
        CreateRoom();
    }

    void CreateRoom()//if failed, try and create our own room
    {
        randomRoomNumber = Random.Range(0, 10000); //creating a random name for the room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps); //attempting to create a new room
    }

    public override void OnCreateRoomFailed(short returnCode, string message)//callback function for if creating a room fails
    {
        //Debug.Log("Failed to create room...trying again");
        CreateRoom(); //Retrying to create a new room with a different name
    }

    public void QuickCancel()
    {
        quickSelected = false;       
        PhotonNetwork.LeaveRoom();
    }
}
