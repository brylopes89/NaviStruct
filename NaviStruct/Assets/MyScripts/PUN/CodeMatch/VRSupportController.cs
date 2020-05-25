using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRSupportController : MonoBehaviour
{
    [SerializeField]
    private Canvas startCanvas;
    [SerializeField]
    private Canvas keyboardCanvas;
    [SerializeField]
    private GameObject cameraRig;
    [SerializeField]
    private AnimationController animController;

    public float cameraDistance = 7f;
    public float keyboardDistance = 3f;
    public float speed = 1.5f;
    public float duration = 5f;

    private void Start()
    {        
        animController = MasterManager.ClassReference.AnimController;        
    }

    public void VRToggleOnClick(bool isToggle)
    {
        StartCoroutine(EnableVRSupport(isToggle));
    }

    public void OnSelectKeyboard()
    {
        if (XRSettings.enabled)
        {
            StartCoroutine(animController.FadeKeyboard(animController.keyboardAnim, "isFade", true));
            SetKeyboardPosition();
        }            
    }

    public void OnDeselectKeyboard()
    {        
        if(XRSettings.enabled)
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

            Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * cameraDistance;
            float i = 0.0f;
            float rate = (1.0f / duration) * speed;

            startCanvas.renderMode = RenderMode.WorldSpace;
            startCanvas.worldCamera = Camera.main;
            startCanvas.transform.localScale = new Vector3(0.010f, 0.010f, 0.010f);

            while (i < 1)
            {
                i += Time.deltaTime * rate;
                startCanvas.transform.position = Vector3.Lerp(startCanvas.transform.position, targetPos, i);
                startCanvas.transform.rotation = Quaternion.LookRotation(startCanvas.transform.position - cameraRig.transform.position);
                yield return null;
            }                      
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
