using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class SinglePlayer : MonoBehaviour 
{
    
    public int PlayerID;
    public int Money = 1000;
    public int RoundScore = 0;
    public int PotRank = 0;
    public List<CardInfo> cards;

    public Sprite CoverCard;
    public int IncrementedTimes;
    public bool seenCards;
    public bool lastPlayer;
    public bool WonPOT;

    private OfflineGameManager offlineGameManager;
    private UIManager ui;



    private void Start()
    {
        seenCards = false;

        IncrementedTimes = 1;
        Debug.Log("Assigining Action for PlayerID : " + PlayerID);
        offlineGameManager = FindObjectOfType<OfflineGameManager>();
        ui = FindObjectOfType<UIManager>();

        OfflineGameManager.ShowVisuals += ShowCards;
        OfflineGameManager.StartRound += DecrementWage;
        OfflineGameManager.AddThePot += AddTableWageToWallet;
    }


    /*
    * 
    * In this function Shows Player ID,   Money,  Cards , See and Show BUtton.
    * 
   */
    public void ShowCards()
    {
        Debug.Log("Showed Cards for PlayerID :  " + PlayerID );


        if (PlayerID == OfflineGameManager.Turn)
        {
            if (!seenCards)
            {
                ui.SeeCards.SetActive(true);
            }
            else
            {
                ui.SeeCards.SetActive(false);
            }

            if (OfflineGameManager.LastTurn == PlayerID)
            {
                ui.ShowButton.SetActive(true);
            }
            else
            {
                ui.ShowButton.SetActive(false);
            }


            GameObject PlayerVisual = GetPlayer();

            DisplayPlayerID(PlayerVisual);
            DisplayPlayerMoney(PlayerVisual);
            DisplayPlayeCards(PlayerVisual);
        }
        else
        {
            GameObject PlayerVisual = GetPlayer();
           
            DisplayPlayerID(PlayerVisual);
            DisplayPlayerMoney(PlayerVisual);
            DisplayPlayeCards(PlayerVisual);
        }


    }


    /*
    * 
    * In this function Decrementing wage of 5 or 10 from player.
    * 
   */
    public void DecrementWage() 
    {
       
        Money -= (seenCards == false) ? 5 : 10;
        DisplayPlayerMoney(GetPlayer());
    }


    /*
    * 
    * In this function Decrementing wage of 5 or 10 from player and return.
    * 
   */
    public int DecrementWageReturn() 
    {
        int val = (seenCards == false) ? 5 : 10; ;
        Money -= val;
        DisplayPlayerMoney(GetPlayer());
        return val;
    }


    /*
    * 
    * In this function We are getting the PlayerVisual GameObject where this player holds.
    * 
   */
    private GameObject GetPlayer()
    {
        GameObject PlayerVisual = null;
        GameObject[] phobj = offlineGameManager.playerHolders;
        foreach (GameObject obj in phobj)
        {
            PlayerHolder ph = obj.GetComponent<PlayerHolder>();
            if (ph.Player == this)
            {
                PlayerVisual = obj;
                break;
            }
          
        }
     
        return PlayerVisual;
    }


    /*
    * 
    * In this function we are Displaying Player ID.
    * 
   */
    private void DisplayPlayerID(GameObject PlayerVisual)
    {

        Text Playername = PlayerVisual.transform.GetChild(0).GetComponent<Text>();
        Playername.text = "Player " + PlayerID.ToString();

    }


    /*
    * 
    * In this function just making a Delay so Event System wont be Null
    * 
   */
    private void DisplayPlayerMoney(GameObject PlayerVisual)
    {
        Text PlayerMoney = PlayerVisual.transform.GetChild(2).GetComponent<Text>();
        PlayerMoney.text = "Money : " + Money.ToString();
        
    }


    /*
    * 
    * In this function just making a Delay so Event System wont be Null
    * 
   */
    private void AddTableWageToWallet()
    {
        if (WonPOT)
        {
            Text tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();
            int val = int.Parse(tablepot.text);
            Money += val;


            //For New Round
            WonPOT = false;
        }
      
    }


    /*
    * 
    * In this function we are showing Cards the player holds.
    * 
   */
    private void DisplayPlayeCards(GameObject PlayerVisual)
    {
        GameObject DisplayCards = PlayerVisual.transform.GetChild(1).gameObject;
        Debug.Log("Show All Cards");

        for (int i = 0; i < 3; i++)
        {
            Image cardsDisplay = DisplayCards.transform.GetChild(i).GetComponent<Image>();
            if (OfflineGameManager.newShow)
            {
                Debug.Log("Show Cards   "+(i+1));
                cardsDisplay.sprite = cards[i].Sprite;

            }
            else if (seenCards && PlayerID == OfflineGameManager.Turn)
            {
                cardsDisplay.sprite = cards[i].Sprite;
                
            }
            else
            {
                cardsDisplay.sprite = CoverCard;

            }

            if (seenCards)
            {
                ui.BetText.text = (OfflineGameManager.defaultBet * 2).ToString();
            }
            else
            {
                ui.BetText.text = (OfflineGameManager.defaultBet).ToString();
            }
        }
    }


  


    /*
    * 
    * In this function removing functions from Action.
    * 
   */
    private void OnDisable()
    {
        OfflineGameManager.ShowVisuals -= ShowCards;
        OfflineGameManager.StartRound -= DecrementWage;
        OfflineGameManager.AddThePot -= AddTableWageToWallet;

    }





}
