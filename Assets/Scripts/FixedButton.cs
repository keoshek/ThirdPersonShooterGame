using UnityEngine;
using UnityEngine.EventSystems;

public class FixedButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public bool Pressed;
    public bool Change;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        if (!Change)
        {
            Change = true;
        }
        else
        {
            Change = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}