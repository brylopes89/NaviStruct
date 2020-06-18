using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using VRKeyboard.Utils;

public class XRMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField]
    private Canvas startMenu;
    [SerializeField]
    private Canvas keyboardCanvas;
    [SerializeField]
    private Canvas interactiveMenu;
    [SerializeField]
    private Canvas standaloneMenu;
    [SerializeField]
    private GameObject vrToggle;

    [Header("Camera Rig")]
    [SerializeField]
    private GameObject cameraRig;   
    [SerializeField]
    private XRController controller;
    
    [Header("Canvas Position Values")]
    public float menuDistance = 6f;
    public float keyboardDistance = 3f;
    public float speed = 1.5f;
    public float duration = 5f;  
    
    public bool isVRSupport;
    private bool isKeyboardActive;

    private AnimationController animController;
    private Scene scene;

    private void OnEnable()
    {
        if (MasterManager.ClassReference.XRSupportManager == null)
            MasterManager.ClassReference.XRSupportManager = this;

        scene = SceneManager.GetActiveScene();
        bool temp = false;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (scene.buildIndex == 0)
        {
            vrToggle.SetActive(true);
            if (XRDevice.isPresent)
            {
                temp = true;
                vrToggle.GetComponent<Toggle>().isOn = true;                
            }
            else
            {
                temp = false;
                vrToggle.GetComponent<Toggle>().isOn = false;
            }

            MasterManager.ClassReference.IsVRSupport = temp;
        }

        isVRSupport = MasterManager.ClassReference.IsVRSupport;

        if (scene.buildIndex == 1)
        {            
            if (isVRSupport)                            
                standaloneMenu.gameObject.SetActive(false);
            else
                standaloneMenu.gameObject.SetActive(true);
        }

#elif UNITY_ANDROID || UNITY_IOS
        if (scene.buildIndex == 0)
        {
            temp = false;
            vrToggle.SetActive(false);
            XRSettings.enabled = true;
        }
#endif        
    }

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;            
    }

    private void Update()
    {        
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool value) && isVRSupport)
        {
            if (value)
            {
                if (scene.buildIndex == 0)
                {
                    if (isKeyboardActive)
                        StartCoroutine(SetCanvasPosition(keyboardCanvas, keyboardDistance));
                    else
                        StartCoroutine(SetCanvasPosition(startMenu, menuDistance));
                }
                else
                {
                    ToggleInteractiveMenu(value);                   
                    //TODO: Disable menu
                    //TODO: Drop down movement list

                }
            }                
        }        
    }

    public void VRToggleOnClick(bool isToggle)
    {        
        StartCoroutine(EnableVRSupport(isToggle));
    }

    public void OnSelectKeyboard()
    {
        if (isVRSupport)
        {
            StartCoroutine(animController.FadeCanvas(animController.keyboard, animController.keyboardAnim, "isFade", true));
            isKeyboardActive = true;
            StartCoroutine(SetCanvasPosition(keyboardCanvas, keyboardDistance));
        }            
    }

    public void OnDeselectKeyboard()
    {
        if (isVRSupport)
        {                                 
            StartCoroutine(animController.FadeCanvas(animController.keyboard, animController.keyboardAnim, "isFade", false));
            keyboardCanvas.gameObject.GetComponent<KeyboardManager>().Clear();
            isKeyboardActive = false;
        }            
    } 

    private void ToggleInteractiveMenu(bool isPressed)
    {
        StartCoroutine(animController.FadeCanvas(animController.interactiveMenu, animController.interactiveMenuAnim, "IsFadeOut", true));
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool value))
        {
            if(value)
                StartCoroutine(SetCanvasPosition(interactiveMenu, menuDistance));
        }
            
    }

    private IEnumerator EnableVRSupport(bool activateDevice)
    {
        yield return new WaitForEndOfFrame();                        

        if (activateDevice)
        {
            if(XRSettings.enabled == false && XRSettings.loadedDeviceName != "OpenVR")
            {                
                XRSettings.LoadDeviceByName("OpenVR");
                yield return new WaitForEndOfFrame();
            }
            
            XRSettings.enabled = true;
            isVRSupport = true;

            yield return new WaitForEndOfFrame();           

            StartCoroutine(SetCanvasPosition(startMenu, menuDistance));            
        }
        else
        {
            XRSettings.LoadDeviceByName("None");
            startMenu.renderMode = RenderMode.ScreenSpaceOverlay;
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;
            isVRSupport = false;
        }

        yield return null;
    }

    private IEnumerator SetCanvasPosition(Canvas canvas, float distance)
    {
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * distance;               

        float i = 0.0f;
        float rate = (1.0f / duration) * speed;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.transform.localScale = new Vector3(0.010f, 0.010f, 0.010f);

        while (i < 1)
        {
            Vector3 rotationOffset = Quaternion.LookRotation(canvas.transform.position - cameraRig.transform.position).eulerAngles;
            rotationOffset.x = 35;

            i += Time.deltaTime * rate;
            canvas.transform.position = Vector3.Lerp(canvas.transform.position, targetPos, i);

            if (isKeyboardActive)
                keyboardCanvas.transform.rotation = Quaternion.Euler(rotationOffset);
            else
                canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - cameraRig.transform.position);               
                
            yield return new WaitForEndOfFrame();
        }        
    }
}
