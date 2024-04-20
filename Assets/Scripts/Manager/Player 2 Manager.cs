using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using System.Threading;

public class Player2Manager : MonoBehaviour
{
    #region Variables
    public int powerPlayer2;
    private GameObject lastClickedCard = null;

    public GameObject cardPrefab2;
    public GameObject cardLeadPrefab;
    
    public GameObject deckPlayer2;
    public GameObject graveyardPlayer2;
    public GameObject handPlayer2;

    public GameObject aumentoMZonePlayer2;
    public GameObject aumentoRZonePlayer2; 
    public GameObject aumentoSZonePlayer2;
    public GameObject climaZonePlayer2;
    public GameObject meleeZonePlayer2;
    public GameObject rangeZonePlayer2;
    public GameObject siegeZonePlayer2;

    public GameObject leadSpotPlayer2;

    [Header ("Hand Counter")]
    public int startingHandSize = 10;
    public int maxHandSize = 10;

    [Header ("UI")]
    public TextMeshProUGUI deckTextPlayer2;
    public TextMeshProUGUI discardTextPlayer2;
    #endregion

    public void CleanField()
    {
        GameObject[] cartas = GameObject.FindGameObjectsWithTag("Card Player2");

        foreach (GameObject carta in cartas)
        {
            //Si esta en la mano no la destruyas
            if (carta.transform.IsChildOf(handPlayer2.transform)) continue;
            else if (carta.transform.IsChildOf(leadSpotPlayer2.transform)) continue;

            //Si no esta en la mano destruyela, Aqui podriamos poner despues que se vayan al cementerio
            carta.transform.SetParent(graveyardPlayer2.transform);
            carta.transform.localPosition = new Vector3(0,0,0);
        }       
    }

    //Cambiar Cartas
    public void SwapCards() //Aqui en algun momento pondre la cantidad de cartas a Swapear, no es por gusto el metodo
    {
        if (GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable == 1)
        {
            if (GameManager.player2CanSwapCards == true)
            {
                for (int i = 0; i < 2; i++)
                {
                    StartingTheCardSwap();
                    GameManager.player2CanSwapCards = false;
                }
            }
        }
    }

    //Metodo para iniciar el cambio de cartas en la mano
    public void StartingTheCardSwap()       //Lo acciona un botton, aqui se dan las condiciones previas para el intercambio
    {
        //Añadir un Subscriber a los Eventos
        EventManager.OnCardClicked += SelectCardInHand;
        //Inicio del metodo para cambiar cartas
        StartCoroutine(OrganizedMetods());      
    }
    IEnumerator OrganizedMetods()
    {
        yield return StartCoroutine(SwapCardsInHands());
        yield return StartCoroutine(DrawSingleCard());
    }
    IEnumerator DrawSingleCard()
    {
        yield return new WaitForSeconds(0.2f);
        DrawCard(1);
    }
    //Coroutine para Cambiar las cartas en la mano
    IEnumerator SwapCardsInHands()
    {
            //WaitUntil es usado aqui para pausar el juego hasta que el usuario toque una carta
            yield return new WaitUntil(() => lastClickedCard != null);
            //Una vez tengamos alguna carta en "lastClikedCard" podemos empezar el intercambio
            ReturnCardToDeck(lastClickedCard);
            //Robamos una carta
            DrawCard(1);
            //Eliminamos el Event Subscriber ya que no queremos que fuera de este metodo el click guarde informacion
            EventManager.OnCardClicked -= SelectCardInHand;      
            lastClickedCard = null; 
    }
    //Guardamos la carta clickeada en la variable lastClickedCard
    public void SelectCardInHand(GameObject card)
    {   
        //Si no esta en la mano no guardamos la informacion, solo queremos cartas de la mano
        if (card.transform.IsChildOf(handPlayer2.transform) != handPlayer2)
        {
            Debug.Log("Debe seleccionar una carta de la mano");
        }
        else
        {
            lastClickedCard = card;
        }
    }

    //Retorna una carta seleccionada al deck
    public void ReturnCardToDeck(GameObject card)
    {   
        //Tomamos el Nombre de la carta que vamos a eliminar
        string name = card.GetComponent<Card>().cardName;
        //Eliminamos la carta de la mano
        Destroy(card);
        //Añadimos el scriptable object con el nombre de la carta a la lista de cartas del deck
        deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.Add(Resources.Load<CardData>("Scriptable Objects/Arthas Deck/" + name));
        //Barajeamos el deck
        ShuffleDeck(deckPlayer2.GetComponent<ArthasDeck>().arthasDeck);
    }

    public void SetLead()
    {
        GameObject g = Instantiate(cardLeadPrefab, leadSpotPlayer2.transform);
        g.GetComponent<Card>().cardData = deckPlayer2.GetComponent<ArthasDeck>().arthasLeadCard;
        g.name = g.GetComponent<Card>().cardData.cardName;
    }

    //Robar carta del Deck
    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)     
        {   
            if (handPlayer2.transform.GetComponentsInChildren<Card>(true).Length < 10)
            {       
                //Instanciando la carta con el prefab y en la posicion de la mano
                GameObject g = Instantiate(cardPrefab2, handPlayer2.transform);                         
                //Dandole a cada prefab de carta los datos de los scriptable objects
                g.GetComponent<Card>().cardData = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck[i];
                //Eliminando la carta robada
                deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.RemoveAt(i);                          
                //Dandole un nombre a la carta en el inspector
                g.name = g.GetComponent<Card>().cardData.cardName;  
            }
            else
            {
                //Instanciando la carta con el prefab y en la posicion de la mano
                GameObject g = Instantiate(cardPrefab2, graveyardPlayer2.transform);
                g.transform.localPosition = new Vector3(0,0,0);                         
                //Dandole a cada prefab de carta los datos de los scriptable objects
                g.GetComponent<Card>().cardData = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck[i];
                //Eliminando la carta robada
                deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.RemoveAt(i);                          
                //Dandole un nombre a la carta en el inspector
                g.name = g.GetComponent<Card>().cardData.cardName;
            }                   
        } 
    }

    //Barajear el Deck
    static void ShuffleDeck(List<CardData> deck)
    {
        System.Random rng = new System.Random();  
        int n = deck.Count;  
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardData value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    //Contar la cantidad de ataque que existe en el campo
    public void CountAttackOnField()
    {
        applyAumento();
        foreach (Transform child in meleeZonePlayer2.transform)
        {
            powerPlayer2 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in rangeZonePlayer2.transform)
        {
            powerPlayer2 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in siegeZonePlayer2.transform)
        {
            powerPlayer2 += child.GetComponent<Card>().attackPower;
        }
    }

    public void applyClima()
    {
        int amountOfChange;
        int amounOfClimas = climaZonePlayer2.transform.childCount;
        
        for (int i = 0; i < amounOfClimas; i ++)
        {
            amountOfChange = climaZonePlayer2.transform.GetChild(i).GetComponent<Card>().effectNumber;
            EffectsManager.ClimateEffect(climaZonePlayer2, amountOfChange);
        }
    }

    public void applyAumento()
    {
        int amountOfChangeM;
        if (aumentoMZonePlayer2.transform.childCount != 0) 
        {
            amountOfChangeM = aumentoMZonePlayer2.transform.GetChild(0).GetComponent<Card>().effectNumber;
            EffectsManager.IncreaseEffect(meleeZonePlayer2, amountOfChangeM);
        }
        int amountOfChangeR;
        if (aumentoRZonePlayer2.transform.childCount != 0) 
        {
            amountOfChangeR = aumentoRZonePlayer2.transform.GetChild(0).GetComponent<Card>().effectNumber;
            EffectsManager.IncreaseEffect(rangeZonePlayer2, amountOfChangeR);
        }

        int amountOfChangeS;
        if (aumentoSZonePlayer2.transform.childCount != 0) 
        {
            amountOfChangeS = aumentoSZonePlayer2.transform.GetChild(0).GetComponent<Card>().effectNumber;
            EffectsManager.IncreaseEffect(siegeZonePlayer2, amountOfChangeS);
        }
    }

    public static void ShowCardBack()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card Player2");

        foreach (GameObject card in cards)
        {
            if (card.transform.parent.name != "Hand p2") continue;
            else
            {
                card.GetComponent<Card>().displayCardBack = true;
            }
        }
    }
    public static void HideCardBack()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card Player2");

        foreach (GameObject card in cards)
        {
            if (card.transform.parent.name != "Hand p2") continue;
            else
            {
                card.GetComponent<Card>().displayCardBack = false;
            }
        }
    }
    
     public void Start()
     {
        ShuffleDeck(deckPlayer2.GetComponent<ArthasDeck>().arthasDeck);
        SetLead();
     }

     public void Update()
     {
        deckTextPlayer2.text = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.Count.ToString();
        discardTextPlayer2.text = (graveyardPlayer2.transform.childCount - 1).ToString();
     }
 }