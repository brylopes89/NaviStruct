using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvents : MonoBehaviour
{
    public ButtonWatcher watcher;
    public RadialMenu radialMenu = null;

    public bool IsPressed = false; // public to show button state in the Unity Inspector window
    public Vector3 rotationAngle = new Vector3(45, 45, 45);
    public float rotationDuration = 0.25f; // seconds

    private Quaternion offRotation;
    private Quaternion onRotation;
    private Coroutine rotator;

    // Start is called before the first frame update
    void Awake()
    {
        watcher.primaryButtonPress.AddListener(onPrimaryButtonEvent);        
        //watcher.axisTouch.AddListener(onAxisTouchEvent);
        watcher.axisTouch += onAxisTouchEvent;

        offRotation = this.transform.rotation;
        onRotation = Quaternion.Euler(rotationAngle) * offRotation;
    }

    private void OnDestroy()
    {
        watcher.axisTouch -= onAxisTouchEvent;
    }

    public void onPrimaryButtonEvent(bool pressed)
    {    
        if (pressed)
        {
            IsPressed = !IsPressed;
            radialMenu.Show(IsPressed);
        }     
    
        /*if (rotator != null)
            StopCoroutine(rotator);

        if (pressed)
            rotator = StartCoroutine(AnimateRotation(this.transform.rotation, onRotation));
        else
            rotator = StartCoroutine(AnimateRotation(this.transform.rotation, offRotation));*/
    }

    public void onAxisTouchEvent(bool touched, Vector2 axis)
    {               
        radialMenu.SetTouchPosition(axis);               
    }

    private IEnumerator AnimateRotation(Quaternion fromRotation, Quaternion toRotation)
    {
        float t = 0;
        while (t < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t / rotationDuration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
