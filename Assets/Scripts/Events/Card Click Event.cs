using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickEvent : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Invocar el evento cuando se haga clic en la carta
        EventManager.CardClicked(gameObject);
    }
}