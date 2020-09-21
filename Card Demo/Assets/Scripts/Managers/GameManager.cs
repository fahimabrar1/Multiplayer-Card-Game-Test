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
    public CardLsit CardList;

    public enum Datatypes
    {
        Coins
    }

    private void Awake()
    {
        if (instance == null)
        {
           
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
