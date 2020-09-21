using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class PhotonManager : Photon.MonoBehaviour 
{
    public string version;
    public UIManager ui;
    public Text ConnectionLogText;

    public static bool connected;

    private void Start()
    {
        connected = false;
        StartCoroutine(ui.OpenLogPanel());

    }
   
  
    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings(version);
        
    }

    public void Logour()
    {
        PhotonNetwork.Disconnect();
        ConnectionLogText.text = PhotonNetwork.connectionStateDetailed.ToString();
        StartCoroutine(ui.OpenLogPanel());

    }

    public void JoinRoon()
    {
        PhotonPlayer player;
        
        PhotonNetwork.JoinRandomRoom();
    }


    public void OnLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    public void OnCreatedRoom()
    {
        Debug.Log("Room Created");
      
        ui.TurnInGameUIOn();
        ui.TurnUIOff();
    }



    public virtual void OnLeftRoom()
    {
        CardLsit cl = (CardLsit)Resources.Load("Data/Card List");
        foreach (CardInfo item in cl.CardList)
        {
            item.Distributed = false;
        }
        ui.TurnInGameUIOff();
        ui.TurnUIOn();
        ui.OtherPlayerInActive();
    }


    public virtual void OnDisconnectedFromPhoton()
    {

        ui.TurnInGameUIOff();
        ui.TurnUIOn();
        ui.OtherPlayerInActive();
        Debug.Log("Time to Scene Change");
        PlayerPrefs.DeleteKey(PlayfabLogin.MyUser.EMAIL.ToString());
        PlayerPrefs.DeleteKey(PlayfabLogin.MyUser.PASSWORD.ToString());
        SceneManager.LoadScene("SampleScene");
    }
   
    public virtual void OnPhotonRandomJoinFailed()
    {
        RoomOptions rm = new RoomOptions
        {
            maxPlayers = 2,
            isVisible = true,
            EmptyRoomTtl = 2000
            
        };
        int randID = Random.Range(0, 3000);
        PhotonNetwork.CreateRoom("Default"+randID , rm ,TypedLobby.Default);
        
    }


    public virtual void OnJoinedRoom()
    {
        ui.TurnInGameUIOn();
        ui.TurnUIOff();
        Debug.Log("Room Joined");
        if (PhotonNetwork.room.playerCount == PhotonNetwork.room.MaxPlayers)
        {
            ui.OtherPlayerActive();
        }

        StartCoroutine(WaitForPlayer());

        PhotonNetwork.Instantiate("Prefabs/CardPlayer", Vector2.zero, Quaternion.identity, 0);
        
    }

    private IEnumerator WaitForPlayer()
    {
        int i = 0;
        bool maxReached = false;
        while ( i<10)
        {
            if (PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers)
            {
                maxReached = true;
            }
            yield return new WaitForSeconds(1);
            i++;
        }

        if (!maxReached)
        {
            Debug.Log("Bot ");
            PhotonNetwork.room.maxPlayers--;
            GameObject obj  =   PhotonNetwork.Instantiate("Prefabs/CardPlayer", Vector2.zero, Quaternion.identity, 0);
            ui.OtherPlayerActive();
            obj.GetComponent<Bot>().enabled = true;
            obj.GetComponent<Player>().enabled = false;
        }

    }

    public virtual void OnConnectedToMaster()
    {
        ConnectionLogText.text = PhotonNetwork.connectionStateDetailed.ToString();

        if (PhotonNetwork.connectionStateDetailed.ToString().Equals("ConnectedToMaster") && connected == false )
        {
            connected = true;
            StartCoroutine(ui.CloseLogPanel());

        }
    }


    public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        ui.OtherPlayerActive();        
    }

    public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        ui.OtherPlayerInActive();
    }




    
   

  
  }
