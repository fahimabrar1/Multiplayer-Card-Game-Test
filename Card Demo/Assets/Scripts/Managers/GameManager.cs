using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static string PlafabID;
    public static GetAccountInfoResult username;
    public static int joinedTime;
    public static bool BotAvailable;
    public static bool ResumeMatch;
    public static bool NewRound;
    public static bool ActiveRound;
    public static List<int> LivePlayerID;
    public CardLsit CardList;

    public enum Datatypes
    {
        Coins
    }

    private void Awake()
    {
        if (instance == null)
        {
            LivePlayerID = new List<int>();
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }


    private void Start()
    {
        
       
        foreach (CardInfo item in CardList.CardList)
        {
            item.Distributed = false;
        }
    }
}
