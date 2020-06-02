using UnityEngine;

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
    public GameObject keyboard = null;

    public AnimationController animationController = null;
    public PuppetController puppetController = null;
    public CodeMatchLobbyController lobbyController = null;
    public XRSupportManager vrInputController = null;

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
