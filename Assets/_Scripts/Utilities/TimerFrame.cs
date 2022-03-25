using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
public class TimerFrame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color enterColor = Color.black;
    [SerializeField] private Color pressedColor = Color.white;

    public UnityEvent OnPressEvent = null;

    private Image image = null;
    private bool enter = false;

    private void Awake()
    {
        image = GetComponent<Image>();

        if (OnPressEvent == null)
            OnPressEvent = new UnityEvent();
    }

    private void Start()
    {
        image.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = pressedColor;
        OnPressEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enter)
            image.color = normalColor;
        else
            image.color = enterColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = enterColor;
        enter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enter = false;
        image.color = normalColor;
    }
}
