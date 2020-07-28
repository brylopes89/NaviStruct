using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionSystemProvider : LocomotionProvider
{
    // Start is called before the first frame update
    void Start()
    {
        system = MasterManager.ClassReference.VRPuppetController.gameObject.GetComponent<LocomotionSystem>();
    }
}
