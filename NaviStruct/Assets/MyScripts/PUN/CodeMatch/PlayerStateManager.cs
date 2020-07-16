using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerStateManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private MonoBehaviour[] localScripts;
    [SerializeField]
    private GameObject[] localObjects;

    [SerializeField]
    private GameObject avatarMesh;
    [SerializeField]
    private float stateChangeWaitTime;    

    private float stateChangeTimer = 3f;
    private bool isStateChange = false;   

    private GameObject playground;

    struct PlayerController
    {
        /// <summary>
        /// The avatar that this state controls
        /// </summary>
        public GameObject m_Avatar;
        /// <summary>
        /// The object set as the avatar parent
        /// </summary>
        public Transform m_parentRig;
        /// <summary>
        /// The Photon View component associated with the avatar
        /// </summary>
        public PhotonView m_PhotonView;

        /// <summary>
        /// When passed an avatar and parent object, this function will assign the avatar the appropriate parent and get the avatars photon view component.     
        /// </summary>
        /// <param name="avatar">The local players avatar that contains the photon view component</param>
        public void Attach(GameObject avatar)
        {
            m_Avatar = avatar;            
            
            if(m_Avatar != null)
            {
                m_PhotonView = m_Avatar.GetComponent<PhotonView>();                
            }                             
        }

        /// <summary>
        /// Starts state change, disabling photon view synchronization
        /// </summary>
        public void StateChangeBegin(bool changingStates)
        {
            if (m_PhotonView)
            {                
                if (changingStates)
                    m_Avatar.transform.SetParent(null);
                else
                    m_Avatar.transform.SetParent(GameObject.Find("Playground").transform);
            }            
        }

        /// <summary>
        /// Ends state change, enabling photon view synchronization
        /// </summary>
        public void StateChangeEnd()
        {
            if (m_PhotonView)
            {                   
                //m_PhotonView.Synchronization = ViewSynchronization.UnreliableOnChange;
            }
        }       
    }

    public enum PlayerStates
    {
        /// <summary>
        /// the Immersive state represents the player in the model
        /// </summary>
        Immersive = 0,
        /// <summary>
        /// the Diorama state represents the player standing outside the dioramic model
        /// </summary>
        Diorama = 1,
        /// <summary>
        /// Maximum sentinel
        /// </summary>
        MAX = 2,
    }

    struct PlayerState
    {
        public PlayerStates m_State;
        PlayerController[] m_Players;       

        /// <summary>
        /// Sets up the player state
        /// </summary>
        public void Initialize()
        {
            m_State = PlayerStates.MAX;
            m_Players = new PlayerController[(int)PlayerStates.MAX];            
        }

        /// <summary>
        /// Attaches the appropriate parent object to the local avatar for each respective state.
        /// </summary>
        /// <param name="state">The state that we're attaching the game object to</param>
        /// <param name="avatar">Represents the avatar we will scrape components from.</param>
        public void SetGameObject(PlayerStates state, GameObject avatar)
        {
            if ((state == PlayerStates.MAX) || (m_Players == null))
                return;                

            m_Players[(int)state].Attach(avatar);
        }

        /// <summary>
        /// Attempts to set the current state of the player.
        /// </summary>
        /// <param name="nextState">The state that we wish to transition to</param>
        public void SetState(PlayerStates nextState, bool changingStates)
        {           
            m_State = nextState;
            m_Players[(int)m_State].StateChangeBegin(changingStates);            
        }
    }

    PlayerStates m_CurrentState;
    PlayerState m_PlayerState;

    private void Awake()
    {
        PhotonNetwork.SendRate = 40; //20
        PhotonNetwork.SerializationRate = 40; //10        

        playground = GameObject.Find("Playground");
        this.transform.SetParent(playground.transform);

        m_PlayerState.Initialize();
        m_PlayerState.SetGameObject(PlayerStates.Immersive, this.transform.gameObject);
        m_PlayerState.SetGameObject(PlayerStates.Diorama, this.transform.gameObject);

        m_CurrentState = PlayerStates.Immersive;
        m_PlayerState.SetState(m_CurrentState, false);
    }

    void Start()
    {            
        if (!photonView.IsMine)        
        {
            if(avatarMesh != null)
                avatarMesh.SetActive(true);
           
            //for (int i = 0; i < localScripts.Length; i++)
            //{
            //    localScripts[i].enabled = false;
            //}
            //for (int i = 0; i < localObjects.Length; i++)
            //{
            //    localObjects[i].SetActive(false);
            //}
        }
        else
        {
            if (avatarMesh != null)
                avatarMesh.SetActive(false);         
        }
    }

    private void Update()
    {
        UpdateTimer();       
    }

    private void UpdateTimer()
    {
        if (isStateChange)
        {
            stateChangeTimer -= Time.deltaTime;

            if (stateChangeTimer <= 0)
            {
                stateChangeTimer = stateChangeWaitTime;               
                photonView.RPC("ChangePlayerState", RpcTarget.All, m_CurrentState, false);
            }
        }
    }      

    [PunRPC]
    public void ChangePlayerState(PlayerStates state, bool isChanging)
    {
        isStateChange = isChanging;
        m_CurrentState = state;

        if (photonView.IsMine)        
            m_PlayerState.SetState(m_CurrentState, isStateChange);        
       
    }   
}
