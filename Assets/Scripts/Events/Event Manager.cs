using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour
{
    public static event Action<GameObject> OnCardClicked;

    public static void CardClicked(GameObject card)
    {
        OnCardClicked?.Invoke(card);
    }
}