using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class GameManager : MonoBehaviour
{
    public int numberOfRounds = 1;
    public int numberOfActionsAvailable = 1;

    public int player1WinedRounds = 0;
    public int player2WinedRounds = 0;

    public bool player1StartTheRound = true;

    public bool player1pass;
    public bool player2pass;

    public GameObject handp1;
    public GameObject handp2;

    public gameTracker state;
    public static bool player1;
    public static bool player2;

    public void ChangeTurn()
    {
        if (state == gameTracker.Player1Turn) state = gameTracker.Player2Turn;
        else if (state == gameTracker.Player2Turn) state = gameTracker.Player1Turn;

        numberOfActionsAvailable = 1;
    }

    public void whoWinsRound()
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
        else 
        {
            player1StartTheRound = false;
            player1WinedRounds ++;
            player2WinedRounds ++;
        }
    }

    public void EndRound()
    {
        GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().CountAttackOnField();
        GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().CountAttackOnField();
        state = gameTracker.FinalOfRound;
        whoWinsRound();
    }

    public void PassTurn()
    {
        if (state == gameTracker.Player1Turn)
        {   
            player1pass = true;
            if (player1pass == player2pass) EndRound();

            ChangeTurn();
        }
         
        else if (state == gameTracker.Player2Turn) 
        {
            player2pass = true;
            if (player2pass == player1pass) EndRound();

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
                player1pass = false;
                player2pass = false;              
                GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().powerPlayer1 = 0;
                GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().powerPlayer2 = 0;

                if (numberOfRounds == 1)
                {
                    GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().DrawCard(10);
                    GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().DrawCard(10);
                } 
                else 
                { 
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
                //
                break;
        }
    }
}