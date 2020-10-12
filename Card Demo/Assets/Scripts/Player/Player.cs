using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : Photon.PunBehaviour, IPunTurnManagerCallbacks
{

    private GameObject PlaySlots;
    private Text tablepot;
    public bool myturn;
    UIManager ui;
    GameObject slots;
    CardLsit cl;
    TableData tableData;
    public int PlayerTurn;
    public int AllTurn;
    




    private void Awake()
    {
        tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();
        ui = FindObjectOfType<UIManager>();
       
    }



    // Start is called before the first frame update
    void Start()
    {

        AllTurn = PhotonNetwork.room.GetTurn();

         //        slots = Instantiate(PlaySlots, GameObject.FindGameObjectWithTag("Canvas").gameObject.transform);
        myturn = false;
        
        if (photonView.isMine)
        {
            gameObject.tag = "Player";
           
            Debug.Log(PhotonNetwork.player.ID);
            Debug.Log(PhotonNetwork.player.UserId);

            Debug.Log(PhotonNetwork.player.customProperties[PhotonNetwork.player.UserId]);
            // PlaterTurn = (int) PhotonNetwork.player.customProperties[PhotonNetwork.player.UserId];
            PlayerTurn = photonView.viewID / 1000;


            Debug.Log("Player Turn     : " + PlayerTurn);


            Debug.Log("Get Turn     : " + PhotonNetwork.room.GetTurn());


            DistributeCards();
            //   SetTableData();


            GameManager.LivePlayerID.Add(PlayerTurn);
            Player[] CP = FindObjectsOfType<Player>();
            foreach (int item in GameManager.LivePlayerID)
            {
                Debug.LogWarning(PlayerTurn);
            }
            
        }
        else
        {

            foreach (int item in GameManager.LivePlayerID)
            {
                Debug.LogWarning(PlayerTurn);
            }
            PlayerTurn = photonView.viewID/1000;

            //    SetTableData();

            GameManager.LivePlayerID.Add(PlayerTurn);
            GameManager.LivePlayerID.Sort();

        }

    }


    public void SetTableData()
    {
        tableData = (TableData)Resources.Load("Data/PlayerTable/Player Table");
        PlayerTurnData playerTurnData = new PlayerTurnData() ;
        playerTurnData.PlayerID = PlayerTurn;
        playerTurnData.PlayerActiveTurn = 0;
        playerTurnData.Nextplayer = 0;

        tableData._PlayerTurnData.Add(playerTurnData);


        /*foreach (PlayerTurnData data in tableData._PlayerTurnData)
        {
            if (data.PlayerID == 0)
            {
                data.PlayerID = PlayerTurn;
                data.Nextplayer = 0;
                break;
            }
        }*/
    }
    private void DistributeCards()
    {
        PlaySlots = GameObject.FindGameObjectWithTag("PlayerSlots").gameObject;
        cl = (CardLsit)Resources.Load("Data/Card List");
        for (int i = 0; i < 5; i++)
        {
            int val = Random.Range(0, cl.CardList.Count);
            Debug.Log("Rand" + val);

            while (cl.CardList[val].Distributed == true)
            {
                val = Random.Range(0, cl.CardList.Count);
                Debug.Log("New Rand" + val);

            }
            if (cl.CardList[val].Distributed == false)
            {
                Debug.Log("Matched" + val);
                cl.CardList[val].Distributed = true;
                Image sprite = PlaySlots.transform.GetChild(i).GetComponent<Image>();
                CardDataHolder cd = PlaySlots.transform.GetChild(i).GetComponent<CardDataHolder>();
                cd.SetVariables(true, (CardDataHolder.Number)cl.CardList[val].number, (CardDataHolder.CardType)cl.CardList[val].cardType, cl.CardList[val].Sprite);
                cd.Sprite = sprite.sprite = cl.CardList[val].Sprite;
            }
        }
    }



    private void Update()
    {
        if (PhotonNetwork.room.GetTurn() == PhotonNetwork.player.ID)
        {
            myturn = true;
        }
        if (photonView.isMine)
        {

            //If enough players are in the room to Resume the game.
            if (GameManager.ResumeMatch)
            {
                Debug.Log("ResumeMatch");
                //If a Round starts
                if (GameManager.NewRound)
                {
                    tablepot.text = "0";
                    GameManager.NewRound = false;

                    Debug.Log("NewRound");
                    StartCoroutine(StartRound());

                }


              /*  if (GameManager.ActiveRound && myturn)
                {
                    Debug.Log("ActiveRound");
                    Debug.Log("myturn");
                    If the round is active and local player's turn.
                    StartCoroutine(PlayerMove());



                }*/


            }
        }

    }


    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        cl = (CardLsit)Resources.Load("Data/Card List");
        tableData = (TableData)Resources.Load("Data/PlayerTable/Player Table");

        if (stream.isWriting)
        {
            foreach (CardInfo item in cl.CardList)
            {
                stream.SendNext(item.Distributed);
            }
            
          /*  foreach (PlayerTurnData data in tableData._PlayerTurnData)
            {
                stream.SendNext(data.PlayerID);
                stream.SendNext(data.PlayerActiveTurn);
                stream.SendNext(data.Nextplayer);
            }*/

            Debug.Log("stream.isWriting     : " + tablepot.text);
            if (!tablepot.text.Equals("0"))
            {
                stream.SendNext(tablepot.text);
            }
        
        }
        else if(stream.isReading)
        {
            foreach (CardInfo item in cl.CardList)
            {
                item.Distributed = (bool)stream.ReceiveNext();

            }
            /* foreach (PlayerTurnData data in tableData._PlayerTurnData)
             {
                 data.PlayerID = (int )stream.ReceiveNext(); ;
                 data.PlayerActiveTurn = (int)stream.ReceiveNext();
                 data.Nextplayer = (int)stream.ReceiveNext(); 
             }*/

            try
            {
                tablepot.text = (string)stream.ReceiveNext();
                Debug.Log("stream.isReading     : " + tablepot.text);
            }
            catch (Exception)
            {
                
            }
           

        }
    }



    IEnumerator PlayerMove()
    {
        int waitSeconds = 30;

        yield return new WaitForSeconds(5);

        myturn = false;
        OnTurnTimeEnds(PlayerTurn);
    }


    public void OnClickFold()
    {
        OnPlayerFinished(PhotonNetwork.player, PlayerTurn, null);


    }


    IEnumerator StartRound()
    {
        ui.OnDecrement();
        yield return new WaitForSeconds(0.2f);

        UpdateTable();

        GameManager.ActiveRound = true;

    }

    public void UpdateTable()
    {
        string wage = tablepot.text;
        int calculate = int.Parse(wage) + 5;
        tablepot.text = calculate.ToString();

        OnTurnBegins(PhotonNetwork.room.GetTurn());
    }

    public void OnTurnBegins(int turn)
    {
        if (turn == PlayerTurn)
        {
            myturn = true;
            Debug.Log("Player Turn :" + turn);

            StartCoroutine(PlayerMove());
        }
       
    }

    public void OnTurnCompleted(int turn)
    {

        if (AllTurn == PhotonNetwork.countOfPlayers)
        {
            Debug.Log("AllTurn == PhotonNetwork.countOfPlayers      :" + AllTurn    );

            AllTurn = 1;
            PhotonNetwork.room.SetTurn(AllTurn);
        }
        else
        {
            Debug.Log("AllTurn      :" + AllTurn);
            AllTurn++;
            PhotonNetwork.room.SetTurn(AllTurn);

        }
      

    }

    public void OnPlayerMove(PhotonPlayer player, int turn, object move)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
    {
        OnTurnTimeEnds(turn);
    }

    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("Player Turn :" + turn);
        if (turn ==PlayerTurn)
        {
            PhotonNetwork.room.SetTurn(turn +1);
        }
    }
}

