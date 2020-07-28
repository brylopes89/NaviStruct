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
    private GameObject vrRig;
    [SerializeField]
    private GameObject arRig;
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

    private GameObject playground;
    private PhotonView pv;            
    private PlayerAvatarManager avatarManager;

    struct RigTransformController
    {
        public GameObject m_Playground;
        public GameObject m_Rig;
        public Vector3 m_WorldScale;         

        public void AssignObjects(GameObject rig, GameObject world)
        {            
            m_Rig = rig;
            m_Playground = world;
        }

        public void ApplyTransform(Vector3 worldScale)
        {
            m_WorldScale = worldScale;
            if(m_Playground)
                m_Playground.transform.localScale = m_WorldScale;            
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
        }
    }

    RigTransformController m_RigController;
    PlayerStates m_CurrentState;
    PlayerState m_PlayerState;

    private void Start()
    {        
        pv = MasterManager.ClassReference.Avatar.GetComponent<PhotonView>();
        avatarManager = MasterManager.ClassReference.Avatar.GetComponent<PlayerAvatarManager>();
        playground = MasterManager.ClassReference.Playground;

        AssignStateValues();               
        
        vr_ModeButton.onClick.AddListener(ChangeMode);
        ar_ModeButton.onClick.AddListener(ChangeMode);
    }

    private void AssignStateValues()
    {
        m_PlayerState.Initialize();
        immersiveWorldScale = Vector3.one;

        if (MasterManager.ClassReference.IsARSupport)
        {                 
            dioramaFloor.SetActive(false);
            isDiorama = true;
            m_CurrentState = PlayerStates.Diorama;
            dioramaWorldScale = new Vector3(0.004f, 0.004f, 0.004f);
            targetScale = dioramaWorldScale;
        }

        else
        {            
            if (MasterManager.ClassReference.IsVRSupport && vrRig != null)
            {
                m_RigController.AssignObjects(vrRig, playground);
                immersivePlayerPos = vrRig.transform.position;
                immersivePlayerRot = vrRig.transform.rotation;
            }               

            isDiorama = false;
            m_CurrentState = PlayerStates.Immersive;
            dioramaPlayerPos = playground.transform.forward * distance;
            dioramaWorldScale = new Vector3(0.01f, 0.01f, 0.01f);            
            targetScale = immersiveWorldScale;
        }

        m_RigController.ApplyTransform(targetScale);
        m_PlayerState.SetState(m_CurrentState);        
    }

    public void AssignARPlayground(GameObject newObj)
    {
        if (MasterManager.ClassReference.IsARSupport)
        {
            playground = newObj;
            m_RigController.AssignObjects(arRig, playground);    
            
            targetScale = dioramaWorldScale;
            m_RigController.ApplyTransform(targetScale);

            dioramaPlayerPos = playground.transform.forward * distance;
            immersivePlayerPos = Vector3.zero;
        }
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

        currentPlayerPos = m_RigController.m_Rig.transform.position;                  

        StartCoroutine(ChangePlayerPos(m_RigController.m_Rig.transform, currentPlayerPos, dioramaPlayerPos, duration));    
        
        if(m_RigController.m_Playground.activeInHierarchy)
            StartCoroutine(ChangeWorldScale(m_RigController.m_Playground.transform, immersiveWorldScale, dioramaWorldScale, duration));                
    }
    
    public void ImmersivePressed()
    {
        if (MasterManager.ClassReference.IsVRSupport)
            dioramaFloor.SetActive(false);

        currentPlayerPos = m_RigController.m_Rig.transform.position;

        if (m_RigController.m_Playground.activeInHierarchy)
            StartCoroutine(ChangeWorldScale(m_RigController.m_Playground.transform, dioramaWorldScale, immersiveWorldScale, duration));    
        
        StartCoroutine(ChangePlayerPos(m_RigController.m_Rig.transform, currentPlayerPos, immersivePlayerPos, duration));                                     
    }

    public void ResetPressed()
    {
        if (pv.IsMine)
        {
            currentPlayerPos = m_RigController.m_Rig.transform.position;
            currentPlayerRot = m_RigController.m_Rig.transform.rotation;
            if(isDiorama)
                StartCoroutine(ChangePlayerPos(m_RigController.m_Rig.transform, currentPlayerPos, dioramaPlayerPos, duration));           
            else
                StartCoroutine(ChangePlayerPos(m_RigController.m_Rig.transform, currentPlayerPos, immersivePlayerPos, duration));
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
