using UnityEngine;
using TMPro;
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
    private GameSetupController _gameSetupController;
    public GameSetupController GameSetupController { get { return _gameSetupController; } set { _gameSetupController = value; } }

    [SerializeField]
    private PuppetController _puppetController;
    public PuppetController PuppetController { get { return _puppetController; } set { _puppetController = value; } }

    [SerializeField]
    private CodeMatchLobbyController _lobbyController;
    public  CodeMatchLobbyController LobbyController { get { return _lobbyController; } set { _lobbyController = value; } }

    [SerializeField]
    private XRMenuManager _xrSupportManager;
    public XRMenuManager XRSupportManager { get { return _xrSupportManager; } set { _xrSupportManager = value; } }

    [SerializeField]
    private KeyboardManager _keyboardManager;
    public KeyboardManager KeyboardManager { get { return _keyboardManager; } set { _keyboardManager = value; } }

    private bool _isVRSupport = false;
    public bool IsVRSupport { get { return _isVRSupport; } set { _isVRSupport = value; } }

    #endregion

    #region Scene Objects
    [Header("Scene Object References")]
    [SerializeField]
    private GameObject _avatar;
    public GameObject Avatar { get { return _avatar; } set { _avatar = value; } }

    [SerializeField]
    private GameObject _camController;
    public GameObject CamController { get { return _camController; } set { _camController = value; } }

    #endregion
}
