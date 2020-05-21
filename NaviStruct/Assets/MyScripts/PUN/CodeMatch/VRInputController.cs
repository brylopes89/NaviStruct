using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRInputController : MonoBehaviour
{      
    [SerializeField]
    private Canvas startCanvas;
    [SerializeField]
    private Canvas keyboardCanvas;
    [SerializeField]
    private GameObject cameraRig;    

    public AnimationController animController;
    private float cameraDistance = 10f;
    private float keyboardDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {        
        keyboardCanvas.gameObject.SetActive(false);
    }
    
    public void VRToggleOnClick(bool isToggle)
    {
        StartCoroutine(EnableVRSupport(isToggle));
    }

    public void OnSelectKeyboard()
    {       
        StartCoroutine(animController.FadeKeyboard(animController.keyboardAnim, "isFade", true));
        SetKeyboardPosition();
    }

    public void OnDeselectKeyboard()
    {        
        StartCoroutine(animController.FadeKeyboard(animController.keyboardAnim, "isFade", false));
    }

    private void SetKeyboardPosition()
    {
        keyboardCanvas.worldCamera = Camera.main;
        keyboardCanvas.transform.localScale = new Vector3(0.010f, 0.010f, 0.010f);
        keyboardCanvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * keyboardDistance;
        keyboardCanvas.transform.rotation = Quaternion.LookRotation(keyboardCanvas.transform.position - cameraRig.transform.position);
    }

    private IEnumerator EnableVRSupport(bool activateDevice)
    {
        yield return new WaitForEndOfFrame();

        if (activateDevice)
        {
            XRSettings.LoadDeviceByName("OpenVR");
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = true;            

            yield return new WaitForEndOfFrame();

            startCanvas.renderMode = RenderMode.WorldSpace;
            startCanvas.worldCamera = Camera.main;           

            startCanvas.transform.localScale = new Vector3(0.010f, 0.010f, 0.010f);
            startCanvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * cameraDistance;
            startCanvas.transform.rotation = Quaternion.LookRotation(startCanvas.transform.position - cameraRig.transform.position);           
        }
        else
        {
            XRSettings.LoadDeviceByName("None");
            startCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;            
        }

        yield return null;
    }
}
