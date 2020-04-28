using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeMatchLobbyJoin : MonoBehaviourPunCallbacks
{
    [Header("Start Menu Display")]
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField]
    private GameObject lobbyConnectButton;

    [Header("Lobby/Create Display")]
    [SerializeField]
    private TMP_InputField roomSizeInputField;
    [SerializeField]
    private TMP_InputField codeCreateInputField;

    [Header("Join Panel Display")]
    [SerializeField]
    private TMP_InputField codeInputField;
    [SerializeField]
    private GameObject joinButton;
    private string joinCode;

    [Header("Available Rooms Display")]
    [SerializeField]
    private Transform roomsContainer;
    [SerializeField]
    private GameObject roomListingPrefab;

    private List<RoomInfo> roomListings;
    private string roomName;
    private int roomSize;

    public override void OnConnectedToMaster()
    {
        roomListings = new List<RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyConnectButton.SetActive(true);

        if (PlayerPrefs.HasKey("NickName"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000); //Generate random player nickname if empty
            else
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName"); //get saved player name
        }
        else
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);
        }

        playerNameInput.text = PhotonNetwork.NickName; //update input field with player name
    }

    public void PlayerNameUpdate(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobbyOnClick()
    {
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.mainAnim, "IsFadeOut", AnimationController.animController.lobbyPanel, AnimationController.animController.mainPanel));
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Joined Lobby"));
    }

    public void OnRoomSizeInputChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void OnRoomCodeInputChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void CreateRoomOnClick()
    {
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.lobbyAnim, "IsFadeOut", AnimationController.animController.roomPanel, AnimationController.animController.lobbyPanel));        

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();        

        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Create Room Failed"));
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Created Room Successfully"));
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        foreach (RoomInfo room in roomList)
        {
            if (roomListings != null) //try to find existing room listing
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            else
                tempIndex = -1;

            if (tempIndex != -1) //remove listing because it has been closed
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }

            if (room.PlayerCount > 0) //add room listing because it is new
            {
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void ListRoom(RoomInfo room) //Displays new room listing for the current room
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public void CodeInput(string code)
    {
        joinCode = code;
    }

    public void JoinRoomOnClick()
    {        
        PhotonNetwork.JoinRoom(joinCode);

        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.joinAnim, "IsFadeOut", AnimationController.animController.roomPanel, AnimationController.animController.joinPanel));
            StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "You have Joined Room " + roomName));
        }
    }        

    public void LeaveRoomOnClick()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.joinAnim, "IsFadeOut", AnimationController.animController.lobbyPanel, AnimationController.animController.joinPanel));
    }

    public void OpenJoinPanel()
    {
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.lobbyAnim, "IsFadeOut", AnimationController.animController.joinPanel, AnimationController.animController.lobbyPanel));
    }

    public void MatchMakingCancelOnClick()
    {
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.lobbyAnim, "IsFadeOut", AnimationController.animController.mainPanel, AnimationController.animController.lobbyPanel));
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "MatchMaking cancelled. Leaving Lobby"));
        PhotonNetwork.LeaveLobby();        
    }
}
