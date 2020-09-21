using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Photon.PunBehaviour
{
    public List<BotCardDataHolder> cd;
    CardLsit cl;

    // Start is called before the first frame update
    void Start()
    {
        //        slots = Instantiate(PlaySlots, GameObject.FindGameObjectWithTag("Canvas").gameObject.transform);

        cd = new List<BotCardDataHolder>();
        
        if (photonView.isMine)
        {
            cl = (CardLsit)Resources.Load("Data/Card List");
            Debug.Log("CD size      :" + cl.CardList.Count);

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

                    
                    cd.Add(new BotCardDataHolder(cl.CardList[val].Distributed, (BotCardDataHolder.Number)cl.CardList[val].number, (BotCardDataHolder.CardType)cl.CardList[val].cardType, cl.CardList[val].Sprite));
                }

            }
            Debug.Log("Max Player Allowed     :" + PhotonNetwork.room.maxPlayers);
            Debug.Log("Player Count     :" + PhotonNetwork.room.playerCount);

        }
        else
        {

        }
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
        }
        else if (stream.isReading)
        {
            foreach (CardInfo item in cl.CardList)
            {
                item.Distributed = (bool)stream.ReceiveNext();

            }

        }
    }
}
