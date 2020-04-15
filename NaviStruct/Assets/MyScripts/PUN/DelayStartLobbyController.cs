using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DelayStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject delayStartButton;

    [SerializeField]
    private GameObject delayCancelButton;

    //[SerializeField]
    //private GameObject joinRoomButton;

    [SerializeField]
    private int roomSize;
    
    public static int randomRoomNumber;

    public override void OnConnectedToMaster() //Callback function for when the first connection is established
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        delayStartButton.SetActive(true);
        //joinRoomButton.SetActive(true);
    }

    public void DelayStart()//Paired to the Quick Start button
    {
        delayStartButton.SetActive(false);
        //joinRoomButton.SetActive(false);
        delayCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); //First tries to join an existing room       
    }

    public void JoinRoom(string input)
    {
        delayStartButton.SetActive(false);
        //joinRoomButton.SetActive(false);
        delayCancelButton.SetActive(true);
        PhotonNetwork.JoinRoom(input);
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

    public void DelayCancel()
    {
        delayCancelButton.SetActive(false);
        delayStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
