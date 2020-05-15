using Photon.Pun;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviourPunCallbacks
{   
    [Header("Menu Display")]
    public GameObject lobbyPanel;
    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject roomPanel;
    public GameObject textController;
    
    [Header("Animators")]
    
    public Animator textAnim;    
    public Animator avatarAnim;    
    public Animator lobbyAnim;    
    public Animator mainAnim;    
    public Animator joinAnim;    
    public Animator roomAnim;

    private TextMeshProUGUI statusText;
    private Scene scene;    
    private string currentAnimation = "";

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        if(SceneManagerSingleton.instance.animationController == null)
            SceneManagerSingleton.instance.animationController = this;

        if (scene.buildIndex == 0)
        {            
            SceneManagerSingleton.instance.lobbyPanel = lobbyPanel;
            SceneManagerSingleton.instance.mainPanel = mainPanel;
            SceneManagerSingleton.instance.joinPanel = joinPanel;
            SceneManagerSingleton.instance.roomPanel = roomPanel;
            SceneManagerSingleton.instance.textController = textController;
            
            lobbyAnim = lobbyPanel.GetComponent<Animator>();
            mainAnim = mainPanel.GetComponent<Animator>();
            joinAnim = joinPanel.GetComponent<Animator>();
            roomAnim = roomPanel.GetComponent<Animator>();
            textAnim = textController.GetComponent<Animator>();
            statusText = textController.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            //lobbyPanel = null;
            //mainPanel = null;
            //joinPanel = null;
            //roomPanel = null;
            //textController = null;
            avatarAnim = PuppetController.pc.avatarPlayer.GetComponent<Animator>();
        }
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

