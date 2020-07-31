using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEditor;
using Photon.Realtime;

public class ChangeModeController : MonoBehaviourPunCallbacks 
{
    #region Public Variables
    [SerializeField]
    private GameObject dioramaFloor;       
    [SerializeField]
    private Button vr_ModeButton;
    [SerializeField]
    private Button ar_ModeButton;

    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private float duration = 4f;
    [SerializeField]
    private float distance = 2f;
    #endregion   

    #region Transform Values
    private Vector3 dioramaWorldScale;
    private Vector3 dioramaPlayerPos;

    private Vector3 immersiveWorldScale;    
    private Vector3 immersivePlayerPos;
    
    private Vector3 currentPlayerPos;
    private Vector3 targetScale;
    private Vector3 targetPos;

    private Quaternion immersivePlayerRot;   
    private Quaternion currentPlayerRot;
    #endregion

    [HideInInspector]
    public bool isDiorama = false;

    private GameObject playerRig;
    private GameObject playground;
    private PlaygroundManager playground_Manager;
    private PhotonView pv;            
    private PlayerAvatarManager avatar_Manager;
    private XRMenuManager menu_Manager;

    public enum PlayerStates
    {
        Immersive = 0,
        Diorama = 1,
        MAX = 2,
    }
    struct PlayerState
    {
        public PlayerStates m_State;        

        public void Initialize()
        {
            m_State = PlayerStates.MAX;           
        }                

        public void SetState(PlayerStates nextState)
        {
            m_State = nextState;                
        }
    }

    PlayerStates m_CurrentState;
    PlayerState m_PlayerState;

    private void Start()
    {        
        pv = MasterManager.ClassReference.Avatar.GetComponent<PhotonView>();
        avatar_Manager = MasterManager.ClassReference.Avatar.GetComponent<PlayerAvatarManager>();
        playground = MasterManager.ClassReference.Playground;
        playground_Manager = playground.GetComponent<PlaygroundManager>();
        menu_Manager = this.GetComponentInParent<XRMenuManager>();
        playerRig = GameObject.FindWithTag("PlayerRig");

        AssignStateValues();               
        
        vr_ModeButton.onClick.AddListener(ChangeMode);
        ar_ModeButton.onClick.AddListener(ChangeMode);
    }

    private void AssignStateValues()
    {
        m_PlayerState.Initialize();
        immersiveWorldScale = Vector3.one;

        if (m_CurrentState == PlayerStates.Diorama)
        {
            if (MasterManager.ClassReference.IsARSupport)
                dioramaWorldScale = new Vector3(0.004f, 0.004f, 0.004f);
            else
                dioramaWorldScale = new Vector3(0.01f, 0.01f, 0.01f);
        }        

        if (MasterManager.ClassReference.IsARSupport)
        {                 
            dioramaFloor.SetActive(false);
            isDiorama = true;
            m_CurrentState = PlayerStates.Diorama;                 
        }
        else
        {            
            if (MasterManager.ClassReference.IsVRSupport && playerRig != null)
            {                
                immersivePlayerPos = playerRig.transform.position;
                immersivePlayerRot = playerRig.transform.rotation;
            }               

            isDiorama = false;
            m_CurrentState = PlayerStates.Immersive;
            dioramaPlayerPos = playground.transform.forward * distance;
            dioramaWorldScale = new Vector3(0.01f, 0.01f, 0.01f);            
            targetScale = immersiveWorldScale;
        }

        m_PlayerState.SetState(m_CurrentState);        
    }

    public void AssignARPlayground(GameObject newObj)
    {
        if (MasterManager.ClassReference.IsARSupport)
        {
            playground = newObj;              
            
            targetScale = dioramaWorldScale;        

            dioramaPlayerPos = playground.transform.forward * distance;
            immersivePlayerPos = Vector3.zero;
        }
    }

    private void ChangeMode()
    {        
        isDiorama = !isDiorama;

        menu_Manager.OpenInteractiveMenuOnClick(false);
        avatar_Manager.SetAvatarParent(true);

        if (pv.IsMine)
        {            
            if (isDiorama)
            {    
                m_CurrentState = PlayerStates.Diorama;
                DioramaPressed();

                if (MasterManager.ClassReference.IsVRSupport)
                {
                    foreach (TextMeshProUGUI childText in vr_ModeButton.GetComponentsInChildren<TextMeshProUGUI>())
                        childText.text = "IMMERSIVE";
                }                
            }
            else
            {
                m_CurrentState = PlayerStates.Immersive;
                ImmersivePressed();

                if (MasterManager.ClassReference.IsVRSupport)
                {
                    foreach (TextMeshProUGUI childText in vr_ModeButton.GetComponentsInChildren<TextMeshProUGUI>())
                        childText.text = "DIORAMA";
                }
            }
            m_PlayerState.SetState(m_CurrentState);            
        }       
    }

    public void DioramaPressed()
    {
        if(MasterManager.ClassReference.IsVRSupport)
            dioramaFloor.SetActive(true);

        currentPlayerPos = playerRig.transform.position;                  

        StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, dioramaPlayerPos, duration));    
        
        if(playground.activeInHierarchy)            
            StartCoroutine(playground_Manager.ChangeWorldScale(immersiveWorldScale, dioramaWorldScale, duration));                
    }
    
    public void ImmersivePressed()
    {
        if (MasterManager.ClassReference.IsVRSupport)
            dioramaFloor.SetActive(false);

        currentPlayerPos = playerRig.transform.position;

        if (playground.activeInHierarchy)
            StartCoroutine(playground_Manager.ChangeWorldScale(dioramaWorldScale, immersiveWorldScale, duration));    
        
        StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, immersivePlayerPos, duration));                                     
    }

    public void ResetPressed()
    {
        if (pv.IsMine)
        {
            currentPlayerPos = playerRig.transform.position;
           
            if(isDiorama)
                StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, dioramaPlayerPos, duration));           
            else
                StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, immersivePlayerPos, duration));
        }               
    }

    private IEnumerator ChangePlayerPos(Transform target, Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            target.position = Vector3.Lerp(a, b, i);

            yield return null;
        }        
    }
}
