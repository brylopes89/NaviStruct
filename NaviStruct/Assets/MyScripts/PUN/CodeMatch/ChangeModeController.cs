using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    private float duration = 4f;
    [SerializeField]
    private float distance = 3f;
    #endregion

    #region Transform Values
    private Vector3 dioramaWorldScale;
    private Vector3 dioramaPlayerPos;     
    private Vector3 immersivePlayerPos;
    
    private Vector3 currentPlayerPos;
    private Vector3 targetScale;
    #endregion
    
    private bool isDiorama = false;
    public bool IsDiorama { get { return isDiorama; } private set { isDiorama = value; } }
    
    private GameObject playground;
    private PlaygroundManager pg_Manager;

    private GameObject avatar;
    private PhotonView pv;
    private PlayerStateManager state_Manager; 

    private RigTransformManager rig_Manager;
    private XRMenuManager menu_Manager;     

    private void Start()
    {
        avatar = MasterManager.ClassReference.Avatar;
        pv = avatar.GetComponent<PhotonView>();       
        state_Manager = avatar.GetComponent<PlayerStateManager>();        

        rig_Manager = MasterManager.ClassReference.PlayerRig.GetComponent<RigTransformManager>();

        playground = MasterManager.ClassReference.Playground;
        pg_Manager = playground.GetComponent<PlaygroundManager>();
        menu_Manager = this.GetComponentInParent<XRMenuManager>();        

        AssignTransformValues();
        HandleInitialState();

        vr_ModeButton.onClick.AddListener(HandleModeChange);
        ar_ModeButton.onClick.AddListener(HandleModeChange);             
    }

    private void AssignTransformValues()
    {     
        if (MasterManager.ClassReference.IsARSupport)
        {
            isDiorama = true;
            dioramaFloor.SetActive(false);

            dioramaWorldScale = new Vector3(0.004f, 0.004f, 0.004f);
            dioramaPlayerPos = avatar.transform.position;
            immersivePlayerPos = playground.transform.position + Vector3.up;
            targetScale = dioramaWorldScale;            
        }
        else
        {
            isDiorama = false;
            
            dioramaWorldScale = new Vector3(0.01f, 0.01f, 0.01f);
            dioramaPlayerPos = playground.transform.position + playground.transform.forward * distance;            
            immersivePlayerPos = avatar.transform.position;
            targetScale = Vector3.one;
        }
        
        pg_Manager.ApplyInitialScale(targetScale);
    }

    private void HandleInitialState()
    {
        if (isDiorama)
            state_Manager.SetPlayerState(PlayerStates.Diorama);
        else
            state_Manager.SetPlayerState(PlayerStates.Immersive);
    }

    private void HandleModeChange()
    {        
        isDiorama = !isDiorama;       
        menu_Manager.OpenInteractiveMenuOnClick();
        state_Manager.SetNextState(true);        

        if (pv.IsMine)
        {
            if (isDiorama)
            {                
                OnDioramaClicked();

                if (MasterManager.ClassReference.IsVRSupport)
                {
                    foreach (TextMeshProUGUI childText in vr_ModeButton.GetComponentsInChildren<TextMeshProUGUI>())
                        childText.text = "IMMERSIVE";
                }                
            }
            else
            {                
                OnImmersiveClicked();

                if (MasterManager.ClassReference.IsVRSupport)
                {
                    foreach (TextMeshProUGUI childText in vr_ModeButton.GetComponentsInChildren<TextMeshProUGUI>())
                        childText.text = "DIORAMA";
                }
            }                   
        }     
    }

    public void OnDioramaClicked()
    {
        if(MasterManager.ClassReference.IsVRSupport)
            dioramaFloor.SetActive(true);

        currentPlayerPos = rig_Manager.gameObject.transform.position;    

        StartCoroutine(rig_Manager.ChangeRigPosition(currentPlayerPos, dioramaPlayerPos, duration));    
        
        if(playground.activeInHierarchy)            
            StartCoroutine(pg_Manager.ChangeWorldScale(playground.transform.localScale, dioramaWorldScale, duration));                
    }
    
    public void OnImmersiveClicked()
    {
        if (MasterManager.ClassReference.IsVRSupport)
            dioramaFloor.SetActive(false);

        currentPlayerPos = rig_Manager.gameObject.transform.position;

        if (playground.activeInHierarchy)
            StartCoroutine(pg_Manager.ChangeWorldScale(dioramaWorldScale, Vector3.one, duration));            
       
        StartCoroutine(rig_Manager.ChangeRigPosition(currentPlayerPos, immersivePlayerPos, duration));
    }

    public void OnResetClicked()
    {
        if (pv.IsMine)
        {
            currentPlayerPos = rig_Manager.gameObject.transform.position;
           
            if(isDiorama)
                StartCoroutine(rig_Manager.ChangeRigPosition(currentPlayerPos, dioramaPlayerPos, duration));
            else
                StartCoroutine(rig_Manager.ChangeRigPosition(currentPlayerPos, immersivePlayerPos, duration));
        }               
    }
}
