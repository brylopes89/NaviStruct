using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneManagerSingleton : MonoBehaviour
{
    public static SceneManagerSingleton instance { get; private set; }

    public GameObject lobbyPanel = null;
    public GameObject mainPanel = null;
    public GameObject joinPanel = null;
    public GameObject roomPanel = null;
    public GameObject textController = null;
    public GameObject avatar = null;
    public GameObject camController = null;
    public AnimationController animationController = null;   

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
