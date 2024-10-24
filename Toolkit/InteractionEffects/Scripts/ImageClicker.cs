using UnityEngine;
using UnityEngine.EventSystems;

public class ImageClicker : MonoBehaviour, IPointerDownHandler
{
    public TextManager textManager;


    public void OnPointerDown(PointerEventData eventData)
    {
        textManager.OnImageClick();
    }
}

