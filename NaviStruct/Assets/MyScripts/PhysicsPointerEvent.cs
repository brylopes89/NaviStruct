using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PhysicsPointerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Color32 normalColor = Color.white;
    [SerializeField] private Color32 enterColor = Color.grey;
    [SerializeField] private Color32 downColor = Color.white;
    [SerializeField] private UnityEvent OnClick = new UnityEvent();
   
    private MeshRenderer meshRenderer = null;

    // Start is called before the first frame update
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    public void OnPointerEnter(PointerEventData eventData)
    {
        print("enter");                
        meshRenderer.material.color = enterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("exit");        
        meshRenderer.material.color = normalColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        print("down");
        meshRenderer.material.color = downColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        print("up");
        meshRenderer.material.color = enterColor;
       
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        print("click");        
        OnClick.Invoke();
    }
}
