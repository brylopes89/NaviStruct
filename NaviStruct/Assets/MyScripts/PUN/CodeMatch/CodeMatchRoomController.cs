using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using TMPro;

public class CodeMatchRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex;

    [Header("Room Panel Display")]
    [SerializeField]
    private TextMeshProUGUI roomNameDisplay;    
    [SerializeField]
    private TMP_Text playerCount;
    [SerializeField]
    private GameObject enterButton;

    [Header("Player List Display")]
    [SerializeField]
    private Transform playerScrollList;   
    [SerializeField]
    private GameObject playerListPrefab;

    private AnimationController animController;
    private CodeMatchLobbyController lobbyController;

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;
        lobbyController = MasterManager.ClassReference.LobbyController; 
    }
    public override void OnJoinedRoom() //called when the local player joins the room
    {
        enterButton.SetActive(false);
        playerCount.text = "Players: " + PhotonNetwork.PlayerList.Length;
        roomNameDisplay.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient) //if master client then activate the start button
            enterButton.SetActive(true);
        else
            enterButton.SetActive(false);        

        ClearPlayerListings();// remove all old player listings
        ListPlayers();// relist all current player listings
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        StartCoroutine(animController.FadeText(animController.textAnim, message, "isFadeMenu"));
    }

    void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListPrefab, playerScrollList);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
        }
    }

    void ClearPlayerListings()
    {
        for (int i = playerScrollList.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(playerScrollList.GetChild(i).gameObject);
        }
    }    

    public override void OnPlayerEnteredRoom(Player newPlayer) //called whenever a new player enters the room
    {
        playerCount.text = "Players: " + PhotonNetwork.PlayerList.Length;
        StartCoroutine(animController.FadeText(animController.textAnim, "Player " + newPlayer.NickName + " has entered the room", "isFadeMenu"));

        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //called whenever a remote player leaves the room
    {
        playerCount.text = "Players: " + PhotonNetwork.PlayerList.Length;
        StartCoroutine(animController.FadeText(animController.textAnim, "Player " + otherPlayer.NickName + " has left the room", "isFadeMenu"));

        ClearPlayerListings();
        ListPlayers();

        if (PhotonNetwork.IsMasterClient)
            enterButton.SetActive(true);
    }

    public void StartGameOnClick()
    {
        if (PhotonNetwork.PlayerList.Length < lobbyController.roomSize)
        {
            StartCoroutine(animController.FadeText(animController.textAnim, 
                "Please wait for a total of " + lobbyController.roomSize + " Players to join this room.", "isFadeMenu"));
            return;
        }            

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
        StartCoroutine(animController.FadeText(animController.textAnim, "Loading Level", "isFadeMenu"));
    }

    public void CancelRoomOnClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList[i]);
            }
            StartCoroutine(LeaveRoom());
        }                    
    }

    IEnumerator LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;

        StartCoroutine(animController.FadeAnimation(animController.roomAnim, "IsFadeOut", animController.lobbyPanel, animController.roomPanel));
    }

    public override void OnLeftRoom()
    {
        playerCount.text = "Players: ";
    }
}
