using UnityEngine;
using VRKeyboard.Utils;

[CreateAssetMenu(menuName = "Manager/ClassReferenceManager")]
public class ClassReferenceManager : ScriptableObject
{
    #region Class References
    [Header("Class References")]
    [SerializeField]
    private AnimationController _animController;
    public AnimationController AnimController { get { return _animController; } set { _animController = value; } }

    [SerializeField]
    private PuppetController _puppetController;
    public PuppetController PuppetController { get { return _puppetController; } set { _puppetController = value; } }

    [SerializeField]
    private CodeMatchLobbyController _lobbyController;
    public  CodeMatchLobbyController LobbyController { get { return _lobbyController; } set { _lobbyController = value; } }

    [SerializeField]
    private VRSupportController _vrInputController;
    public VRSupportController VrInputController { get { return _vrInputController; } set { _vrInputController = value; } }

    [SerializeField]
    private KeyboardManager _keyboard;
    public KeyboardManager Keyboard { get { return _keyboard; } set { _keyboard = value; } }
    #endregion

    #region Scene Objects
    [Header("Scene Object References")]
    
    [SerializeField]
    private GameObject _textController;
    public GameObject TextController { get { return _textController; } set { _textController = value; } }

    [SerializeField]
    private GameObject _avatar;
    public GameObject Avatar { get { return _avatar; } set { _avatar = value; } }

    [SerializeField]
    private GameObject _camController;
    public GameObject CamController { get { return _camController; } set { _camController = value; } }
   
    #endregion
}
