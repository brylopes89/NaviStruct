using UnityEngine;
using System.Collections;

[System.Serializable] [AddComponentMenu("Camera-Control/Mouse Look")]
public class StandaloneCameraController : MonoBehaviour
{
    #region Mouse Look Values
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

    [Header("Mouse Input Values")]
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 3f;
    public float sensitivityY = 3f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -60f;
    public float maximumY = 60f;

    private float rotationY = 0f;
    #endregion

    #region Camera Follow Values
    public Transform target;

    [Header("Camera Follow Values")]
    public float smooth_Speed = 0.125f;
    public float follow_Distance = 3.0f; //How far behind the camera will follow the targeter.
    public float camera_Elevation = 3.0f; //How high the camera will rise above the targeter's Z axis.
    public float follow_Tightness = 5.0f; //How closely the camera will follow the target. Higher values are snappier, lower results in a more lazy follow.
    public float rotation_Tightness = 10.0f; //How closely the camera will react to rotations, similar to above.    
    public float camera_Yaw = 3.0f;
    public float yaw_Multiplier = 0.005f;//Curbs the extremes of input. This should be a really small number. Might need to be tweaked, but do it as a last resort.
    #endregion

    private void Start()
    {
        target = MasterManager.ClassReference.Avatar.transform;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))        
            SetMouseInputValues();
        else
            SetCameraPosition();
    }

    void SetMouseInputValues()
    {        
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }               
    }

    void SetCameraPosition()
    {
        //Calculate where we want the camera to be.
        if(target != null)
        {
            Vector3 thirdPos = target.TransformPoint(camera_Yaw * yaw_Multiplier, camera_Elevation, -follow_Distance);
            //Get the difference between the current location and the target's current location.
            Vector3 thirdDiff = target.position - transform.position;

            //Move the camera towards the new position.
            transform.position = Vector3.Lerp(transform.position, thirdPos, Time.deltaTime * follow_Tightness);

            Quaternion newRotation = Quaternion.LookRotation(thirdDiff, target.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotation_Tightness);
        }       
    }
}
