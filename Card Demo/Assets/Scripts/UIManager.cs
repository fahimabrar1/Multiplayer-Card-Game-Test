using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Steps;

public class UIManager : MonoBehaviour
{
    [Header("User Info")]
    [Tooltip("User Name will be set when screen laods")]
    public Text UserName;

    [Tooltip("User Available Coins will be set when screen laods")]
    public Text CoinsAvailable;

    private int CoinsAvailableInteger;

    private void Start()
    {

        var coins = new GetUserDataRequest();
        var profile = new GetAccountInfoRequest();
        profile.PlayFabId = GameManager.PlafabID;
        coins.PlayFabId = GameManager.PlafabID;
        Debug.Log(GameManager.username);
        UserName.text = GameManager.username;
         PlayFabClientAPI.GetUserData(coins, GetUserDataSuccess, GetuserDataFailure);
    }

  
    private void GetUserDataSuccess(GetUserDataResult result)
    {
        
        if (result.Data.ContainsKey(GameManager.Datatypes.Coins.ToString()))
        {

            CoinsAvailable.text = result.Data[GameManager.Datatypes.Coins.ToString()].Value;
            CoinsAvailableInteger = int.Parse(result.Data[GameManager.Datatypes.Coins.ToString()].Value);
        }
    }


    private void GetuserDataFailure(PlayFabError result)
    {
        Debug.LogWarning("User Data Set Failed");

    }


    public void OnIncrement()
    {
        
        int val = 5;
        CoinsAvailableInteger += val;
        CoinsAvailable.text = CoinsAvailableInteger.ToString();
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {

            Data = new Dictionary<string, string>()
            {
                { GameManager.Datatypes.Coins.ToString() , CoinsAvailable.text}
            }
        }, SetUserDataSuccess, SetuserDataFailure);
    }

   

    public void OnDecrement()
    {
        int val = 5;
        CoinsAvailableInteger -= val;
        CoinsAvailable.text = CoinsAvailableInteger.ToString();

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {

            Data = new Dictionary<string, string>()
            {
                { GameManager.Datatypes.Coins.ToString() , CoinsAvailable.text}
            }
        }, SetUserDataSuccess, SetuserDataFailure);

    }


    private void SetuserDataFailure(PlayFabError obj)
    {
        Debug.Log("User Data Set Failure");
    }


    private void SetUserDataSuccess(UpdateUserDataResult obj)
    {
        Debug.Log("User Data Set Sucess");
    }


}
