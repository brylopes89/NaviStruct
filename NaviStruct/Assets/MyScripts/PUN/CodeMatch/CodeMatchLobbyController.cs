using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CodeMatchLobbyController : MonoBehaviourPunCallbacks
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

    private string roomName;
    private int roomSize;        

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyConnectButton.SetActive(true);

        //check for player name saved to player prefs
        if (PlayerPrefs.HasKey("NickName"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
            else
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
        }
        else
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);

        playerNameInput.text = PhotonNetwork.NickName;
        AnimationController.animController.updateText.text = "We are now connected to the " + PhotonNetwork.CloudRegion + "server!";
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Joined Lobby"));        
    }

    public void PlayerNameUpdateInputChanged(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobbyOnClick()
    {        
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.mainAnim, "IsFadeOut", AnimationController.animController.lobbyPanel, AnimationController.animController.mainPanel));
        PhotonNetwork.JoinLobby();
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
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Creating Room Now"));

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);        

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();
        else
            roomName = codeCreateInputField.text;

        PhotonNetwork.CreateRoom(roomName, roomOps);              
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Created Room Successfully"));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, AnimationController.animController.updateText, true, "Tried to create a new room but failed. Try using a different name"));
        
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();
        else
            roomName = codeCreateInputField.text;

        PhotonNetwork.CreateRoom(roomName, roomOps);        
    }

    public void OpenJoinPanel()
    {
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.lobbyAnim, "IsFadeOut", AnimationController.animController.joinPanel, AnimationController.animController.lobbyPanel));              
    }

    public void MatchMakingCancelOnClick()
    {
        PhotonNetwork.LeaveLobby();
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.lobbyAnim, "IsFadeOut", AnimationController.animController.mainPanel, AnimationController.animController.lobbyPanel));            
    }
}
