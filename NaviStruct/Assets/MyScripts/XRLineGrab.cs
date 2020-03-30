using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRLineGrab : MonoBehaviour
{
    InputHelpers.Button trigger = InputHelpers.Button.Trigger;

    public InputDevice m_RightController;
    public InputDevice m_LeftController;    
    public XRController inputdevice;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        bool activated = false;

        m_LeftController.IsPressed(trigger, out bool value);
        activated |= value;

        if (activated)
            print("Trigger presed");
    }
}
