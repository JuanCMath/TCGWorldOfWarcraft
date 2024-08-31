using UnityEngine;
using UnityEngine.EventSystems;
using Compiler;

public class CardLeadEffect : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.OnCardClicked += ActivateEffect;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.OnCardClicked -= ActivateEffect;
    }

    private void ActivateEffect(GameObject card)
    {
        Evaluator evaluator = new Evaluator();
        evaluator.Evaluate(card.GetComponent<Card>().cardEffect);

        GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable --;
    }
}