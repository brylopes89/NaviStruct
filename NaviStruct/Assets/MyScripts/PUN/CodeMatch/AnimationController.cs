using Photon.Pun;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRKeyboard.Utils;

public class AnimationController : MonoBehaviourPunCallbacks
{   
    [Header("Menu Displays")]
    public GameObject lobbyPanel;
    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject roomPanel;
    public GameObject keyboard;
    public GameObject interactiveMenu;

    [Header("Text Display Controller")]
    public GameObject statusTextController;
    public GameObject tipTextController;

    [Header("Animators")]         
    public Animator avatarAnim;    
    public Animator lobbyAnim;    
    public Animator mainAnim;    
    public Animator joinAnim;    
    public Animator roomAnim;
    public Animator keyboardAnim;
    public Animator statusTextAnim;
    public Animator tipTextAnim;
    public Animator interactiveMenuAnim;

    private TextMeshProUGUI statusText;
    private TextMeshProUGUI tipText;

    private Scene scene;    
    private string currentAnimation = "";

    public override void OnEnable()
    {
        base.OnEnable();

        if (MasterManager.ClassReference.AnimController == null)
            MasterManager.ClassReference.AnimController = this;

        scene = SceneManager.GetActiveScene();        
    }

    private void Start()
    {
        AssignObjectReferences();        
    }

    private void AssignObjectReferences()
    {
        if (scene.buildIndex == 0)
        {
            lobbyAnim = lobbyPanel.GetComponent<Animator>();
            mainAnim = mainPanel.GetComponent<Animator>();
            joinAnim = joinPanel.GetComponent<Animator>();
            roomAnim = roomPanel.GetComponent<Animator>();            
            statusTextAnim = statusTextController.GetComponent<Animator>();
            keyboardAnim = keyboard.GetComponentInChildren<Animator>();
            statusTextAnim = statusTextController.GetComponent<Animator>();
            tipTextAnim = tipTextController.GetComponent<Animator>();

            statusText = statusTextController.GetComponentInChildren<TextMeshProUGUI>();
            tipText = tipTextController.GetComponentInChildren<TextMeshProUGUI>();       
        }
        else
        {
            avatarAnim = MasterManager.ClassReference.Avatar.GetComponent<Animator>();            
            interactiveMenuAnim = interactiveMenu.GetComponentInChildren<Animator>();            
        }
    }

    public IEnumerator FadeMenuPanels(Animator anim, string boolName, GameObject activePanel, GameObject inactivePanel)
    {
        anim.SetBool(boolName, true);

        yield return new WaitForSeconds(.7f);

        anim.SetBool(boolName, false);
        activePanel.SetActive(true);
        inactivePanel.SetActive(false);
    }

    public IEnumerator FadeStatusText(Animator anim, string statusDisplayText, string animBoolName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"))
        {
            statusTextAnim.SetBool(animBoolName, true);
            yield return new WaitForSeconds(1.3f);
        }

        if(statusDisplayText != null)
        {           
            statusText.text = statusDisplayText;
            yield return new WaitForEndOfFrame();                        
        }        

        statusTextAnim.SetBool(animBoolName, false);
        yield return new WaitForSeconds(1.3f);
        anim.SetBool(animBoolName, true);
        yield return null;
    }

    public IEnumerator FadeCanvas(GameObject go, Animator anim, string boolName, bool isEnable)
    {
        if (isEnable)
        {
            go.SetActive(true);
            yield return new WaitForEndOfFrame();
            anim.SetBool(boolName, false);            
        }
        else
        {
            anim.SetBool(boolName, true);
            yield return new WaitForSeconds(.5f);
            go.SetActive(false);            
        }
        yield return null;
    }

    public void SetStandaloneAvatarAnimation(string animationName)
    {
        if (currentAnimation != "")
        {
            avatarAnim.SetBool(currentAnimation, false);
        }
        avatarAnim.SetBool(animationName, true);
        currentAnimation = animationName;
    }

    public void SetAvatarFloatAnimation(string floatName, float inputValue, float dampTime)
    {        
        avatarAnim.SetFloat(floatName, inputValue, dampTime, Time.deltaTime);
    }
}

