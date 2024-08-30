using UnityEngine;
using UnityEngine.EventSystems;
using Compiler;

public class CardLeadEffect : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.OnCardClicked += YourMethod;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.OnCardClicked -= YourMethod;
    }

    private void YourMethod(GameObject card)
    {
        Evaluator evaluator = new Evaluator();
        evaluator.Evaluate(card.GetComponent<Card>().cardEffect);
    }
}