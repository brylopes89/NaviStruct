using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeMatchLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject lobbyConnectButton;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private InputField playerNameInput;

    private string roomName;
    private int roomSize;

    [SerializeField]
    private GameObject createPanel;
    [SerializeField]
    private TMP_InputField codeDisplay;
    [SerializeField]
    private TMP_InputField codeInputField;
    
    [SerializeField]
    private GameObject joinPanel;
    private string joinCode;
    [SerializeField]
    private GameObject JoinButton;

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
            PhotonNetwork.NickName = "Player"  + Random.Range(0, 1000);

        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdateInputChanged(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobbyOnClick()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public void OnRoomSizeInputChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void CreateRoomOnClick()
    {
        createPanel.SetActive(true);
        Debug.Log("Creating Room Now");

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);
        roomName = roomCode.ToString();
        PhotonNetwork.CreateRoom(roomName, roomOps);

        codeDisplay.text = roomName;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed. Try using a different name");

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);
        roomName = roomCode.ToString();
        PhotonNetwork.CreateRoom(roomName, roomOps);

        codeDisplay.text = roomName;
    }

    public void CancelRoomOnClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList[i]);
            }
        }

        PhotonNetwork.LeaveRoom();
        createPanel.SetActive(false);
        JoinButton.SetActive(true);
    }

    public void OpenJoinPanel()
    {
        joinPanel.SetActive(true);
    }

    public void CodeInput(string code)
    {
        joinCode = code;
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(joinCode);
    }

    public void LeaveRoomOnClick()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        JoinButton.SetActive(true);
        joinPanel.SetActive(false);
        codeInputField.text = "";
    }

    public void MatchMakingCancelOnClick()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }
}
