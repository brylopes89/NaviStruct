using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum ActiveVRFamily { Oculus, Vive, Windows };
public class HMDHandler : MonoBehaviour
{
    public string detectedHMD = "";
    public ActiveVRFamily activeVRHMD;
    public Transform leftController;   
    public Transform rightController;
    public Transform rightAimingNub;
    public Transform leftAimingNub;

    public Vector3 ViveOffset;

    // Start is called before the first frame update
    private void Awake()
    {
        detectedHMD = XRSettings.loadedDeviceName;
        if (detectedHMD.ToLower().Contains("vive"))
        {
            // Must be a Vive headset.
            activeVRHMD = ActiveVRFamily.Vive;
            leftAimingNub.localEulerAngles = ViveOffset;
            rightAimingNub.localEulerAngles = ViveOffset;
            
        }

       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
