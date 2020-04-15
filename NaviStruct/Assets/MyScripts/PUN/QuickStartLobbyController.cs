using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject quickStartButton;

    [SerializeField]
    private GameObject quickCancelButton;

    [SerializeField]
    private int roomSize;

    [HideInInspector]
    public int randomRoomNumber;

    public override void OnConnectedToMaster() //Callback function for when the first connection is established
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartButton.SetActive(true);
    }
    
    public void QuickStart()//Paired to the Quick Start button
    {
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); //First tries to join an existing room       
    }

    // Update is called once per frame
    public override void OnJoinRandomFailed(short returnCode, string message) //Callback function for when you cannot connect to a room
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom()//if failed, try and create our own room
    {
        Debug.Log("Creating room now");
        randomRoomNumber = Random.Range(0, 10000); //creating a random name for the room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps); //attempting to create a new room
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)//callback function for if creating a room fails
    {
        Debug.Log("Failed to create room...trying again");
        CreateRoom(); //Retrying to create a new room with a different name
    }

    public void QuickCancel()
    {
        quickCancelButton.SetActive(false);
        quickStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
