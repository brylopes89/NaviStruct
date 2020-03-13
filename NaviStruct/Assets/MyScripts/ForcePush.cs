using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ForcePush : MonoBehaviour
{
    public float chargeRate;
    public float pushAmount;
    private float amountStart;
    public float pushRadius;
    private float radiusStart;

    public bool showGizmos;
    
    public SteamVR_Action_Boolean gripAction;
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Input_Sources targetSource;

    // Start is called before the first frame update
    void Start()
    {        
        amountStart = pushAmount;
        radiusStart = pushRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (gripAction.GetState(targetSource))
        {
            pushAmount += chargeRate * Time.deltaTime;
            pushRadius += chargeRate * Time.deltaTime;
            Pulse(pushAmount, 150, 10, SteamVR_Input_Sources.LeftHand);
        }
        if (gripAction.GetStateUp(targetSource))
        {
            Push();
        }
    }

    private void Push()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pushRadius);

        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Interact"))
            {
                Rigidbody pushedBody = pushedObject.GetComponent<Rigidbody>();
                pushedBody.AddExplosionForce(pushAmount, Vector3.up, pushRadius);
            }
            
        }

        pushAmount = amountStart;
        pushRadius = radiusStart;
    }

    private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, pushRadius);
        }
    }
}
