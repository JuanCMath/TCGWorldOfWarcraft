using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    public int power;
    public player owner;
    private GameObject lastClickedCard = null;

    [Header("Prefabs")]
    public GameObject cardPrefab;
    public GameObject cardLeadPrefab;
    
    [Header("Setup Panels")]
    public GameObject deck;
    public GameObject graveyard;
    public GameObject hand; 
    public GameObject lead;
    

    [Header("Game Panels")]
    public GameObject field;
    public GameObject aumentoMZone;
    public GameObject aumentoRZone; 
    public GameObject aumentoSZone;
    public GameObject climaZone;
    public GameObject meleeZone;
    public GameObject rangeZone;
    public GameObject siegeZone;

    [Header ("Hand Counter")]
    public int startingHandSize = 10;
    public int maxHandSize = 10;

    [Header ("UI")]
    public TextMeshProUGUI deckText;
    public TextMeshProUGUI discardText;
    #endregion

    //Eliminar todas las cartas del campo
    public void CleanField()
    {
        GameObject[] cards = owner == player.Player1 ? GameObject.FindGameObjectsWithTag("Card Player1") : GameObject.FindGameObjectsWithTag("Card Player2");

        foreach (GameObject carta in cards)
        {
            //Si esta en la mano no la destruyas
            if (carta.transform.IsChildOf(hand.transform)) continue;
            else if (carta.transform.IsChildOf(lead.transform)) continue;

            //Si no esta en la mano destruyela, Aqui podriamos poner despues que se vayan al cementerio
            carta.transform.SetParent(graveyard.transform);
            carta.transform.localPosition = new Vector3(0,0,0);
        }
    }

    //Cambiar Cartas
    public void SwapCards() //Aqui en algun momento pondre la cantidad de cartas a Swapear si es necesario
    {
        if (owner == player.Player1)
        {
            if (GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable == 1)
            {
                if (GameManager.player1CanSwapCards == true)
                {
                    GameManager.player1CanSwapCards = false;
                    for (int i = 0; i < 2; i++)
                    {
                        StartingTheCardSwap(); //Iniciando el cambio de cartas
                        GameManager.player1CanSwapCards = false;
                    }               
                }
            }
        }
        else if (owner == player.Player2)
        {
            if (GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable == 1)
            {
                if (GameManager.player2CanSwapCards == true)
                {
                    GameManager.player2CanSwapCards = false;
                    for (int i = 0; i < 2; i++)
                    {
                        StartingTheCardSwap(); //Iniciando el cambio de cartas
                        GameManager.player2CanSwapCards = false;
                    }               
                }
            }
        }
    }
    
    //Metodo para iniciar el cambio de cartas en la mano
    public void StartingTheCardSwap()       //Lo acciona un botton, aqui se dan las condiciones previas para el intercambio
    {
        //Añadir un SelectCardInHand a los Eventos
        EventManager.OnCardClicked += SelectCardInHand;
        //Inicio de la Coroutine para cambiar cartas
        StartCoroutine(OrganizedMetods()); 
    }

    //Coroutine necesaria para oganizar el timepo de ejecucion
    IEnumerator OrganizedMetods()
    {
        yield return StartCoroutine(SwapCardsInHands()); //Este se ejecuta primero
        yield return StartCoroutine(DrawSingleCard()); //Este se ejecuta despues que se ejecute el primero
    }
    
    //Robar una carta
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
            //Eliminamos el Event Subscriber ya que no queremos que fuera de este metodo el click guarde informacion
            EventManager.OnCardClicked -= SelectCardInHand;      
            lastClickedCard = null;
    }

    //Guardamos la carta clickeada en la variable lastClickedCard
    public void SelectCardInHand(GameObject card)
    {   
        //Si no esta en la mano no guardamos la informacion, solo queremos cartas de la mano
        if (card.transform.IsChildOf(hand.transform) != hand)
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
        card.transform.SetParent(deck.transform);
        card.transform.localPosition = new Vector3(515,0,0);
    }

    //Poniendo carta lider en el campo
    public void SetLead()
    {
        //GameObject g = Instantiate(cardLeadPrefab, lead.transform);
        //g.GetComponent<Card>().cardData = deck.GetComponent<Deck>().leadCard;
        //g.name = g.GetComponent<Card>().cardData.cardName;
    }
    //Robar Carta del Deck
    public void DrawCard(int amount)       
    {
        for (int i = 0; i < amount; i++)     
        {   
            if (hand.transform.GetComponentsInChildren<Card>(true).Length < 10)
            {   
                GameObject card = deck.transform.GetChild(1).gameObject;
                card.transform.SetParent(hand.transform);
            }
            else
            {
                GameObject card = deck.transform.GetChild(1).gameObject;
                card.transform.SetParent(graveyard.transform);
            }                   
        }     
    }

    //Contar la cantidad de ataque que existe en el campo
    public void CountAttackOnField()
    {   
        //applyAumento();
        foreach (Transform child in meleeZone.transform)
        {
            power += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in rangeZone.transform)
        {
            power += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in siegeZone.transform)
        {
            power += child.GetComponent<Card>().attackPower;
        }
    }
    
    public void ShowCardBack()
    {
        if (owner == player.Player1)
        {
            GameObject[] cards = GameObject.FindGameObjectsWithTag("Card Player1");

            foreach (GameObject card in cards)
            {
                if (card.transform.parent.name != "Hand p1") continue;
                else
                {
                    card.GetComponent<Card>().displayCardBack = true;
                }
            }
        }

        else if (owner == player.Player2)
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
        
        
    }
    public void HideCardBack()
    {
        if (owner == player.Player1)
        {
            GameObject[] cards = GameObject.FindGameObjectsWithTag("Card Player1");

            foreach (GameObject card in cards)
            {
                if (card.transform.parent.name != "Hand p1") continue;
                else
                {
                    card.GetComponent<Card>().displayCardBack = false;
                }
            }
        }
        else if (owner == player.Player2)
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
    }
    
    public void Start()
    {
       deck.GetComponent<Deck>().ShuffleDeck();
       SetLead();
    }

    
    public void Update()
    {
        deckText.text = (deck.transform.childCount - 1).ToString();
        discardText.text = (graveyard.transform.childCount - 1).ToString();
    }
 }