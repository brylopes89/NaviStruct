using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public Camera mainCamera;
    public float speed = 1f;
    public float duration = 2f;

    public IEnumerator SetCanvasPosition(float distance, float offset)
    {
        Vector3 forwardCam = mainCamera.transform.forward;
        Vector3 camPos = mainCamera.transform.position;

        Vector3 projected = Vector3.ProjectOnPlane(forwardCam, Vector3.up).normalized * distance;
        Vector3 targetPos = projected + camPos + new Vector3(0, -0.3f, 0);

        float menuBottom = targetPos.y - this.transform.localScale.y / 2;     
        float i = 0.0f;
        float rate = (1.0f / duration) * speed;     

        while (i < 1)
        {
            Vector3 rotationOffset = Quaternion.LookRotation(this.transform.position - camPos).eulerAngles;
            rotationOffset.x = offset;

            i += Time.deltaTime * rate;

            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, i);
            this.transform.rotation = Quaternion.Euler(rotationOffset);

            if (menuBottom < 0)            
                targetPos.y = Mathf.Lerp(targetPos.y, this.transform.localScale.y / 2, i);               

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
