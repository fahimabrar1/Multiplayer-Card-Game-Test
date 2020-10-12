using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : Photon.MonoBehaviour 
{
    public string version;
    public UIManager ui;
    public Text ConnectionLogText;

    public static bool connected;

    private void Start()
    {

        connected = false;
        if (!PlayfabLogin.offline)
        {
            // StartCoroutine(ui.OpenLogPanel());

        }

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
        Text tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();
        tablepot.text = "0";
        ui.TurnInGameUIOff();
        ui.TurnUIOn();
        ui.OnLeaveGame();
        GameManager.ResumeMatch = false;
    }


    public virtual void OnDisconnectedFromPhoton()
    {

        ui.TurnInGameUIOff();
        ui.TurnUIOn();
        ui.OnLeaveGame();
        Debug.Log("Time to Scene Change");
        PlayerPrefs.DeleteKey(PlayfabLogin.MyUser.EMAIL.ToString());
        PlayerPrefs.DeleteKey(PlayfabLogin.MyUser.PASSWORD.ToString());
        SceneManager.LoadScene("SampleScene");
    }
   
    public virtual void OnPhotonRandomJoinFailed()
    {
        RoomOptions rm = new RoomOptions
        {
            maxPlayers = 5,
            isVisible = true,
            publishUserId = true,
            EmptyRoomTtl = 2000
            
        };
        int randID = Random.Range(0, 3000);
        PhotonNetwork.CreateRoom("Default"+randID , rm ,TypedLobby.Default);
        
    }


    public virtual void OnJoinedRoom()
    {
        /*  int[] ID = { 100, 200,300 ,400 ,500 };
          int i = 0;
          foreach (PhotonPlayer player in PhotonNetwork.playerList)
          {
              i
              i++;
          }*/

        /*if (!PhotonNetwork.player.customProperties.ContainsValue(ID5))
        {
            Debug.Log("Entry Id     :" + ID1);
            Hashtable hash = new Hashtable();
            hash.Add(PhotonNetwork.player.ID, ID5);
            PhotonNetwork.player.SetCustomProperties(hash);
        }*/

        
        ui.TurnInGameUIOn();
        ui.TurnUIOff();
        Debug.Log("Room Joined");
        Debug.Log(PhotonNetwork.player.UserId);
        if (PhotonNetwork.room.playerCount > 0)
        {

            if (PhotonNetwork.room.playerCount > 1)
            {
                GameManager.ResumeMatch = true;
                GameManager.NewRound = true;
                Debug.Log("ResumeMatch");
                Debug.Log("NewRound");
            }
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                //Id of each player
                GameManager.LivePlayerID.Add(player.ID);

                Debug.Log("player Name      :" + player.UserId);

                if (PhotonNetwork.player.UserId != player.UserId)
                {
                    ui.OtherPlayerActive(player.UserId);

                }
                if (!player.customProperties.ContainsKey(PhotonNetwork.player.ID))
                {
                    Debug.Log("player.ID    : " + player.ID);
                    Hashtable hash = new Hashtable();
                    hash.Add(player.UserId , player.ID );

                    player.SetCustomProperties(hash);
                }
            }
        }

        if (PhotonNetwork.room.maxPlayers !=5)
        {
            GameManager.BotAvailable = true;
            ui.OtherPlayerActive("bot");
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
                break;
            }
            yield return new WaitForSeconds(1);
            i++;
        }

        if (PhotonNetwork.room.playerCount == 1 && !GameManager.BotAvailable)
        {
            Debug.Log("Bot ");
            PhotonNetwork.room.maxPlayers--;
            
            GameObject obj  =   PhotonNetwork.Instantiate("Prefabs/CardPlayer", Vector2.zero, Quaternion.identity, 0);
            
            GameManager.BotAvailable = true;
            ui.OtherPlayerActive("bot");
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
        if (PhotonNetwork.room.playerCount > 1)
        {
            GameManager.ResumeMatch = true;
            GameManager.NewRound = true;

            PhotonNetwork.room.SetTurn(1);
            TableData tableData = (TableData)Resources.Load("Data/PlayerTable/Player Table");
            foreach (PlayerTurnData data in tableData._PlayerTurnData)
            {
                if (data.PlayerID == 1)
                {
                    data.PlayerActiveTurn = 1;
                    break;
                }
            }
            Debug.Log("OnPhotonPlayerConnected");
            Debug.Log("ResumeMatch");
            Debug.Log("NewRound");
        }
        
        ui.OtherPlayerActive(newPlayer.UserId);
        GameManager.LivePlayerID.Add(newPlayer.ID);
    }


    public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        
        if (PhotonNetwork.room.playerCount < 2)
        {
            Text tablepot = GameObject.FindGameObjectWithTag("TablePot").GetComponent<Text>();
            tablepot.text = "0";
            GameManager.ResumeMatch = false;
            GameManager.NewRound = false;
        }
        GameManager.LivePlayerID.Remove(otherPlayer.ID);
        ui.OtherPlayerInActive(otherPlayer.UserId);
        StartCoroutine(WaitForPlayer());

    }


}
