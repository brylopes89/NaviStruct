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
    private GameObject mainPanel;
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField]
    private GameObject lobbyConnectButton;    

    [Header("Lobby/Create Display")]    
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private TMP_InputField roomSizeInputField;
    [SerializeField]
    private TMP_InputField codeCreateInputField;    

    [Header("Custom Room Display")]
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private TMP_Text codeDisplay;    
    
    [Header("Join Panel Display")]
    [SerializeField]
    private GameObject joinPanel;    
    [SerializeField]
    private TMP_InputField codeInputField;
    [SerializeField]
    private GameObject joinButton;

    private string roomName;
    private int roomSize;
    private string joinCode;

    private Animator lobbyAnim;
    private Animator mainAnim;
    private Animator joinAnim;
    private Animator createAnim;

    private void Awake()
    {
        lobbyAnim = lobbyPanel.GetComponent<Animator>();
        mainAnim = mainPanel.GetComponent<Animator>();
        joinAnim = joinPanel.GetComponent<Animator>();
        createAnim = roomPanel.GetComponent<Animator>();
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
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);

        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdateInputChanged(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobbyOnClick()
    {
        StartCoroutine(FadeAnimation(mainAnim, "IsFadeOut", lobbyPanel, mainPanel));        

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
        //if (!PhotonNetwork.IsConnected)
        //return;
        StartCoroutine(FadeAnimation(lobbyAnim, "IsFadeOut", roomPanel, lobbyPanel));

        Debug.Log("Creating Room Now");

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);        

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();        

        PhotonNetwork.CreateRoom(roomName, roomOps, TypedLobby.Default);
        codeDisplay.text = "Room: " + roomName;        
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully.");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed. Try using a different name");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        int roomCode = Random.Range(1000, 10000);

        if (string.IsNullOrEmpty(codeCreateInputField.text))
            roomName = roomCode.ToString();
        else
            roomName = codeCreateInputField.text;

        PhotonNetwork.CreateRoom(roomName, roomOps);
        codeDisplay.text = "Room: " + roomName;
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
        StartCoroutine(FadeAnimation(lobbyAnim, "IsFadeOut", lobbyPanel, roomPanel));
        StartCoroutine(DisconnectAndLoad());                
    }

    public void OpenJoinPanel()
    {
        StartCoroutine(FadeAnimation(lobbyAnim, "IsFadeOut", joinPanel, lobbyPanel));        
    }

    public void CodeInput(string code)
    {
        joinCode = code;
    }

    public void JoinRoomOnClick()
    {
        StartCoroutine(FadeAnimation(joinAnim, "IsFadeOut", roomPanel, joinPanel));
        PhotonNetwork.JoinRoom(joinCode);
        
    }

    public void LeaveRoomOnClick()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();            
        }

        StartCoroutine(FadeAnimation(joinAnim, "IsFadeOut", lobbyPanel, joinPanel));
    }

    public override void OnLeftRoom()
    {        
        StartCoroutine(FadeAnimation(lobbyAnim, "IsFadeOut", joinButton, joinPanel));
        
        codeInputField.text = "";
    }

    public void MatchMakingCancelOnClick()
    {
        PhotonNetwork.LeaveLobby();
        StartCoroutine(FadeAnimation(lobbyAnim, "IsFadeOut", mainPanel, lobbyPanel));     
    }

    private IEnumerator FadeAnimation(Animator anim, string animBoolString, GameObject activePanel, GameObject inactivePanel)
    {
        anim.SetBool(animBoolString, true);

        yield return new WaitForSeconds(.7f);

        anim.SetBool(animBoolString, false);
        activePanel.SetActive(true);
        inactivePanel.SetActive(false);
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        StartCoroutine(FadeAnimation(createAnim, "IsFadeOut", mainPanel, lobbyPanel));
    }
}
