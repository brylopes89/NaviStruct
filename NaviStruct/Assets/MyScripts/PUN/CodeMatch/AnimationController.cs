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
    [Header("Menu Display Panels")]
    public GameObject lobbyPanel;
    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject roomPanel;
    public GameObject keyboard;

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

    public float speedThreshold;
    [Range(0,1)]
    public float smoothing;

    private TextMeshProUGUI statusText;
    private TextMeshProUGUI tipText;
    private VRRig vrRig;
    private Vector3 previousPos;

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
            vrRig = avatarAnim.GetComponent<VRRig>();
            previousPos = vrRig.head.vrTarget.position;
        }
    }

    private void Update()
    {
        //Compute the speed of headset
        Vector3 headsetSpeed = vrRig.head.vrTarget.position - previousPos / Time.deltaTime;
        headsetSpeed.y = 0;
        //Local speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = vrRig.head.vrTarget.position;

        //Set Animator Values
        float previousDirectionX = avatarAnim.GetFloat("DirectionX");
        float previousDirectionY = avatarAnim.GetFloat("DirectionY");

        avatarAnim.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        avatarAnim.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        avatarAnim.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));
    }

    public IEnumerator FadeMenuPanel(Animator anim, string animBoolString, GameObject activePanel, GameObject inactivePanel)
    {
        anim.SetBool(animBoolString, true);

        yield return new WaitForSeconds(.7f);

        anim.SetBool(animBoolString, false);
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

    public IEnumerator FadeKeyboard(Animator anim, string boolName, bool isEnable)
    {
        if (isEnable)
        {
            keyboard.SetActive(true);
            yield return new WaitForEndOfFrame();
            anim.SetBool(boolName, false);            
        }
        else
        {
            anim.SetBool(boolName, true);
            yield return new WaitForSeconds(.5f);
            keyboard.SetActive(false);
            keyboard.GetComponent<KeyboardManager>().Clear();
        }
        yield return null;
    }

    public void SetAvatarAnimation(string animationName)
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

    public void SetAvatarAnimationIdle()
    {
        if (currentAnimation != "")
        {
            avatarAnim.SetBool(currentAnimation, false);
        }
    }

    public void SetDeathAnimation(int numOfClips)
    {
        int clipIndex = Random.Range(0, numOfClips);
        string animationName = "Death";
        Debug.Log(clipIndex);

        avatarAnim.SetInteger(animationName, clipIndex);
    }
}

