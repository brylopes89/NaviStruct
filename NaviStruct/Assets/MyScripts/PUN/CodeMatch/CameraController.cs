using UnityEngine;
using System.Collections;

[System.Serializable]
public class CameraController : MonoBehaviour
{
    public Transform target;

    public float follow_Distance = 3.0f; //How far behind the camera will follow the targeter.
    public float camera_Elevation = 3.0f; //How high the camera will rise above the targeter's Z axis.
    public float follow_Tightness = 5.0f; //How closely the camera will follow the target. Higher values are snappier, lower results in a more lazy follow.
    public float rotation_Tightness = 10.0f; //How closely the camera will react to rotations, similar to above.    
    public float camera_Yaw = 3.0f;
    public float yaw_Multiplier = 0.005f;//Curbs the extremes of input. This should be a really small number. Might need to be tweaked, but do it as a last resort.

    public VRPlayerLocomotion pl;

    private void OnEnable()
    {
        if (SceneManagerSingleton.instance.camController == null)
            SceneManagerSingleton.instance.camController = this.gameObject;
    }
    private void Start()
    {
        target = SceneManagerSingleton.instance.avatar.transform;
    }
    // Update is called once per frame
    void Update()
    {
        SetCameraPosition();
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
