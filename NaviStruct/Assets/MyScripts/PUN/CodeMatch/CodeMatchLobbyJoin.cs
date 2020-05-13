using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class CodeMatchLobbyJoin : MonoBehaviourPunCallbacks
{
    public static CodeMatchLobbyJoin lobby;

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

    [HideInInspector]
    public int roomSize;

    private void Awake()
    {
        lobby = this;
    }

    void Start()
    {        
        PhotonNetwork.ConnectUsingSettings();        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;        
        roomListings = new List<RoomInfo>();        
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

    public void OnPlayerNameInput(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobbyOnClick()
    {
        StartCoroutine(AnimationController.instance.FadeAnimation(AnimationController.instance.mainAnim, "IsFadeOut", AnimationController.instance.lobbyPanel, AnimationController.instance.mainPanel));
        PhotonNetwork.JoinLobby();
    }   

    public void OnRoomSizeInput(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void OnRoomNameInput(string nameIn)
    {
        roomName = nameIn;
    }

    public void CreateRoomOnClick()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();
        if (string.IsNullOrEmpty(roomSizeInputField.text))
        {
            StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "Please enter a room size", "isFadeMenu"));
            return;
        }
       
        StartCoroutine(AnimationController.instance.FadeAnimation(AnimationController.instance.lobbyAnim, "IsFadeOut", AnimationController.instance.roomPanel, AnimationController.instance.lobbyPanel));        

        PhotonNetwork.CreateRoom(roomName, roomOps);
    }   

    public void MatchMakingCancelOnClick()
    {
        StartCoroutine(AnimationController.instance.FadeAnimation(AnimationController.instance.lobbyAnim, "IsFadeOut", AnimationController.instance.mainPanel, AnimationController.instance.lobbyPanel));
        StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "MatchMaking cancelled. Leaving Lobby", "isFadeMenu"));
        PhotonNetwork.LeaveLobby();
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

    public void EnterRoomOnClick()
    {        
        if(PhotonNetwork.PlayerList.Length == roomSize)
        {
            StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "Room " + roomName + "is full. Please try another room.", "isFadeMenu"));
            return;
        }            

        PhotonNetwork.JoinRoom(joinCode);

        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(AnimationController.instance.FadeAnimation(AnimationController.instance.joinAnim, "IsFadeOut", AnimationController.instance.roomPanel, AnimationController.instance.joinPanel));
            StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "You have Joined Room " + roomName, "isFadeMenu"));
        }
    }        

    public void CancelJoinRoomOnClick()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        StartCoroutine(AnimationController.instance.FadeAnimation(AnimationController.instance.joinAnim, "IsFadeOut", AnimationController.instance.lobbyPanel, AnimationController.instance.joinPanel));
    }

    public void OpenJoinPanel()
    {
        StartCoroutine(AnimationController.instance.FadeAnimation(AnimationController.instance.lobbyAnim, "IsFadeOut", AnimationController.instance.joinPanel, AnimationController.instance.lobbyPanel));
    }    
}
