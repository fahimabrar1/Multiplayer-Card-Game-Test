using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("User Info")]
    [Tooltip("User Name will be set when screen laods")]
    public Text UserName;

    [Tooltip("User Available Coins will be set when screen laods")]
    public Text CoinsAvailable;

    public GameObject UI;

    public GameObject InGame;

    public Text TableText;

    public Text ConnectionLogText;
    public GameObject ConnectionLogPanel;
   
    public GameObject[] OtherPlayer;

    private Animator animator;

    public PhotonManager pm;


    private int CoinsAvailableInteger;

    private void Start()
    {
        animator = ConnectionLogPanel.GetComponent<Animator>();
        
        UI.SetActive(true);
        InGame.SetActive(false);
        var coins = new GetUserDataRequest();
        var profile = new GetAccountInfoRequest();
        profile.PlayFabId = GameManager.PlafabID;
        coins.PlayFabId = GameManager.PlafabID;
        Debug.Log(GameManager.username);
        UserName.text = GameManager.username.AccountInfo.Username;
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

    internal void OnLeaveGame()
    {
        foreach (GameObject item in OtherPlayer)
        {
            item.name= "P";
            item.SetActive(false);
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


    public void NextTurn()
    {
        if (PhotonNetwork.room.playerCount == PhotonNetwork.room.GetTurn())
        {
            PhotonNetwork.room.SetTurn( 0);
        }
        PhotonNetwork.room.SetTurn(PhotonNetwork.room.GetTurn() +1);
        Debug.Log("Get Turn Of Player   : " + PhotonNetwork.room.GetTurn());
    }


    public void TurnInGameUIOn()
    {
        InGame.SetActive(true);
    }
    
    public void TurnInGameUIOff()
    {
        InGame.SetActive(false);
    }
    
    public void TurnUIOn()
    {
        UI.SetActive(true);
    }
    
    public void TurnUIOff()
    {
        UI.SetActive(false);
    }

    
    
    public void OtherPlayerActive(string userId)
    {
        foreach (GameObject item in OtherPlayer)
        {
            if (item.name.Equals("P"))
            {
                item.name = userId;
                item.SetActive(true);
                break;
            }
        }
    }


    public void OtherPlayerInActive(string userId)
    {
        foreach (GameObject item in OtherPlayer)
        {
            if (userId.Equals(item.name))
            {
                item.name = "P";
                item.SetActive(false);
                break;
            }
        }
    }



    public IEnumerator OpenLogPanel()
    {
        animator.SetBool("connect", true);
        animator.SetBool("stay", false);
        Debug.Log("PhotonManager.connected  :" + PhotonManager.connected);

        if (PhotonManager.connected)
        {
            PhotonManager.connected = false;
            Debug.Log("Closing Panel");
            while (PhotonNetwork.connected)
            {
                yield return null;
            }
            StartCoroutine(CloseLogPanel());
        }
        else
        {
            
            Debug.Log("Connecting to Master");
            yield return new WaitForSeconds(1);

            pm.ConnectToMaster();
        } 
    }


    public IEnumerator CloseLogPanel()
    {
        Debug.Log(PhotonNetwork.connectionState);

        animator.SetBool("connect", false);
        animator.SetBool("stay", false);
        yield return new WaitForSeconds(2f);
        animator.SetBool("stay", true);

    }

}
