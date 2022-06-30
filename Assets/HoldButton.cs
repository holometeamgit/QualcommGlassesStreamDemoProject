using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {

    [SerializeField]
    UnityEvent OnHeld;

    public void OnPointerDown(PointerEventData eventData) {
        OnHeld?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData) {
        throw new System.NotImplementedException();
    }
}
