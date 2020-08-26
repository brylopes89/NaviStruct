using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
    private QuickStartController quickStartController;

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;
        lobbyController = MasterManager.ClassReference.LobbyController;
        quickStartController = FindObjectOfType<QuickStartController>();
    }
    public override void OnJoinedRoom() //called when the local player joins the room
    {
        if (quickStartController.quickSelected)
        {
            if (PhotonNetwork.CountOfRooms > 0)
                StartCoroutine(animController.FadeMenuPanels(animController.lobbyAnim, "IsFadeOut", animController.customRoomPanel, animController.lobbyPanel));
            else
                StartGameOnClick();

            StartCoroutine(QuickStartSelected());
        }

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
        StartCoroutine(animController.FadeStatusText("Player " + newPlayer.NickName + " has entered the room"));

        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //called whenever a remote player leaves the room
    {
        playerCount.text = "Players: " + PhotonNetwork.PlayerList.Length;
        StartCoroutine(animController.FadeStatusText("Player " + otherPlayer.NickName + " has left the room"));

        ClearPlayerListings();
        ListPlayers();

        if (PhotonNetwork.IsMasterClient)
            enterButton.SetActive(true);
    }

    public void StartGameOnClick()
    {
        if (PhotonNetwork.PlayerList.Length < lobbyController.roomSize)
        {
            StartCoroutine(animController.FadeStatusText("Please wait for a total of " + lobbyController.roomSize + " Players to join this room."));
            return;
        }            

        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
        StartCoroutine(animController.FadeStatusText("Loading Level"));
    }

    public void CancelCustomRoomOnClick()
    {
        PhotonNetwork.LeaveRoom();
        StartCoroutine(animController.FadeMenuPanels(animController.customRoomAnim, "IsFadeOut", animController.lobbyPanel, animController.customRoomPanel));

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 0)
        {           
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
        }        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log(newMasterClient.NickName);
    }


    IEnumerator QuickStartSelected()
    {
        yield return new WaitForSeconds(2f);
        quickStartController.quickSelected = false;
    }

    IEnumerator LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        
        while (PhotonNetwork.InRoom)
            yield return null;

        StartCoroutine(animController.FadeMenuPanels(animController.customRoomAnim, "IsFadeOut", animController.lobbyPanel, animController.customRoomPanel));
    }

    public override void OnLeftRoom()
    {
        playerCount.text = "Players: " + PhotonNetwork.PlayerList.Length;

        if (PhotonNetwork.PlayerList.Length < 0)
        {
            Debug.Log("Player list is less than 0");
            PhotonNetwork.CurrentRoom.RemovedFromList = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }
}
