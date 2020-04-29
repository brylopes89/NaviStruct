using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class AnimationController : MonoBehaviourPunCallbacks
{
    public static AnimationController animController;

    [Header("Display Panels")]
    public GameObject lobbyPanel;
    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject roomPanel;

    [Header("Text Display")]
    public Animator textAnim;
    public TextMeshProUGUI updateText;

    [Header("Avatar Animator")]
    [SerializeField]
    private Animator avatarAnim;
    private string currentAnimation = "";

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
        if(animController == null)
        {
            DontDestroyOnLoad(gameObject);
            animController = this;
        }
        else if(animController != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        lobbyAnim = lobbyPanel.GetComponent<Animator>();
        mainAnim = mainPanel.GetComponent<Animator>();
        joinAnim = joinPanel.GetComponent<Animator>();
        roomAnim = roomPanel.GetComponent<Animator>();
    }
    public IEnumerator FadeAnimation(Animator anim, string animBoolString, GameObject activePanel, GameObject inactivePanel)
    {
        anim.SetBool(animBoolString, true);

        yield return new WaitForSeconds(.7f);

        anim.SetBool(animBoolString, false);
        activePanel.SetActive(true);
        inactivePanel.SetActive(false);
    }

    public IEnumerator ScreenTextFade(Animator anim, TextMeshProUGUI screenText, bool isFade, string text)
    {
        anim.SetBool("isFadeMenu", true);

        yield return new WaitForSeconds(1.4f);

        anim.SetBool("isFadeMenu", false);
        screenText.text = text;

        yield return new WaitForSeconds(2f);
        anim.SetBool("isFadeMenu", true);
    }

    public void SetAnimation(string animationName)
    {
        if (currentAnimation != "")
        {
            avatarAnim.SetBool(currentAnimation, false);
        }
        avatarAnim.SetBool(animationName, true);
        currentAnimation = animationName;
    }

    public void SetAnimationIdle()
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

