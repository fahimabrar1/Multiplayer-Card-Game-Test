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

    GameObject slots;
    CardLsit cl;
    public int PlaterTurn;
    private bool myturn;



    private void Awake()
    {
        tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();

    }



    // Start is called before the first frame update
    void Start()
    {
        //        slots = Instantiate(PlaySlots, GameObject.FindGameObjectWithTag("Canvas").gameObject.transform);
        myturn = false;

        if (photonView.isMine)
        {

           
            Debug.Log(PhotonNetwork.player.ID);
            Debug.Log(PhotonNetwork.player.UserId);

            Debug.Log(PhotonNetwork.player.customProperties[PhotonNetwork.player.UserId]);
            PlaterTurn = (int) PhotonNetwork.player.customProperties[PhotonNetwork.player.UserId];

            Debug.Log("Player Turn     : " + PlaterTurn);

            Debug.Log("Get Turn     : " + PhotonNetwork.player.GetFinishedTurn());
            Debug.Log("Get Turn     : " + PhotonNetwork.room.GetTurn());


             DistributeCards(); 

        }
        else
        {
           
        
        }
           
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
        if (photonView.isMine && myturn)
        {
            if (GameManager.ResumeMatch)
            {
                if (GameManager.NewRound)
                {
                    StartRound();
                    
                    GameManager.NewRound = false;
                }


                if (GameManager.ActiveRound && PhotonNetwork.room.GetTurn() == PhotonNetwork.player.ID)
                {

                    StartCoroutine(PlayerMove());



                }


            }
        }

    }

    private string PlayerMove()
    {
        throw new NotImplementedException();
    }

    private string StartRound()
    {
        throw new NotImplementedException();
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        cl = (CardLsit)Resources.Load("Data/Card List");

        if (stream.isWriting)
        {
            foreach (CardInfo item in cl.CardList)
            {
                stream.SendNext(item.Distributed);
            }

            stream.SendNext(tablepot.text);
            stream.SendNext(GameManager.NewRound);

        }
        else if(stream.isReading)
        {
            foreach (CardInfo item in cl.CardList)
            {
                item.Distributed = (bool)stream.ReceiveNext();

            }

            tablepot.text = (string) stream.ReceiveNext();
            GameManager.NewRound = (bool) stream.ReceiveNext();
        }
    }

    public void OnTurnBegins(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void OnTurnCompleted(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerMove(PhotonPlayer player, int turn, object move)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
    {
        throw new System.NotImplementedException();
    }

    public void OnTurnTimeEnds(int turn)
    {
        throw new System.NotImplementedException();
    }
}

