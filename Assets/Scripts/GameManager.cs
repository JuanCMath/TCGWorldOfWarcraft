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
    
    public gameTracker state;
    public static bool player1;
    public static bool player2;

    public void ChangeTurn()
    {
        if (state == gameTracker.Turn1) state = gameTracker.Turn2;
        else if (state == gameTracker.Turn2) state = gameTracker.Turn1;

        numberOfActionsAvailable = 1;
    }

    public void whoWinsRound()
    {
        if (GetComponent<Player1CardManager>().powerPlayer1 > 10) //el 10 esta representando powerPLayer2
        {
            //Player 1 Wins!
            player1WinedRounds ++;
        }
        else if (GetComponent<Player1CardManager>().powerPlayer1 < 10)
        {
            //Player 2 wins!
            player2WinedRounds ++;
        }
        else 
        {
            //Draw
            player1WinedRounds ++;
            player2WinedRounds ++;
        }
    }

    public void EndRound()
    {

    }

    void Start()
    {
         state = gameTracker.Starting;         
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case gameTracker.Starting:
                //Metodo de llevar cartas a la mano
                state = gameTracker.Playing;
                break;
            case gameTracker.Playing:
                state = gameTracker.Turn1;
                break;
            case gameTracker.Turn1:
                player1 = true;
                player2 = false;
                break;
            case gameTracker.Turn2:
                player1 = false;
                player2 = true;
                break;
            case gameTracker.FinalOfRound:
                numberOfRounds ++;
                state = gameTracker.Turn1;
                break;
            case gameTracker.GameOver:
                //
                break;
        }
    }
}
