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

    public Text ConnectionLogText;
    public GameObject ConnectionLogPanel;
   
    public GameObject OtherPlayer;

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

    
    
    public void OtherPlayerActive()
    {
        OtherPlayer.SetActive(true);
    }


    public void OtherPlayerInActive()
    {
        OtherPlayer.SetActive(false);
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
