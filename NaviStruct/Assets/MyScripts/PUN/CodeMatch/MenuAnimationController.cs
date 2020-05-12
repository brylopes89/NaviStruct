﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class MenuAnimationController : MonoBehaviourPunCallbacks
{
    public static MenuAnimationController animController;

    [Header("Display Panels")]
    public GameObject lobbyPanel;
    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject roomPanel;

    [Header("Text Display")]
    public Animator textAnim;
    public TextMeshProUGUI statusText;   

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
            animController = this;
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

    public IEnumerator ScreenTextFade(Animator anim, string displayText, string boolName)
    {      
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"))
        {
            anim.SetBool(boolName, true);
            yield return new WaitForSeconds(1.2f);
        }
        
        statusText.text = displayText;
        anim.SetBool(boolName, false);       

        yield return new WaitForSeconds(1.3f);

        anim.SetBool(boolName, true);                     
    }
}
