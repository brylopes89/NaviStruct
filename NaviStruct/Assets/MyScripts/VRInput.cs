using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInput : BaseInput
{
    public Camera eventCamera = null;
    public SteamVR_Input_Sources targetSource;
    public SteamVR_Action_Boolean clickAction;

    protected override void Awake()
    {
        GetComponent<BaseInputModule>().inputOverride = this;
    }

    public override bool GetMouseButton(int button)
    {
        return clickAction.GetState(targetSource);        
    }

    public override bool GetMouseButtonDown(int button)
    {
        return clickAction.GetStateDown(targetSource);
    }

    public override bool GetMouseButtonUp(int button)
    {
        return clickAction.GetStateUp(targetSource);
    }

    public override Vector2 mousePosition
    {
        get
        {
            return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight /2);
        }
    }
}
