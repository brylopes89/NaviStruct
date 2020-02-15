using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDrag : MonoBehaviour
{
    private void OnMouseDown()
    {
        print("On Mouse Down");
    }

    private void OnMouseDrag()
    {
        print("On mouse drag");
    }

}
