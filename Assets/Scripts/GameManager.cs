using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class GameManager : MonoBehaviour
{
    public int numberOfRounds = 1;
    public int numberOfActionsAvailable = 1;

    public static bool firstTurnOfTheRoundPlayer1 = true;
    public static bool firstTurnOfTheRoundPlayer2 = true;

    public int player1WinedRounds = 0;
    public int player2WinedRounds = 0;

    public bool player1StartTheRound = true;

    public bool player1Pass;
    public bool player2Pass;
    
    public GameObject leadP1;
    public GameObject leadP2;
    public GameObject handP1;
    public GameObject handP2;

    public gameTracker state;
    public static bool player1;
    public static bool player2;

    public void ChangeTurn()
    {
        if (state == gameTracker.Player1Turn)
        {
            if (player2Pass == true) state = gameTracker.Player1Turn;
            else state = gameTracker.Player2Turn;

            firstTurnOfTheRoundPlayer1 = false;
        } 
        else if (state == gameTracker.Player2Turn)
        {   
            if (player1Pass == true) state = gameTracker.Player2Turn;
            else state = gameTracker.Player1Turn;

            firstTurnOfTheRoundPlayer2 = false;
        }

        numberOfActionsAvailable = 1;
    }

    public void WhoWinsRound()
    {
        if (GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().powerPlayer1 > GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().powerPlayer2) 
        {
            //Player 1 Wins!
            player1StartTheRound = true;
            player1WinedRounds ++;
        }
        else if (GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().powerPlayer1 < GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().powerPlayer2)
        {
            player1StartTheRound = false;
            player2WinedRounds ++;
        }
        else //Esto significa que quedo en empate
        {
            player1StartTheRound = false;
            //Revisamos si algun lider es The Lich King, si lo es entonces el dueño de el gana la ronda
            if (leadP1.transform.GetChild(0).GetComponent<Card>().cardName == "The Lick King")
            {
                player1WinedRounds ++;
            }
            else if (leadP2.transform.GetChild(0).GetComponent<Card>().cardName == "The Lick King")
            {
                player2WinedRounds ++;
            }
            //Si nadie lo tiene entonces empate 
            else
            {
                player1WinedRounds ++;
                player2WinedRounds ++;
            }       
        }
    }

    public void EndRound()
    {
        GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().applyClima();
        GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().applyClima();
        GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().CountAttackOnField();
        GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().CountAttackOnField();
        state = gameTracker.FinalOfRound;
        WhoWinsRound();
    }

    public void PassTurn()
    {
        if (state == gameTracker.Player1Turn)
        {   
            player1Pass = true;
            if (player1Pass == player2Pass) EndRound();

            ChangeTurn();
        }
         
        else if (state == gameTracker.Player2Turn) 
        {
            player2Pass = true;
            if (player2Pass == player1Pass) EndRound();

            ChangeTurn();
        }
    }

    public void WhoWinsTheGame()
    {
        if (player1WinedRounds > player2WinedRounds)
        {
            Debug.Log("Player 1 WINS");
        }
        else if (player1WinedRounds < player2WinedRounds)
        {
            Debug.Log("Player 2 WINS");
        }
        else 
        {
            Debug.Log("DRAW!!!!");
        }
    }

    void Start()
    {
         state = gameTracker.StartingTheGame;         
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case gameTracker.StartingTheGame:

                state = gameTracker.StartingRound;
                break;

            case gameTracker.StartingRound:
                player1Pass = false;
                player2Pass = false;              
                GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().powerPlayer1 = 0;
                GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().powerPlayer2 = 0;

                if (numberOfRounds == 1)
                {
                    GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().DrawCard(10);
                    GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().DrawCard(10);
                } 
                else 
                {
                    
                    if (leadP1.transform.GetChild(0).GetComponent<Card>().cardName == "Deathwing")
                    {
                        GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().DrawCard(1);
                    }
                    else if (leadP2.transform.GetChild(0).GetComponent<Card>().cardName == "Deathwing")
                    {
                        GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().DrawCard(1);
                    }

                    GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().CleanField();
                    GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().CleanField();                  
                    GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().DrawCard(2);
                    GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().DrawCard(2);
                }

                if (player1StartTheRound == true)
                {
                    state = gameTracker.Player1Turn;
                }
                else
                {
                    state = gameTracker.Player2Turn;
                }
                
                break;

            case gameTracker.Player1Turn:
                player1 = true;
                player2 = false;
                break;

            case gameTracker.Player2Turn:
                player1 = false;
                player2 = true;
                break;

            case gameTracker.FinalOfRound:
                numberOfRounds ++;
                state = gameTracker.StartingRound;
                if (player1WinedRounds == 2 || player2WinedRounds == 2) 
                {
                    GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().CleanField();
                    GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().CleanField();
                    WhoWinsTheGame();
                    state = gameTracker.GameOver;
                }
                break;
            case gameTracker.GameOver:
                //Se acabo jejeje
                break;
        }
    }
}