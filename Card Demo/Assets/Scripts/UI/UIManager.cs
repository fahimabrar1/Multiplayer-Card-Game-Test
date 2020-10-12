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
    
    public GameObject OfflineMode;


    public Text TableText;

    public Text ConnectionLogText;

    public Text BetText;

    public GameObject ConnectionLogPanel;
   
    public GameObject[] OtherPlayer;

    private Animator animator;

    public PhotonManager pm;

    public GameObject SeeCards;
    public GameObject ShowButton;




    private int CoinsAvailableInteger;

    private Player player;
    
    private OfflineGameManager offlineGameManager;
    
    
    private void Start()
    {
        animator = ConnectionLogPanel.GetComponent<Animator>();
        UI.SetActive(true);
        InGame.SetActive(false);
        OfflineMode.SetActive(true);
        /*
         * While Using Playfab And photon
         * 
         * 
        var coins = new GetUserDataRequest();
        var profile = new GetAccountInfoRequest();
        profile.PlayFabId = GameManager.PlafabID;
        coins.PlayFabId = GameManager.PlafabID;
        Debug.Log(GameManager.username);
        UserName.text = GameManager.username.AccountInfo.Username;
        PlayFabClientAPI.GetUserData(coins, GetUserDataSuccess, GetuserDataFailure);
        */




    }

    #region PhotonBased


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
        int turn = PhotonNetwork.room.GetTurn();


        if (PhotonNetwork.room.playerCount <= turn)
        {
            GameManager.NewRound = true;
            PhotonNetwork.room.SetTurn(1);
            Debug.Log("Get Turn Of Player   : " + PhotonNetwork.room.GetTurn());

            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().myturn = false;

        }
        else if (turn == PhotonNetwork.player.ID)
        {
            PhotonNetwork.room.SetTurn(PhotonNetwork.room.GetTurn() + 1);
            Debug.Log("Get Turn Of Player   : " + PhotonNetwork.room.GetTurn());

            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().myturn = false;
        }
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


    #endregion PhotonBased


    #region PopUps

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



    #endregion PopUps



    public void OnClickFold()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().OnClickFold();    
    }


    #region Offline

    /*
     * 
     * In this function Offline Game Starts On Click Offline
     * 
    */
    public void OnClickOfflineMode()
    {
        UI.SetActive(false);
        InGame.SetActive(true);
        OfflineMode.SetActive(true);
        ShowButton.SetActive(false);
        offlineGameManager = FindObjectOfType<OfflineGameManager>().GetComponent<OfflineGameManager>();
        Text tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();
        tablepot.text = "0";
    }
    
    
    /*
     * 
     * In this function Offline player see his cards and bets doubled for him.
     * 
    */
    public void OnClickOfflineSeeCards()
    {
        PlayerHolder ph = offlineGameManager.playerHolders[0].GetComponent<PlayerHolder>();
        SinglePlayer sp = ph.Player;
        sp.seenCards = true;
        sp.ShowCards();
    }
    
    
    
    /*
     * 
     * In this function Hides cards for new round.
    */
    public void OnClickOfflineHideCards()
    {
        foreach (GameObject player in offlineGameManager.playerHolders)
        {
            PlayerHolder ph = player.GetComponent<PlayerHolder>();
            SinglePlayer sp = ph.Player;
            sp.seenCards = false;
            sp.ShowCards();
        } 
    }


    /*
     * 
     * In this function we are just switching new players and showing their cards.
     * 
    */
    public void OnClickOfflineFold()
    {
        offlineGameManager.StopCoroutinePlease();
        OfflineGameManager.stopCorutine = true;   
        ShowNextPlayer();
        offlineGameManager.SetPlayers();
        OfflineGameManager.ShowVisuals();
        StartCoroutine(offlineGameManager.WaitForPlayer());
    }


    /*
     * 
     * In this function we are deducting 5 from player, updating the Table and swtiching to next player.
     * 
    */
    public void OnClickOfflineBet()
    {
 //       OfflineGameManager.stopCorutine = true;

        //whos turn is it ?
        SinglePlayer[] Players = FindObjectsOfType<SinglePlayer>();

        int hissTurn =  OfflineGameManager.Turn;
        foreach (SinglePlayer player in Players)
        {
            if (hissTurn == player.PlayerID)
            {
                player.IncrementedTimes++;
                Debug.Log("Decrementing : " + player.PlayerID);
                UpdateTable(player.DecrementWageReturn(), false);

                Debug.Log("Done Decrementing");
                break;
            }
        }

        FindObjectOfType<OfflineGameManager>().GetComponent<OfflineGameManager>().StopCoroutinePlease();
        StopAllCoroutines();
        ShowNextPlayer();
        offlineGameManager.SetPlayers();
        OfflineGameManager.ShowVisuals();
        StartCoroutine(offlineGameManager.WaitForPlayer());

    }


    /*
     * 
     * In this function we are swtiching to next player.
     * 
    */
    public void ShowNextPlayer()
    {
        SinglePlayer[] singlePlayers = FindObjectsOfType<SinglePlayer>();
        foreach (SinglePlayer player in singlePlayers)
        {

            if (player.PlayerID == OfflineGameManager.Turn)
            {
                if (OfflineGameManager.Turn < (singlePlayers.Length))
                {

                    OfflineGameManager.newRound = false;
                    OfflineGameManager.newShow = false;

                    OfflineGameManager.Turn++;
                    Debug.Log("Assigning Turn : " + OfflineGameManager.Turn);
                    break;

                }
                else
                {
                    OfflineGameManager.newRound = true;
                    OfflineGameManager.newShow = true;
                    OfflineGameManager.Turn = 1;
                    Debug.Log("Assigning Turn : " + OfflineGameManager.Turn);

                    foreach (SinglePlayer player1 in singlePlayers)
                    {
                        player1.seenCards = false;
                    }
                    break;
                }

            }

        }
    }



    /*
     * 
     * In this function we are updating the Table.
     * 
    */
    public void UpdateTable(int val , bool Reset )
    {
        Text tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();
        if (Reset)
        {
            tablepot.text = val.ToString();
        }
        else
        {
            int value = int.Parse(tablepot.text);
            value += val;
            tablepot.text = value.ToString();
        }

    }



    /*
     * 
     * In this function if the bets are meet the last player can show..
     * 
    */
    public void OnClickOfflineShow()
    {
        offlineGameManager.StopCoroutinePlease();
        OfflineGameManager.stopCorutine = true;
        OfflineGameManager.newShow = true;
        StopAllCoroutines();
        StartCoroutine(offlineGameManager.WaitForPlayer());

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




    #endregion Offline





}
