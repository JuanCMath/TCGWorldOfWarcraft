using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.PackageManager;
using UnityEngine.EventSystems;
using UnityEditor.UI;
using System.Runtime.CompilerServices;
using System.Linq;
using Unity.VisualScripting;
using Enums;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Unity.VisualScripting.Antlr3.Runtime;

public class EffectsManager : MonoBehaviour
{
    static GameObject selectedcard;
    static GameObject panel;

    //Efecto Clima
    public static void ClimateEffect(GameObject panel, int number) //panel es el panel donde esta el clima, y number la cantidad de ataque que modifica el clima
    {   
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1"); //Cartas Jugador 1
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2"); //Cartas Jugador 2

        //Este if verifica de quien es el clima y en base a eso modifica cartas
        if (panel.transform.parent.name == GameObject.Find("Panels p1").transform.name) 
        {
            foreach (GameObject card in cardsP2) //Para cada carta del jugador 2
            {   
                if (card.transform.parent.ToString() == "Hand p2") continue; //Si esta en la mano se ignora
                else if (card.GetComponent<Card>().isHero == true) continue; //Si es Heroe se ignora
                else 
                {
                    card.GetComponent<Card>().attackPower -= number;    //Aplicamos efecto clima
                    if (card.GetComponent<Card>().attackPower < 0) card.GetComponent<Card>().attackPower = 0; //Si se llega a un ataque menor que 0 dejamos en 0
                }
            }
        }

        else if (panel.transform.parent.name == GameObject.Find("Panels p2").transform.name)
        {
            foreach (GameObject card in cardsP1) //Para cada carta del jugador 1
            {
                if (card.transform.parent.ToString() == "Hand p1") continue; //Si esta en la mano se ignora
                else if (card.GetComponent<Card>().isHero == true) continue; //Si es Heroe se ignora
                else 
                {
                    card.GetComponent<Card>().attackPower -= number;   //Aplicamos efecto clima
                    if (card.GetComponent<Card>().attackPower < 0) card.GetComponent<Card>().attackPower = 0; //Si se llega a un ataque menor que 0 dejamos en 0
                }   
            }
        }
    }

    //Efecto Aumento
    public static void IncreaseEffect(GameObject ouputPanel, int number) //outputPanel es el panel donde el aumento hace efecto, number la cantidad de ataque que modifica el aumento
    {
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1"); //Cartas jugador 1
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2"); //Cartas jugador 2

        //Verificamos de que jugador es el aumento
        if (ouputPanel.transform.parent.name == GameObject.Find("Panels p1").transform.name)
        {
            foreach (GameObject card in cardsP1)    //Para cada carta del jugador 1
            {
                if (card.transform.IsChildOf(ouputPanel.transform)) //Si pertenece al output panel
                {
                    if (card.GetComponent<Card>().isHero == true) continue; //Ignoramos si es heroe
                    else card.GetComponent<Card>().attackPower += number;   //Aplicamos aumento          
                }
            }
        }
        else if(ouputPanel.transform.parent.name == GameObject.Find("Panels p2").transform.name)
        {
            foreach (GameObject card in cardsP2) //Para cada carta del jugador 2
            {
                if (card.transform.IsChildOf(ouputPanel.transform)) //Si pertenece al output panel
                {
                    if (card.GetComponent<Card>().isHero == true) continue; //Ignoramos si es heroe
                    else card.GetComponent<Card>().attackPower += number;   //Aplicamos aumento 
                }
            }
        }
    }

    //Efecto Despeje
    public void ClearanceEffect(GameObject panelOfTheDropedCard) //Vemos en que panel se dropeo la carta
    {
        panel = panelOfTheDropedCard;
        Debug.Log("aplicando efecto Despeje");
        //Seleccionar una carta
        EventManager.OnCardClicked += SelectCard;
        //y devolverla a la mano
        StartCoroutine(SendCardToHand());  
    }

    //Efecto Señuelo
    public void BaitEffect(GameObject panelOfTheDropedCard) //Vemos en que panel se dropeo la carta
    {
        panel = panelOfTheDropedCard;
        Debug.Log("aplicando efecto señuelo");
        //Seleccionar una carta 
        EventManager.OnCardClicked += SelectCard;
        //y devolverla a la mano
        StartCoroutine(SendCardToHand());
    }

    //Efecto de robo de carta
    public static void DrawACard()
    {
        if (GameManager.player1 == true)  GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().DrawCard(1);  
        else if (GameManager.player2 == true) GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().DrawCard(1);
    }

    //Efecto Destruir Carta con menor poder de ataque del oponente
    public static void DestroyLowerPowerCardOnOponent()
    {
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1"); //Cartas Jugador 1
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2"); //Cartas Jugador 2 

        //Inicializando las condiciones para hayar la carta con menor ataque
        GameObject cardToDestroy = null;
        int attackPowerOfCardToDestroy = 1000;

        //Segun el jugador que este jugando escogeremos un campo o el otro
        //Iteramos por cada carta comparando el ataque de ella con la anterior
        //Si es menor entonces escogemos esa carta para destruirla
        if (GameManager.player1 == true)
        {
            foreach (GameObject card in cardsP2)
            {
                if (card.GetComponent<Card>().cardType == type.Unidad && card.GetComponent<Card>().isHero == false)
                {
                    if (card.transform.parent.name == "Hand p2") continue;
                    else if (card.transform.parent.name == "Graveyard p2") continue;
                    else if (card.transform.parent.name == "Lider p2") continue;
                         
                    if (card.GetComponent<Card>().attackPower < attackPowerOfCardToDestroy) //Comparando con la anterior
                    {
                        attackPowerOfCardToDestroy = card.GetComponent<Card>().attackPower;                            
                        cardToDestroy = card;
                    }
                }  
            }            
            SendCardToGraveyard(cardToDestroy);  
        }
        else if (GameManager.player2 == true)
        {   
            foreach (GameObject card in cardsP1)
            {
                if (card.GetComponent<Card>().cardType == type.Unidad && card.GetComponent<Card>().isHero == false)
                {
                    if (card.transform.parent.name == "Hand p1") continue;
                    else if (card.transform.parent.name == "Graveyard p1") continue;
                    else if (card.transform.parent.name == "Lider p1") continue;
                         
                    if (card.GetComponent<Card>().attackPower < attackPowerOfCardToDestroy)
                    {
                        attackPowerOfCardToDestroy = card.GetComponent<Card>().attackPower;                            
                        cardToDestroy = card;
                    }
                }
            }
            SendCardToGraveyard(cardToDestroy);
        }    
    }

    //Efecto Destruir Cara con mayor poder de ataque del campo
    public static void DestroyHighestPowerCardOnField(GameObject dropedcard)
    {   
        //Iniciando condiciones
        GameObject cardToDestroy = null;
        int attackPowerOfCardToDestroy = 0;
        
        //Hallando todas las cartas en el juego
        Card[] provisionalCards = FindObjectsOfType<Card>();
        // Convierte el array de Card a GameObject[]
        GameObject[] cards = new GameObject[provisionalCards.Length];
        for (int i = 0; i < provisionalCards.Length; i++)
        {
            cards[i] = provisionalCards[i].gameObject;
        }

        //Buscamos la carta con mayor poder de ataque en el campo,
        //comparando el ataque de cada una con attackPowerOfCardToDestroy, y si es mayor escogemos esa carta
        foreach (GameObject card in cards)//Iteramos por cada carta
        {
            if (card.GetComponent<Card>().cardType == type.Unidad && card.GetComponent<Card>().isHero == false)
            {
                if (card.transform.parent.name == "Hand p1" || card.transform.parent.name == "Hand p2") continue;
                else if (card.transform.parent.name == "Graveyard p1" || card.transform.parent.name == "Graveyard p2") continue;
                else if (card.transform.parent.name == "Lider p1" || card.transform.parent.name == "Lider p2") continue;
                else if (card == dropedcard) continue;
                else 
                {
                    if (card.GetComponent<Card>().attackPower > attackPowerOfCardToDestroy) //Si es mayor que la que teniamos anteriormente escogemos esa
                    {
                        attackPowerOfCardToDestroy = card.GetComponent<Card>().attackPower;
                        cardToDestroy = card; 
                    }
                }
            }
        }
        SendCardToGraveyard(cardToDestroy);    
    }

    //Efecto Aumentar ataque de la carta segun la cantidad iguales a ella en el campo
    public static void MultAttackPower(GameObject dropedcard)
    {
        string multCardName = dropedcard.GetComponent<Card>().cardName; //Guardamos el nombre de la carta dropeada
        int amountOfCardsWithTheSameName = 0;   //Cantidad de ellas en el campo

        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1"); //Cartas jugador 1
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2"); //Cartas jugador 2

        //Chequeamos que jugador esta jugando, respecto a esto activamos el efecto en un campo o en el otro
        if (GameManager.player1 == true)
        {
            foreach (GameObject card in cardsP1)
            {
                if (card.transform.parent.name == "Hand p1") continue;

                else if (card.GetComponent<Card>().cardName == multCardName) //Si encontramos una carta con el mismo nombre que la dropeada aumentamos 1 el contador
                    {
                        amountOfCardsWithTheSameName += 1;
                    }
            }
            dropedcard.GetComponent<Card>().attackPower *= amountOfCardsWithTheSameName; //Multiplicamos el ataque de la carta dropeada por la cantidad de cartas iguales a ella  
        }
        else if (GameManager.player2 == true)
        {
            foreach (GameObject card in cardsP2)
            {
                if (card.transform.parent.name == "Hand p2") continue;

                else if (card.GetComponent<Card>().cardName == multCardName) //Si encontramos una carta con el mismo nombre que la dropeada aumentamos 1 el contador
                {
                    amountOfCardsWithTheSameName += 1;
                }
            }
            dropedcard.GetComponent<Card>().attackPower *= amountOfCardsWithTheSameName; //Multiplicamos el ataque de la carta dropeada por la cantidad de cartas iguales a ella  
        }
    }

    //Efecto Destruir la fila con menor cantidad de cartas
    public static void DestoyFieldSpotWithLowerAmountOfCards()
    {
        //Iniciando condiciones
        int amountOfCardsInPanelToDestroyedPanel = 0;
        GameObject panelToDestroyed = null;

        GameObject meleePlayer1 = GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().meleeZonePlayer1;
        GameObject rangePlayer1 = GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().rangeZonePlayer1;
        GameObject siegePlayer1 = GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().siegeZonePlayer1;
        
        GameObject meleePlayer2 = GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().meleeZonePlayer2;
        GameObject rangePlayer2 = GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().meleeZonePlayer2;
        GameObject siegePlayer2 = GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().meleeZonePlayer2;

        GameObject[] panels = new GameObject[] {meleePlayer1, rangePlayer1, siegePlayer1, meleePlayer2, rangePlayer2, siegePlayer2};

        foreach (GameObject panel in panels) //Obtenemos el panel con menos cartas
        {
            if (panel.transform.childCount > amountOfCardsInPanelToDestroyedPanel)
            {
                panelToDestroyed = panel;
                amountOfCardsInPanelToDestroyedPanel = panel.transform.childCount;
            }
        }
        for (int i = 0; i < amountOfCardsInPanelToDestroyedPanel; i++) //Iteramos por cada carta y la destruimos
        {
            SendCardToGraveyard(panelToDestroyed.transform.GetChild(i).gameObject);
        }
    }

    //Efecto para igualar ataque de todas las cartas del campo a su promedio
    public static void SetAttackPowerOfAllCardsToAverageAtackPower()
    {
        //Obtenemos la lista de todas las cartas en el campo
        Card[] provisionalCards = FindObjectsOfType<Card>();
        // Convierte el array de Card a GameObject[]
        GameObject[] cards = new GameObject[provisionalCards.Length];
        for (int i = 0; i < provisionalCards.Length; i++)
        {
            cards[i] = provisionalCards[i].gameObject;
        }

        int amountOfCardsInField = 0;
        int sumOfAllAttackPower = 0;
        int averageAtackPower;

        foreach (GameObject card in cards) //Calculamos la cantidad de cartas que hay y la suma total de sus ataques
        {
            if (card.GetComponent<Card>().cardType == type.Unidad && card.GetComponent<Card>().isHero == false && card.GetComponent<Card>().cardType != type.Señuelo)
            { 
                if (card.transform.parent.name == "Hand p1" || card.transform.parent.name == "Hand p2") continue;
                else if (card.transform.parent.name == "Graveyard p1" || card.transform.parent.name == "Graveyard p2") continue;
                else if (card.transform.parent.name == "Lider p1" || card.transform.parent.name == "Lider p2") continue;
                else
                {
                    amountOfCardsInField += 1;
                    sumOfAllAttackPower += card.GetComponent<Card>().attackPower;
                }
            }
        }
        averageAtackPower = sumOfAllAttackPower % sumOfAllAttackPower;  //Hallamos el promedio

        foreach (GameObject card in cards) //Igualamos el ataque de todas las cartas al promedio
        {
            if (card.GetComponent<Card>().cardType == type.Unidad && card.GetComponent<Card>().isHero == false && card.GetComponent<Card>().cardType != type.Señuelo)
            { 
                if (card.transform.parent.name == "Hand p1" || card.transform.parent.name == "Hand p2") continue;
                else if (card.transform.parent.name == "Graveyard p1" || card.transform.parent.name == "Graveyard p2") continue;
                else if (card.transform.parent.name == "Lider p1" || card.transform.parent.name == "Lider p2") continue;
                else
                {
                    card.GetComponent<Card>().attackPower = averageAtackPower;
                }
            }
        }
    }

    #region Utilidades
    //Metodo para seleccionar una carta del mismo panel que una carta dada
    public static void SelectCard(GameObject card)
    {
        if (card.transform.IsChildOf(panel.transform))
        {
            if (card.GetComponent<Card>().isHero == true)
            {
                Debug.Log("Cartas Heroes No son Afectadas");
            }
            else
            {
                selectedcard = card;
            }
        }
        else
        {
            Debug.Log("La carta debe estar en el mismo panel");
        }
    }

    //Coroutine para enviar la carta seleccionada a la mano
    IEnumerator SendCardToHand()
    {
        yield return new WaitUntil(() => selectedcard != null); //Esperando a que la carta sea seleccionada

        ReturnCardToHand(selectedcard); //Returnandola a la mano

        EventManager.OnCardClicked -= SelectCard; //Removiendo el evento Select card, no queremos fuera de estos efectos que cuando se clikee una carta inicie el metodo
        //Una vez terminada la accion reseteamos los parametros de la carta
        selectedcard = null;
        panel = null;
    }

    //Enviar una carta a la mano
    public void ReturnCardToHand(GameObject card)
    {
        //Buscando que player esta jugando y cambiando la carta del campo a su mano
        if (GameManager.player1 == true)
        {
            selectedcard.transform.SetParent(GameObject.Find("Game Manager").GetComponent<GameManager>().handP1.transform);
        }
        else if (GameManager.player2 == true)
        {
            selectedcard.transform.SetParent(GameObject.Find("Game Manager").GetComponent<GameManager>().handP2.transform);
        } 
    }

    //Enviar una carta al cementerio
    public static void SendCardToGraveyard(GameObject card)
    {
        if (card != null)
        {
            if (card.transform.parent.parent.name == "Panels p1")
            {
                card.transform.SetParent(GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().graveyardPlayer1.transform);
                card.transform.localPosition = new Vector3 (0,0,0);
            }
            else if (card.transform.parent.parent.name == "Panels p2")
            {
                card.transform.SetParent(GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().graveyardPlayer2.transform);
                card.transform.localPosition = new Vector3 (0,0,0);
            }
        }
        else
        {
            return;
        }
        
    }
    #endregion
}