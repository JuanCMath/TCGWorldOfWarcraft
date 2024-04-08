using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum gameTracker
    {
                Starting, Playing, Turn1, Turn2, GameOver
    }
    public gameTracker state;
    public static bool player1;
    public static bool player2;
           
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
            case gameTracker.GameOver:
                break;
        }
    }
}
