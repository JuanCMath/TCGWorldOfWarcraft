using UnityEngine;
using UnityEngine.EventSystems; // Necesario para las interfaces de eventos

public class DisplayCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject displayedCard;
    // Este método se llama cuando el puntero entra en el objeto
    public void OnPointerEnter(PointerEventData eventData)
    {   
        foreach (Transform child in GameObject.Find("Game Manager").GetComponent<GameManager>().panelCardDsiplay.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject enteredObject = eventData.pointerEnter;
        GameObject cardToDisplay = enteredObject.GetComponentInParent<Card>().gameObject;   

        displayedCard = Instantiate(cardToDisplay, GameObject.Find("Game Manager").GetComponent<GameManager>().panelCardDsiplay.transform);

        displayedCard.transform.localPosition = new Vector3(0,0,0);
        displayedCard.transform.localScale *= 5;
    }

    // Este método se llama cuando el puntero sale del objeto
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(displayedCard);
    }
}