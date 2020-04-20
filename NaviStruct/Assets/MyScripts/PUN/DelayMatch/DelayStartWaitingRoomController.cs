using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayStartWaitingRoomController : MonoBehaviourPunCallbacks
{
    //This Object must be attached to an object in the waiting room scene

    //Photon view for sending rpc that updates the timer
    private PhotonView myPhotonView;
    //Estblishing an index for the start menu and waiting room scenes
    [SerializeField]
    private int multiplayerSceneIndex;
    [SerializeField]
    private int menuSceneIndex;
    //number of players in the room out of the total room size
    private int playerCount;
    private int roomSize;

    //Minimum amoutn of players needed to begin countdown
    [SerializeField]
    private int minPlayersToStart;
    //loading button to sync with scene scene
    ///[SerializeField]
    //private Button loader;

    //text variables for holding the displays for the countdown timer, player count and room number
    [SerializeField]
    private TextMeshProUGUI playerCountDisplay;
    [SerializeField]
    private TextMeshProUGUI timerToStartDisplay;    

    //bool values for if the timer can count down
    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;
    //countdown timer variables
    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;
    //countdown timer reset variables
    [SerializeField]
    private float maxWaitTime;
    [SerializeField]
    private float maxFullGameWaitTime;
    
    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;        

        PlayerCountUpdate();
    }
    
    void PlayerCountUpdate()
    {
        //updates player count when players join the room
        //displays player count
        //triggers countdown timer
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCountDisplay.text = playerCount + ":" + roomSize;

        if (playerCount == roomSize)
            readyToStart = true;
        else if (playerCount >= minPlayersToStart)
            readyToCountDown = true;
        else
        {
            readyToCountDown = false;
            readyToStart = false;
        }           
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //called whenever a new player joins the room
        PlayerCountUpdate();
        
        //send master clients countdown timer to all other players in order to sync time
        if (PhotonNetwork.IsMasterClient)        
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);           
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn, int roomNumber)
    {
        //RPC for syncing the countdown timer to those that join after it has started the countdown
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;        

        if (timeIn < fullGameTimer)
            fullGameTimer = timeIn;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Called whenever a player leaves the room
        PlayerCountUpdate();
    }

    private void Update()
    {
        WaitingForMorePlayers();
    }

    void WaitingForMorePlayers()
    {
        //If there is only one player in the room the timer will stop and reset
        if (playerCount <= 1)
            ResetTimer();
        //when there are enough (minimum) players in the room the start timer will begin counting down
        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }
        //format and display countdown timer
        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerToStartDisplay.text = tempTimer;
        //if the countdown timer reaches 0 the game will then start
        if(timerToStartGame <= 0f)
        {
            if (startingGame)
                return;
            StartGame();
        }
    }

    void ResetTimer()
    {
        //resets the count down timer
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    public void StartGame()
    {
        //Multiplayer scene is loaded to start the game
        startingGame = true;

        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        //StartCoroutine(LoadAsynchronously(multiplayerSceneIndex));
        Debug.Log("Starting Game");
    }

    /*private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        //PhotonNetwork.LoadLevel(sceneIndex);
        AsyncOperation operation = PhotonNetwork._AsyncLevelLoadingOperation;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loader.image.fillAmount = progress;            
            yield return null;
        }
    }*/

    public void Cancel()
    {
        //public function paired to cancel button in waiting room scene 
        StartCoroutine(DelayCancel());        
    }

    IEnumerator DelayCancel()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;

        SceneManager.LoadScene(menuSceneIndex);
    }
}
