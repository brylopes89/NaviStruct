using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public Camera mainCamera;

    public void SetInteractiveMenuPosition(float distance, float offset)
    {        
        Vector3 forwardCam = mainCamera.transform.forward;
        Vector3 camPos = mainCamera.transform.position;

        Vector3 projected = Vector3.ProjectOnPlane(forwardCam, Vector3.up).normalized * distance;
        Vector3 menuPos = projected + camPos + new Vector3(0, -0.5f, 0);

        float menuBottom = menuPos.y - this.transform.localScale.y / 2;

        if (menuBottom < 0)
        {
            menuPos.y = this.transform.localScale.y / 2;
        }

        this.transform.position = menuPos;

        Vector3 rotationOffset = Quaternion.LookRotation(this.transform.position - camPos).eulerAngles;
        rotationOffset.x = offset;
        this.transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
