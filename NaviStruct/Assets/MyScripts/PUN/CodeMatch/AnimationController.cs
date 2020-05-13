using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class AnimationController : MonoBehaviourPunCallbacks
{
    public static AnimationController instance;

    [Header("Display Panels")]
    public GameObject lobbyPanel;
    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject roomPanel;

    [Header("Text Display")]
    public Animator textAnim;
    public TextMeshProUGUI statusText;   
    
    private Animator avatarAnim;
    string currentAnimation = "";    

    [HideInInspector]
    public Animator lobbyAnim;
    [HideInInspector]
    public Animator mainAnim;
    [HideInInspector]
    public Animator joinAnim;
    [HideInInspector]
    public Animator roomAnim;

    private void Awake()
    {        
        if (instance != null)
        {
            GameObject.Destroy(instance);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }        
    }

    private void Start()
    {
        lobbyAnim = lobbyPanel.GetComponent<Animator>();
        mainAnim = mainPanel.GetComponent<Animator>();
        joinAnim = joinPanel.GetComponent<Animator>();
        roomAnim = roomPanel.GetComponent<Animator>();
        avatarAnim = PuppetController.pc.avatarPlayer.GetComponent<Animator>();
    }
    public IEnumerator FadeAnimation(Animator anim, string animBoolString, GameObject activePanel, GameObject inactivePanel)
    {
        anim.SetBool(animBoolString, true);

        yield return new WaitForSeconds(.7f);

        anim.SetBool(animBoolString, false);
        activePanel.SetActive(true);
        inactivePanel.SetActive(false);
    }

    public IEnumerator ScreenTextFade(Animator anim, string displayText, string boolName)
    {      
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"))
        {
            anim.SetBool(boolName, true);
            yield return new WaitForSeconds(1.3f);
        }

        statusText.text = displayText;

        if (boolName == "isFadeStart")
        {
            anim.SetBool("isFadeStart", true);
            yield return new WaitForSeconds(1.3f);
            anim.SetBool("isFadeStart", false);
            anim.SetBool("isFadeMenu", true);
        }
        else
        {
            anim.SetBool(boolName, false);
            yield return new WaitForSeconds(1.3f);
            anim.SetBool(boolName, true);
        }                    
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

