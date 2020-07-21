using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;

public class ChangeModeController : MonoBehaviourPunCallbacks 
{
    #region Public Variables
    [SerializeField]
    private GameObject dioramaFloor;
    [SerializeField]
    private GameObject playground;
    [SerializeField]
    private GameObject playerRig;
    [SerializeField]
    private Button modeButton;
    
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

    private PhotonView pv;            
    private PlayerAvatarManager avatarManager;

    struct StateController
    {
        public GameObject m_Playground;
        public GameObject m_Rig;

        public Vector3 m_WorldScale;
        public Vector3 m_PlayerPos;
        public Quaternion m_PlayerRot;

        public void AssignObject(GameObject rig, GameObject world)
        {
            m_Rig = rig;
            m_Playground = world;
        }

        public void ApplyTransform(Vector3 worldScale, Vector3 playerPos)
        {
            m_WorldScale = worldScale;
            m_PlayerPos = playerPos;

            m_Playground.transform.localScale = m_WorldScale;
            m_Rig.transform.position = m_PlayerPos;
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

        public void Initialize()
        {
            m_State = PlayerStates.MAX;           
        }                

        public void SetState(PlayerStates nextState)
        {
            m_State = nextState;     
            Debug.Log(m_State);
        }
    }

    StateController m_StateController;
    PlayerStates m_CurrentState;
    PlayerState m_PlayerState;

    private void Start()
    {        
        pv = MasterManager.ClassReference.Avatar.GetComponent<PhotonView>();
        avatarManager = MasterManager.ClassReference.Avatar.GetComponent<PlayerAvatarManager>();

        SetTransformValues();
        AssignStateValues();
        
        modeButton.onClick.AddListener(ChangeMode);
    }

    private void SetTransformValues()
    {
        immersivePlayerPos = playerRig.transform.position;
        immersivePlayerRot = playerRig.transform.rotation;
        immersiveWorldScale = playground.transform.localScale;

        dioramaWorldScale = new Vector3(0.01f, 0.01f, 0.01f);
        dioramaPlayerPos = playground.transform.forward * distance;
    }
    private void AssignStateValues()
    {
        m_PlayerState.Initialize();
        m_StateController.AssignObject(playerRig, playground);

        if (XRSettings.enabled && !MasterManager.ClassReference.IsVRSupport)
        {
            isDiorama = true;
            m_CurrentState = PlayerStates.Diorama;
            targetScale = dioramaWorldScale;
            targetPos = dioramaPlayerPos;
        }
        else
        {
            isDiorama = false;
            m_CurrentState = PlayerStates.Immersive;
            targetScale = immersiveWorldScale;
            targetPos = immersivePlayerPos;
        }

        m_StateController.ApplyTransform(targetScale, targetPos);
        m_PlayerState.SetState(m_CurrentState);
    }

    private void ChangeMode()
    {        
        isDiorama = !isDiorama;
        
        avatarManager.SetAvatarParent(true);

        if (pv.IsMine)
        {            
            if (isDiorama)
            {    
                m_CurrentState = PlayerStates.Diorama;
                DioramaPressed();
                foreach (TextMeshProUGUI childText in modeButton.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    childText.text = "IMMERSIVE";
                }
            }
            else
            {
                m_CurrentState = PlayerStates.Immersive;
                ImmersivePressed();
                foreach (TextMeshProUGUI childText in modeButton.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    childText.text = "DIORAMA";
                }
            }

            m_PlayerState.SetState(m_CurrentState);
        }       
    }

    public void DioramaPressed()
    {                     
        dioramaFloor.SetActive(true);
        currentPlayerPos = playerRig.transform.position;
        currentPlayerRot = playerRig.transform.rotation;            

        StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, dioramaPlayerPos, duration));           
        StartCoroutine(ChangeWorldScale(playground.transform, immersiveWorldScale, dioramaWorldScale, duration));                
    }
    
    public void ImmersivePressed()
    {        
        dioramaFloor.SetActive(false);
        currentPlayerPos = playerRig.transform.position;           

        StartCoroutine(ChangeWorldScale(playground.transform, dioramaWorldScale, immersiveWorldScale, duration));            
        StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, immersivePlayerPos, duration));                                     
    }

    public void ResetPressed()
    {
        if (pv.IsMine)
        {
            currentPlayerPos = playerRig.transform.position;
            currentPlayerRot = playerRig.transform.rotation;
            if(isDiorama)
                StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, dioramaPlayerPos, duration));           
            else
                StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, immersivePlayerPos, duration));
        }               
    }

    private IEnumerator ChangeWorldScale(Transform target, Vector3 startPos, Vector3 desiredPos, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;        

        while (i < 1)
        {            
            i += Time.deltaTime * rate;            
            target.localScale = Vector3.Lerp(startPos, desiredPos, i);                      

            yield return null;
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

    private IEnumerator ChangePlayerRot(Quaternion a, Quaternion b, float time, Quaternion objectRotation)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            objectRotation = Quaternion.Slerp(a, b, i);

            yield return null;
        }
    }

}
