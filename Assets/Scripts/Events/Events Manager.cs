using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<GameObject> OnCardClicked;

    public static event Action<Transform> OnCardDrop;

    public static void CardClicked(GameObject card)
    {
        OnCardClicked?.Invoke(card);
        //Esto es lo mismo que:
        //if (OnCardClicked =! null)
        //    OnCardClicked();
    }

    public static void CardDroped(Transform panel)
    {
        OnCardDrop?.Invoke(panel);
    }
}