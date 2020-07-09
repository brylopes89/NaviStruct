using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerStateManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private MonoBehaviour[] localScripts;
    [SerializeField]
    private GameObject[] localObjects;

    [SerializeField]
    private float stateChangeWaitTime;
    [SerializeField]
    private float SmoothingDelay = 5;

    private float stateChangeTimer = 3f;
    private bool isStateChange = false;

    private GameObject playground;   

    private Vector3 originalScale;
    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    struct PlayerController
    {
        /// <summary>
        /// The avatar that this state controls
        /// </summary>
        public GameObject m_Avatar;
        /// <summary>
        /// The object set as the avatar parent
        /// </summary>
        public Transform m_parentObject;
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

        public void SetParent(Transform parentObject)
        {
            m_parentObject = parentObject;

            if (m_parentObject != null)
                m_Avatar.transform.SetParent(m_parentObject.transform);
        }

        /// <summary>
        /// Starts state change, disabling photon view synchronization
        /// </summary>
        public void StateChangeBegin(bool changingStates)
        {
            if (m_PhotonView)
            {                
                //m_PhotonView.Synchronization = ViewSynchronization.Off;

                if (!changingStates)
                {
                    StateChangeEnd();
                }                    
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

        originalScale = this.transform.localScale;

        m_PlayerState.Initialize();
        m_PlayerState.SetGameObject(PlayerStates.Immersive, this.transform.gameObject);
        m_PlayerState.SetGameObject(PlayerStates.Diorama, this.transform.gameObject);

        m_CurrentState = PlayerStates.Immersive;
        m_PlayerState.SetState(m_CurrentState, false);
    }

    void Start()
    {
        if (m_CurrentState == PlayerStates.Immersive)
            this.transform.SetParent(playground.transform);
        else
            this.transform.SetParent(null);

        if (!photonView.IsMine)        
        {           
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        UpdateTimer();

        if (!photonView.IsMine)
        {
            if(m_CurrentState == PlayerStates.Immersive)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            }
        }
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

    private void UpdateTransform()
    {       
        if (isStateChange)
        {
            this.transform.SetParent(null);            
        }
        else
        {
            this.transform.SetParent(playground.transform);            
        }            
    }

    [PunRPC]
    public void ChangePlayerState(PlayerStates state, bool isChanging)
    {
        isStateChange = isChanging;
        m_CurrentState = state;

        m_PlayerState.SetState(m_CurrentState, isStateChange);
        UpdateTransform();
    }   

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {      
        if (stream.IsWriting)
        {            
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);     
            stream.SendNext(transform.localScale);
        }
        else
        {
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
        }               
    }
}
