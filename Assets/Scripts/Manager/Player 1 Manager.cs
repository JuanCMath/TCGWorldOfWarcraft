using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using System.Threading;
using UnityEngine.Assertions.Must;

public class Player1Manager : MonoBehaviour
{
    #region Variables
    public int powerPlayer1;
    private GameObject lastClickedCard = null;

    public GameObject cardPrefab1;
    
    public GameObject deckPlayer1;
    public GameObject graveyardPlayer1;
    public GameObject handPlayer1;

    public GameObject meleeZonePlayer1;
    public GameObject rangeZonePlayer1;
    public GameObject siegeZonePlayer1;

    [Header ("Hand Counter")]
    public int startingHandSize = 10;
    public int maxHandSize = 10;

    [Header ("UI")]
    public TextMeshProUGUI deckTextPlayer1;
    public TextMeshProUGUI discardTextPlayer1;
    #endregion

    public void CleanField()
    {
        GameObject[] cartas = GameObject.FindGameObjectsWithTag("Card Player1");

        foreach (GameObject carta in cartas)
        {
            //Si esta en la mano no la destruyas
            if (carta.transform.IsChildOf(handPlayer1.transform)) continue;

            //Si no esta en la mano destruyela, Aqui podriamos poner despues que se vayan al cementerio
            carta.transform.SetParent(graveyardPlayer1.transform);
            carta.transform.localPosition = new Vector3(0,0,0);
        }
        
    }

    //Cambiar Cartas
    public void SwapCards() //Aqui en algun momento pondre la cantidad de cartas a Swapear, no es por gusto el metodo
    {
        for (int i = 0; i < 2; i++)
        {
            StartingTheCardSwap();
        }
    }
    
    //Metodo para iniciar el cambio de cartas en la mano
    public void StartingTheCardSwap()       //Lo acciona un botton, aqui se dan las condiciones previas para el intercambio
    {
        //Añadir un Listener a los Eventos
        EventManager.OnCardClicked += SetCardClicked;
        //Inicio del metodo para cambiar cartas
        StartCoroutine(SwapCardsInHands());      
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
            Debug.Log("Carta Devuelta al deck, seleccione otra por favor");
            //Eliminamos el Event Listener ya que no queremos que fuera de este metodo el click guarde informacion
            EventManager.OnCardClicked -= SetCardClicked;      
            lastClickedCard = null; 
    }

    //Guardamos la carta clickeada en la variable lastClickedCard
    public void SetCardClicked(GameObject card)
    {   
        //Si no esta en la mano no guardamos la informacion, solo queremos cartas de la mano
        if (card.transform.IsChildOf(handPlayer1.transform) != handPlayer1)
        {
            Debug.Log("Debe seleccionar una carta de la mano");
        }
        else
        {
            lastClickedCard = card;
            Debug.Log(lastClickedCard.GetComponent<Card>().cardName);
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
        deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.Add(Resources.Load<CardData>("Scriptable Objects/Aspectos Deck/" + name));
        //Barajeamos el deck
        ShuffleDeck(deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck);
    }

    //Robar Carta del Deck
    public void DrawCard(int amount)       
    {
        for (int i = 0; i < amount; i++)     
        {   
            if (handPlayer1.transform.childCount <= 10)
            {       
                //Instanciando la carta con el prefab y en la posicion de la mano
                GameObject g = Instantiate(cardPrefab1, handPlayer1.transform);                         
                //Dandole a cada prefab de carta los datos de los scriptable objects
                g.GetComponent<Card>().cardData = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck[i];
                //Eliminando la carta robada
                deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.RemoveAt(i);                          
                //Dandole un nombre a la carta en el inspector
                g.name = g.GetComponent<Card>().cardData.cardName;  
            }
            else
            {
                //Instanciando la carta con el prefab y en la posicion de la mano
                GameObject g = Instantiate(cardPrefab1, graveyardPlayer1.transform);           
                g.transform.localPosition = new Vector3(0,0,0);              
                //Dandole a cada prefab de carta los datos de los scriptable objects
                g.GetComponent<Card>().cardData = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck[i];
                //Eliminando la carta robada
                deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.RemoveAt(i);                          
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
        foreach (Transform child in meleeZonePlayer1.transform)
        {
            powerPlayer1 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in rangeZonePlayer1.transform)
        {
            powerPlayer1 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in siegeZonePlayer1.transform)
        {
            powerPlayer1 += child.GetComponent<Card>().attackPower;
        }
    }

    public void Start()
    {
       ShuffleDeck(deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck);
    }

    
    public void Update()
    {
       deckTextPlayer1.text = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.Count.ToString();
       discardTextPlayer1.text = (graveyardPlayer1.transform.childCount - 1).ToString();
    }
 }