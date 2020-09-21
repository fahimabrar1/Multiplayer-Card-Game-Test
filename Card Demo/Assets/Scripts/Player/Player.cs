using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.PunBehaviour
{

    private GameObject PlaySlots;

    GameObject slots;
    CardLsit cl;

    // Start is called before the first frame update
    void Start()
    {
//        slots = Instantiate(PlaySlots, GameObject.FindGameObjectWithTag("Canvas").gameObject.transform);
       
        

        if (photonView.isMine)
        {
            PlaySlots = GameObject.FindGameObjectWithTag("PlayerSlots").gameObject;
            cl = (CardLsit)Resources.Load("Data/Card List");
            for (int i = 0; i < 5; i++)
            {
                int val = Random.Range(0, cl.CardList.Count);
                Debug.Log("Rand"+val);

                while (cl.CardList[val].Distributed == true)
                {
                    val = Random.Range(0, cl.CardList.Count);
                    Debug.Log("New Rand" + val);

                }
                if (cl.CardList[val].Distributed == false)
                {
                    Debug.Log("Matched"+val);
                    cl.CardList[val].Distributed = true;
                    Image sprite = PlaySlots.transform.GetChild(i).GetComponent<Image>();
                    CardDataHolder cd = PlaySlots.transform.GetChild(i).GetComponent<CardDataHolder>();
                    cd.SetVariables(true , (CardDataHolder.Number)cl.CardList[val].number , (CardDataHolder.CardType)cl.CardList[val].cardType , cl.CardList[val].Sprite);
                    cd.Sprite = sprite.sprite = cl.CardList[val].Sprite;
                }
                
                
            }
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
        else if(stream.isReading)
        {
            foreach (CardInfo item in cl.CardList)
            {
                item.Distributed = (bool)stream.ReceiveNext();

            }

        }
    }

}

