using UnityEngine;
using Enums;
using TMPro;

public class GameManager : MonoBehaviour
{ 
    #region Variables
    [Header("Show info")]
    public TextMeshProUGUI actualRound;
    public TextMeshProUGUI currentTurn;
    public TextMeshProUGUI player1Won;
    public TextMeshProUGUI player2Won;
    public TextMeshProUGUI player1CanSwapCardsText;
    public TextMeshProUGUI player2CanSwapCardsText;
    public TextMeshProUGUI player1ShowingPower;
    public TextMeshProUGUI player2ShowingPower;
    public TextMeshProUGUI whoWonDescription;

    [Header("Pass Manager")]
    public bool player1Pass;
    public bool player2Pass;
    
    [Header("Scene Objects")]
    public GameObject leadP1;
    public GameObject leadP2;
    public GameObject handP1;
    public GameObject handP2;
    public GameObject cardDisplayPrefab;
    public GameObject panelCardDsiplay;
    public GameObject endGameMenu;

    [Header("Game Trackers")]
    public gameTracker state;
    public int numberOfRounds = 1;
    public int numberOfActionsAvailable = 1;
    public static bool player1;
    public static bool player2;
    public int player1WinedRounds = 0;
    public int player2WinedRounds = 0;

    public static bool player1CanSwapCards = true;
    public static bool player2CanSwapCards = true;

    public static bool player1StartTheRound = true;
    #endregion

    public void ShowAttackOnField()
    {
        player1ShowingPower.text = Player1AttackOnField().ToString();
        player2ShowingPower.text = Player2AtacckOnField().ToString();
    }

    public int Player1AttackOnField()
    {
        int helper1 = 0;
        GameObject[] cartasp1 = GameObject.FindGameObjectsWithTag("Card Player1");

        foreach (GameObject carta in cartasp1)
        {
            //Si esta en la mano no la destruyas
            if (carta.transform.IsChildOf(handP1.transform)) continue;
            else if (carta.transform.IsChildOf(leadP1.transform)) continue;
            else if (carta.transform.IsChildOf(panelCardDsiplay.transform)) continue;

            //Si no esta en la mano destruyela, Aqui podriamos poner despues que se vayan al cementerio
            helper1 += carta.GetComponent<Card>().attackPower;
        }
        return helper1;
    }

    public int Player2AtacckOnField()
    {
        int helper2 = 0;
        GameObject[] cartasp2 = GameObject.FindGameObjectsWithTag("Card Player2");

        foreach (GameObject carta in cartasp2)
        {
            //Si esta en la mano no la destruyas
            if (carta.transform.IsChildOf(handP2.transform)) continue;
            else if (carta.transform.IsChildOf(leadP2.transform)) continue;
            else if (carta.transform.IsChildOf(panelCardDsiplay.transform)) continue;
            //Si no esta en la mano destruyela, Aqui podriamos poner despues que se vayan al cementerio
            helper2 += carta.GetComponent<Card>().attackPower;
        }
        return helper2;
    }
    //Cambiar de turno
    public void ChangeTurn()
    {   
        //Si es Turno del jugador 1 pasamos al jugador 2, pero si este paso seguimos en jugador 1
        if (state == gameTracker.Player1Turn)
        {
            if (player2Pass == true) state = gameTracker.Player1Turn;
            else state = gameTracker.Player2Turn;

            player1CanSwapCards = false;
        } 
        //Si es Turno del jugador 2 pasamos al jugador 1, pero si este paso seguimos en jugador 2
        else if (state == gameTracker.Player2Turn)
        {   
            if (player1Pass == true) state = gameTracker.Player2Turn;
            else state = gameTracker.Player1Turn;

            player2CanSwapCards = false;
        }
        //Una vez pasado el turno reseteamos la cantidad de acciones disponibles
        numberOfActionsAvailable = 1;
    }

    //Quien gana la ronda?
    public void WhoWinsRound()
    {   
        //Comparamos ataque total en el campo de cada jugador
        if (GameObject.Find("Player1").GetComponent<PlayerManager>().power > GameObject.Find("Player2").GetComponent<PlayerManager>().power) 
        {
            //Player 1 Wins!
            player1StartTheRound = true;
            player1WinedRounds ++;
        }
        else if (GameObject.Find("Player1").GetComponent<PlayerManager>().power < GameObject.Find("Player2").GetComponent<PlayerManager>().power)
        {
            //PLayer 2 Wins!
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

    //Metodo para terminar la ronda
    public void EndRound()  //Seteamos las condiciones para el final de ronda
    {
        //Aplicamos clima
        GameObject.Find("Player1").GetComponent<PlayerManager>().applyClima();
        GameObject.Find("Player2").GetComponent<PlayerManager>().applyClima();
        //Contamos el ataque de cada jugador en el campo
        GameObject.Find("Player1").GetComponent<PlayerManager>().CountAttackOnField();
        GameObject.Find("Player2").GetComponent<PlayerManager>().CountAttackOnField();

        state = gameTracker.FinalOfRound;
        WhoWinsRound();
    }

    //Pasar turno
    public void PassTurn()
    {
        //Si es Turno del jugador 1 entonces este no puede jugar mas,
        if (state == gameTracker.Player1Turn)
        {   
            player1Pass = true; //Aqui reconocemos que este jugador ya paso, no puede jugar mas
            if (player1Pass == player2Pass) EndRound(); //Si ambos ya pasaron es hora de terminar la ronda

            ChangeTurn(); //Si no cambiamos de turno
        }
        //Si es Turno del jugador 2 pasamos al jugador 1, pero si este paso seguimos en jugador 2
        else if (state == gameTracker.Player2Turn) 
        {
            player2Pass = true; //Aqui reconocemos que este jugador ya paso, no puede jugar mas
            if (player2Pass == player1Pass) EndRound(); //Si ambos ya pasaron es hora de terminar la ronda

            ChangeTurn(); //Si no cambiamos de turno
        }
    }

    //Quien gana el juego?
    public void WhoWinsTheGame()
    {
        //Comparamos la cantidad de ronda ganadas por cada jugador, el de mayor cantidad gana
        if (player1WinedRounds > player2WinedRounds)
        {
            whoWonDescription.text = "The Aspects have won the war!, now the world is finally pacified thanks to you! what u will like to do next?";
            endGameMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (player1WinedRounds < player2WinedRounds)
        {
            whoWonDescription.text = "The Plague have conquerer the world, now we will never live in peace, want to try again?";
            endGameMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else 
        {
            whoWonDescription.text = "The War is NOT over";
            endGameMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    //Mostrar Informacion al usuario
    public void UpdateInfo()
    {   
        //Mostrar Ronda Actual
        actualRound.text= "Current Round: " + numberOfRounds;
        //Mostrar Si el Player 1 puede cambiar cartas
        if (state == gameTracker.Player1Turn)
        {
            if (player1CanSwapCards == true)
            {
                if (numberOfActionsAvailable == 1 )  player1CanSwapCardsText.text = "<- Can Swap";
                else
                {
                    player1CanSwapCardsText.text = "";
                }
            }
            else
            {
                player1CanSwapCardsText.text = "";
            }
        }
        else
        {
            player1CanSwapCardsText.text = "";
        }
        //Mostrar si el player 2 puede cambiar cartas
        if (state == gameTracker.Player2Turn)
        {
            if (player2CanSwapCards == true)
            {
                if (numberOfActionsAvailable == 1 )  player2CanSwapCardsText.text = "<- Can Swap";
                else
                {
                    player2CanSwapCardsText.text = "";
                }
            }
            else
            {
                player2CanSwapCardsText.text = "";
            }
        }
        else
        {
            player2CanSwapCardsText.text = "";
        }
        //Mostrar actual turno
        if (player1 == true)
        {
            currentTurn.text = "Current Turn: Player 1";
        }
        else
        {
            currentTurn.text = "Current Turn: Player 2";
        }

        //Mostrar cuantas rondas ha ganado cada uno
        player1Won.text = "Player 1 Won:" + player1WinedRounds + " Rounds";
        player2Won.text = "Player 2 Won:" + player2WinedRounds + " Rounds";

        ShowAttackOnField();
    }

    public void ResetVar()
    {
        player1Pass = false;
        player2Pass = false;
        numberOfRounds = 1;
        numberOfActionsAvailable = 1;
        player1 = false;
        player2 = false;
        player1WinedRounds = 0;
        player2WinedRounds = 0;
        player1CanSwapCards = true;
        player2CanSwapCards = true;
        player1StartTheRound = true;
    }

    //Inicial el juego
    void Start()
    {
        state = gameTracker.StartingTheGame;         
    }

    //
    void Update()
    {
        switch (state) //Este switch sera el controlador principal del juego, todas las fases del mismo estan mostradas aqui
        {
            //Empezando el juego, aqui pondre algunas animaciones e inputs a los usuarios para poner su nombre
            case gameTracker.StartingTheGame:
                endGameMenu.SetActive(false);
                state = gameTracker.ChoosingFaction;
                break;

            case gameTracker.ChoosingFaction: //Aqui se mostrara la pantalla de seleccion de faccion
                //TODO
                GameObject.Find("Deck p1").GetComponent<Deck>().LoadDeck();
                GameObject.Find("Deck p2").GetComponent<Deck>().LoadDeck();
                state = gameTracker.StartingRound;
                break;

            //Seteando condiciones necesarias para el inicio de cada ronda
            case gameTracker.StartingRound:
                //Reseteando los pass
                player1Pass = false;
                player2Pass = false;      
                //Reseteando los atacksPower
                GameObject.Find("Player1").GetComponent<PlayerManager>().power = 0;
                GameObject.Find("Player2").GetComponent<PlayerManager>().power = 0;
                //Si es la primera ronda todos roban 10 cartas
                if (numberOfRounds == 1)
                {
                    GameObject.Find("Player1").GetComponent<PlayerManager>().DrawCard(10);
                    GameObject.Find("Player2").GetComponent<PlayerManager>().DrawCard(10);
                } 
                //Si no roban 2
                else 
                {
                    //Habilidad lider, si deathwing esta en el campo el poseedor roba una mas
                    if (leadP1.transform.GetChild(0).GetComponent<Card>().cardName == "Deathwing")
                    {
                        GameObject.Find("Player1").GetComponent<PlayerManager>().DrawCard(1);
                    }
                    else if (leadP2.transform.GetChild(0).GetComponent<Card>().cardName == "Deathwing")
                    {
                       GameObject.Find("Player2").GetComponent<PlayerManager>().DrawCard(1);
                    }

                    GameObject.Find("Player1").GetComponent<PlayerManager>().CleanField();
                    GameObject.Find("Player2").GetComponent<PlayerManager>().CleanField();                  
                    GameObject.Find("Player1").GetComponent<PlayerManager>().DrawCard(2);
                    GameObject.Find("Player2").GetComponent<PlayerManager>().DrawCard(2);
                }

                //Dandole el turno a quien gano la ronda anterior, en caso de que la ronda sea la primera entonces es Player1Turn por Default
                if (player1StartTheRound == true)
                {
                    state = gameTracker.Player1Turn;
                }
                else
                {
                    state = gameTracker.Player2Turn;
                }
                
                break;

            case gameTracker.Player1Turn: //Turno del jugador 1
                //Mostrando sus cartas y escondiendo las del oponente
                GameObject.Find("Player2").GetComponent<PlayerManager>().ShowCardBack();
                GameObject.Find("Player1").GetComponent<PlayerManager>().HideCardBack();

                player1 = true;
                player2 = false;
                break;

            case gameTracker.Player2Turn: //Turno del jugador 2
                //Mostrando sus cartas y escondiendo las del oponente
                GameObject.Find("Player1").GetComponent<PlayerManager>().ShowCardBack();
                GameObject.Find("Player2").GetComponent<PlayerManager>().HideCardBack();

                player1 = false;
                player2 = true;
                break;

            case gameTracker.FinalOfRound: //Final de la ronda, aqui se hacen todos los calculos
                numberOfRounds ++; //Sumando uno a la cantidad de rondas
                
                //Si algun jugador gano 2 rondas veremos quien fue el ganador
                if (player1WinedRounds == 2 || player2WinedRounds == 2) 
                {
                    GameObject.Find("Player1").GetComponent<PlayerManager>().CleanField();
                    GameObject.Find("Player2").GetComponent<PlayerManager>().CleanField();
                    WhoWinsTheGame();
                    state = gameTracker.GameOver;
                }
                else
                {
                    state = gameTracker.StartingRound;
                }

                break;
            case gameTracker.GameOver:  //En un futuro pondra animaciones aqui,
                ResetVar();
                break;
        }

        //Mostrando informacion al usuario
        UpdateInfo();
    }
}