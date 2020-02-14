using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Color32 normalColor = Color.white;
    [SerializeField] private Color32 enterColor = Color.grey;
    [SerializeField] private Color32 downColor = Color.white;
    [SerializeField] private UnityEvent OnClick = new UnityEvent();

    private Image image = null;
    private MeshRenderer meshRenderer = null;
    private bool isMeshRenderer = false;
    private bool isImage = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (GetComponent<Image>() != null)
        {
            isImage = true;
            image = GetComponent<Image>();
        }
        else
        {
            isImage = false;
            return;
        }            

        if (GetComponent<MeshRenderer>() != null)
        {
            isMeshRenderer = true;
            meshRenderer = GetComponent<MeshRenderer>();
        }

        else
        {
            isMeshRenderer = false;
            return;
        }         
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        print("enter");
        if(isImage)
            image.color = enterColor;
        if(isMeshRenderer)
            meshRenderer.material.color = enterColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        print("exit");
        if(isImage)
            image.color = normalColor;
        if(isMeshRenderer)
            meshRenderer.material.color = normalColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        print("down");
        if(isImage)
            image.color = downColor;
        if(isMeshRenderer)
            meshRenderer.material.color = downColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        print("up");
        if(isMeshRenderer)
            meshRenderer.material.color = enterColor;
        if (isImage)
            image.color = enterColor;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        print("click");
        //image.color = enterColor;
        OnClick.Invoke();
    }
}
