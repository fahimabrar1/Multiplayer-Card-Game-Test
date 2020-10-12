using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OfflineGameManager : MonoBehaviour
{

    public class PlayerScoreHolder
    {
        public int RoundScore;
        public int RoundRank;
    }
    public enum Rules
    {
        HgihCards = 1,
        Pair = 2,
        Color = 3,
        Sequence = 4,
        PureSequence = 5,
        TripleOrSet = 6
    }


    public GameObject[] Players;
    public GameObject[] playerHolders;

    public static Action ShowVisuals;
    public static Action StartRound;
    public static Action AddThePot;

    public static int defaultBet;
    public static int Turn;
    public static int StartTurn;
    public static int LastTurn;

    public static bool newRound;
    public static bool newShow;
    public static bool betMeet;
    public static bool stopCorutine;


    public static Coroutine coroutine;
    
 
    private CardLsit cardLsit;
    private UIManager ui;

    [Header("Test Purpose")]
    
    [SerializeField]
    public ArrayList tempoCards;
    
    [SerializeField]
    public List<SinglePlayer> plist;


    private void Start()
    {
        plist = new List<SinglePlayer>();

 /*      
         tempoCards = new ArrayList();

          
            CardInfo CI;
            CI = (CardInfo)Resources.Load("Data/Clubs/C - "+2);
            tempoCards.Add( (int) CI.number); 
        
        CI = (CardInfo)Resources.Load("Data/Clubs/C - "+ 6);
            tempoCards.Add( (int) CI.number);
        
        CI = (CardInfo)Resources.Load("Data/Clubs/C - "+3);
            tempoCards.Add( (int) CI.number);

        GetSequence(tempoCards);
*/
        stopCorutine = false;
        defaultBet = 5;
        foreach (GameObject player in Players)
        {   

            player.GetComponent<SinglePlayer>().enabled = true;
        }

        
        ui = FindObjectOfType<UIManager>();
        
       // Turn = Random.Range(1,Players.Length +1);
        Turn = 1;
        StartTurn = 1;
        LastTurn = 3;
        SetPlayers();

        cardLsit = (CardLsit)Resources.Load("Data/Card List");

        DistributeCards();

        if (ShowVisuals !=null)
        {
            ShowVisuals();
        }
        else
        {
            StartCoroutine(Delay());    
        }

    }


    /*
    * 
    * In this function we are setting playings pn PlayerVisual Slots. 
    * 
   */
    public void SetPlayers()
    {
        Debug.Log("Set Players");

        switch (Turn)
        {
            case 1:
                //Turn = 1
                playerHolders[0].GetComponent<PlayerHolder>().Player = Players[Turn - 1].GetComponent<SinglePlayer>();    //Turn = 0  ;   player-1
                playerHolders[1].GetComponent<PlayerHolder>().Player = Players[Turn].GetComponent<SinglePlayer>(); ;        //Turn = 1  ;   player-2
                playerHolders[2].GetComponent<PlayerHolder>().Player = Players[Turn + 1].GetComponent<SinglePlayer>(); ;    //Turn = 2  ;   player-3

                break;
            case 2:
                //Turn = 1
                playerHolders[0].GetComponent<PlayerHolder>().Player = Players[Turn - 1].GetComponent<SinglePlayer>(); ;    //Turn = 1  ;   player-2
                playerHolders[1].GetComponent<PlayerHolder>().Player = Players[Turn].GetComponent<SinglePlayer>(); ;        //Turn = 2  ;   player-3
                playerHolders[2].GetComponent<PlayerHolder>().Player = Players[0].GetComponent<SinglePlayer>(); ;           //Turn = 0  ;   player-1


                break;
            case 3:
                //Turn = 1
                playerHolders[0].GetComponent<PlayerHolder>().Player = Players[Turn - 1].GetComponent<SinglePlayer>(); ;    //Turn = 2  ;   player-3
                playerHolders[1].GetComponent<PlayerHolder>().Player = Players[0].GetComponent<SinglePlayer>(); ;           //Turn = 0  ;   player-1
                playerHolders[2].GetComponent<PlayerHolder>().Player = Players[Turn - 2].GetComponent<SinglePlayer>(); ;    //Turn = 1  ;   player-2

                break;

        }
    }


    /*
     * 
     * In this function just making a Delay so Event System wont be Null
     * 
    */
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        ShowVisuals();
        newRound = true;
        StartCoroutine(WaitForPlayer());
    }


    /*
     * 
     * In this function we are waiting 10 seconds and switching to next Player.
     * It also resets Table And cuts fixed wage newRound is true.
     * 
    */
    public IEnumerator WaitForPlayer()
    {
        Debug.LogWarning("New Show "+newShow);

        if (newShow)
        {
            GetPotWinner();
            ShowVisuals();
            yield return new WaitForSeconds(2);
            AddThePot();

            Debug.LogWarning("New Show " + newShow);

            Debug.Log("Show Cards");

            yield return new WaitForSeconds(3);
            
            
            ui.ShowNextPlayer(); 
            SetPlayers();
            StopCoroutinePlease();
            ui.StopCoroutinePlease();
            newRound = true;
            newShow = false;
            StartCoroutine(WaitForPlayer());

        }
        else
        {
            if (newRound)
            {
                Debug.Log("New Round");
                ShowVisuals();
                NewRound();
            }
            int i = 0;
            while (i < 10)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("Running Corroutine sec   : " + i);
                i++;
            }
            ui.ShowNextPlayer();
            SetPlayers();
            ShowVisuals();
            StopAllCoroutines();
            ui.StopCoroutinePlease();
            StartCoroutine(WaitForPlayer());
        }
       
    }


    /*
     * 
     * In this function it stops all the corroutines in this behaviour
     * 
    */
    public void StopCoroutinePlease()
    {
        try
        {
            StopAllCoroutines();
            Debug.Log("Stopped All Corroutines");

        }
        catch (Exception)
        {
            Debug.Log("StopAllCoroutines() in WaitForPlayers()  ");
        }
    }


    /*
     * 
     * In this function A fixed amount is deducted from each player.
     * 
    */
    private void NewRound()
    {
        DistributeCards();
        if (StartRound != null)
        {
            newRound = true;
            StartRound();
            int i;

            for (i = 0; i < Players.Length; i++){   }
            
            ui.UpdateTable(i*5 , newRound);
            newRound = false;
        }
        newShow = false;
        ShowVisuals();
    }


    /*
     * 
     * In this function Cards are Distrubuted to Players in new Round.
     * 
    */
    private void DistributeCards()
    {
        ResetCards();
        foreach (GameObject players in Players)
        {
           
            for (int i = 0; i < 3; i++)
            {
                int val = Random.Range(0, cardLsit.CardList.Count);
//                Debug.Log("Rand" + val);

                while (cardLsit.CardList[val].Distributed == true)
                {
                    val = Random.Range(0, cardLsit.CardList.Count);
//                    Debug.Log("New Rand" + val);

                }
                if (cardLsit.CardList[val].Distributed == false)
                {
//                   Debug.Log("Matched" + val);
                    cardLsit.CardList[val].Distributed = true;
                    players.GetComponent<SinglePlayer>().cards.Add(cardLsit.CardList[val]);
 
                }
            }
        }
    }


    /*
     * 
     * In this function Cards Reset Fore new Round.
     * 
    */
    private void ResetCards()
    {
        plist.Clear();
        foreach (GameObject players in Players)
        {

            players.GetComponent<SinglePlayer>().cards.Clear();
            
        }

        foreach (CardInfo info in cardLsit.CardList)
        {
            info.Distributed = false;
        }
    }


    /*
     * 
     * In this function Calculates who is the winner.
     * 
    */


    private void GetPotWinner()
    {
        foreach (GameObject obj in Players)
        {
            SinglePlayer player = obj.GetComponent<SinglePlayer>();

            //         int number = (int)  player.cards[0].number;
            //         Debug.Log("Player Card Number   : " +number);
            PlayerScoreHolder playerScore = ProcessCards(player.cards);
            player.RoundScore = playerScore.RoundScore;
            player.PotRank = playerScore.RoundRank;

            plist.Add(player);
        }

        SinglePlayer Winner = plist[0];
        for (int i = 0; i < 3; i++)
        {
            SinglePlayer Pcurrent = plist[i];
            for (int j = 0; j < 3; j++)
            {
                if (Pcurrent.PotRank < plist[j].PotRank)
                {
                    Winner = plist[j];
                }
                else if (Pcurrent.PotRank == plist[j].PotRank && Pcurrent.RoundScore < plist[j].RoundScore)
                {
                    Winner = plist[j];
                }
            }

            Winner.WonPOT = true;
        }

    }


    private PlayerScoreHolder ProcessCards(List<CardInfo> cards)
    {
        int Cnum1 = (int)cards[0].number;
        int Cnum2 = (int)cards[1].number;
        int Cnum3 = (int)cards[2].number;
        
        int Crank1 = (int)cards[0].cardType;
        int Crank2 = (int)cards[1].cardType;
        int Crank3 = (int)cards[2].cardType;

        ArrayList cardArray = new ArrayList();
        foreach (CardInfo card in cards)
        {
            cardArray.Add((int)card.number);
        }

        PlayerScoreHolder score = new PlayerScoreHolder();
        if (Cnum1 == Cnum2 && Cnum1 == Cnum3)
        {
            //Triple Or Set

            score.RoundScore = (Cnum1 + Cnum2 + Cnum3);
            score.RoundRank = (int)Rules.TripleOrSet;
            return score;

        } else if (GetSequence(cardArray))
        {
             if (Crank1 == Crank2 && Crank1 == Crank3)
            {
                //Pure Sequence

                score.RoundScore = (Cnum1 * Crank1 + Cnum2 * Crank2 + Cnum3 * Crank3);
                score.RoundRank = (int)Rules.PureSequence;
                return score;

            }
            else
            {
                // Sequence

                score.RoundScore = (Cnum1 * Crank1 + Cnum2 * Crank2 + Cnum3 * Crank3);
                score.RoundRank = (int)Rules.Sequence;
                return score;

            }
        }
        else if (Crank1 == Crank2 && Crank1 == Crank3)
        {
            //Color

            score.RoundScore = (Cnum1 * Crank1 + Cnum2 * Crank2 + Cnum3 * Crank3);
            score.RoundRank = (int)Rules.Color;
            return score;


        }
        else if ((Cnum1 == Cnum2 && Cnum1 != Cnum3) ||
                  (Cnum1 == Cnum3 && Cnum1 != Cnum2) ||
                  (Cnum2 == Cnum3 && Cnum2 != Cnum1))
        {
            //Pair (2 card sof the same Number)
            score.RoundScore = (Cnum1 * Crank1 + Cnum2 * Crank2 + Cnum3 * Crank3);
            score.RoundRank = (int)Rules.Pair;
            return score;

        }
        else
        {
            //High Cards
            score.RoundScore = (Cnum1*Crank1 + Cnum2*Crank2 + Cnum3*Crank3);
            score.RoundRank = (int)Rules.HgihCards;
            return score;

        }

    }


    private bool GetSequence(ArrayList array)
    {
        int min = 2;
        int max = 14;

        array.Sort();

        
        int val1 = (int) array[0];
        int val2 = (int) array[1];
        int val3 = (int) array[2];
        
        
        foreach (int i in array)
        {
            Debug.LogWarning("Sorted card Number   : " +i);
        }
      
        if (val1 >= 2 && val3 <=14 )
        {
            if (val2 == (val1+1) && val3 == (val2+1))
            {
                Debug.Log("A Swquence");
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;

    }

    
}
