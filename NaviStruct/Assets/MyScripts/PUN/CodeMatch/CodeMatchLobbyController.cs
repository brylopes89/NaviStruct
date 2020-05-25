﻿using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using VRKeyboard.Utils;
using UnityEngine.XR;

public class CodeMatchLobbyController : MonoBehaviourPunCallbacks
{
    #region Display Variables
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
    #endregion

    private AnimationController animController;
    private KeyboardManager keyboardManager;
    private List<RoomInfo> roomListings;

    [HideInInspector]
    public string roomName; 
    [HideInInspector]
    public int roomSize;

    public override void OnEnable()
    {
        base.OnEnable();
        if (MasterManager.ClassReference.LobbyController == null)
            MasterManager.ClassReference.LobbyController = this;
    }

    public override void OnConnectedToMaster()
    {        
        animController = MasterManager.ClassReference.AnimController;        

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

    #region InputFields
    public void OnPlayerNameInput(string nameInput)
    {    
        PhotonNetwork.NickName = nameInput;          
        PlayerPrefs.SetString("NickName", nameInput);
    }    

    public void OnRoomSizeInput(string sizeIn)
    {        
        roomSize = int.Parse(sizeIn);        
    }

    public void OnRoomNameInput(string nameIn)
    {        
        roomName = nameIn;       
    }

    public void CodeInput(string code)
    {        
        joinCode = code;        
    }
    #endregion

    #region Lobby OnClick Events    
    /// <summary>  
    /// Buttons events within the Main and Lobby panels
    /// </summary>
    public void JoinLobbyOnClick() //Joins lobby from Player name input screen
    {
        StartCoroutine(animController.FadeAnimation(animController.mainAnim, "IsFadeOut", animController.lobbyPanel, animController.mainPanel));
        PhotonNetwork.JoinLobby();
    } 
    public void OpenJoinPanelOnClick() //Opens Join Room Panel from lobby
    {
        StartCoroutine(animController.FadeAnimation(animController.lobbyAnim, "IsFadeOut", animController.joinPanel, animController.lobbyPanel));
    }    
    public void CreateRoomOnClick() //Creates custom room from lobby
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();
        if (string.IsNullOrEmpty(roomSizeInputField.text))
        {
            StartCoroutine(animController.FadeText(animController.textAnim, "Please enter a room size", "isFadeMenu"));
            return;
        }
        
       
        StartCoroutine(animController.FadeAnimation(animController.lobbyAnim, "IsFadeOut", animController.roomPanel, animController.lobbyPanel));        

        PhotonNetwork.CreateRoom(roomName, roomOps);
    }   
    public void MatchMakingCancelOnClick() //Cancels lobby session and returns to start menu
    {
        StartCoroutine(animController.FadeAnimation(animController.lobbyAnim, "IsFadeOut", animController.mainPanel, animController.lobbyPanel));
        StartCoroutine(animController.FadeText(animController.textAnim, "MatchMaking cancelled. Leaving Lobby", "isFadeMenu"));
        PhotonNetwork.LeaveLobby();
    }
    #endregion

    #region JoinRoom OnClick Events
    /// <summary>  
    /// Buttons events within the Join Room Panel
    /// </summary>
    public void EnterRoomOnClick()
    {
        if (PhotonNetwork.PlayerList.Length == roomSize)
        {
            StartCoroutine(animController.FadeText(animController.textAnim, "Room " + roomName + "is full. Please try another room.", "isFadeMenu"));
            return;
        }

        PhotonNetwork.JoinRoom(joinCode);

        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(animController.FadeAnimation(animController.joinAnim, "IsFadeOut", animController.roomPanel, animController.joinPanel));
            StartCoroutine(animController.FadeText(animController.textAnim, "You have Joined Room " + roomName, "isFadeMenu"));
        }
    }

    public void CancelJoinRoomOnClick()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        StartCoroutine(animController.FadeAnimation(animController.joinAnim, "IsFadeOut", animController.lobbyPanel, animController.joinPanel));
    }
    #endregion

    #region Room Listing Updates
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
    #endregion
}
