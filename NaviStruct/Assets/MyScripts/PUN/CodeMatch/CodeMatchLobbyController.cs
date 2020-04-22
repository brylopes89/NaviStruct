using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CodeMatchLobbyController : MonoBehaviourPunCallbacks
{
    //start menu screen
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private TMP_InputField playerNameInput;

    //lobby create/join room screen
    [SerializeField]
    private GameObject lobbyConnectButton;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private TMP_InputField roomSizeInputField;
    [SerializeField]
    private TMP_InputField codeCreateInputField;

    private string roomName;
    private int roomSize;

    //room display screen
    [SerializeField]
    private GameObject createPanel;
    [SerializeField]
    private TMP_Text codeDisplay;
    [SerializeField]
    private TMP_InputField codeInputField;
    
    [SerializeField]
    private GameObject joinPanel;
    private string joinCode;
    [SerializeField]
    private GameObject JoinButton;

    private Animator roomAnim;
    private Animator buttonAnim;

    private void Awake()
    {
        roomAnim = mainPanel.GetComponent<Animator>();
        buttonAnim = codeCreateInputField.gameObject.GetComponent<Animator>();
    }

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
        StartCoroutine(FadeAnimation(roomAnim, "IsFadeOut"));
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        PhotonNetwork.JoinLobby();
    }

    public void OnRoomSizeInputChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    private IEnumerator FadeAnimation(Animator anim, string animBoolString)
    {
        anim.SetBool(animBoolString, true);        

        yield return new WaitForSeconds(1f);
        
        anim.SetBool(animBoolString, false);
    }

    public void CreateRoomOnClick()
    {
        StartCoroutine(FadeAnimation(roomAnim, "IsFadeOut"));
        lobbyPanel.SetActive(false);
        createPanel.SetActive(true);
        
        Debug.Log("Creating Room Now");

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);

        if(codeCreateInputField.textComponent.text != null)
            roomName = "Room: " + codeCreateInputField.textComponent.text;
        else
            roomName = "Room: " + roomCode.ToString();

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

        codeDisplay.text = "Room Number: " + roomName;
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
