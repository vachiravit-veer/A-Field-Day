using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

public class OnlineStateManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{

    [SerializeField]
    private Text _TurnText;
    [SerializeField]
    private GameObject _Time;
    [SerializeField]
    private Text _TimeText;
    [SerializeField]
    private int _currentID = 0;

    private PunTurnManager turnManager;

    private bool IsShowingResults,IsGameOver;
    public bool IsRolling;

    private SpawnTiles _spawnTiles;
    GUIController _gui;
    OnlineGameLogic _gameLogic;

    private List<int> disconnectedID;

    private int MaxSize;
    private int TotalPlayers;
    public void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        _spawnTiles = FindObjectOfType<SpawnTiles>();
        _gui = FindObjectOfType<GUIController>();
        _gameLogic = FindObjectOfType<OnlineGameLogic>();
        turnManager.TurnDuration = 30f;

        MaxSize = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"];
        if (MaxSize == 8)
            _spawnTiles.SpawnOld();
        else
            _spawnTiles.Spawn();


        TotalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        disconnectedID = new List<int>();

    }

    private void Update()
    {
        if(!PhotonNetwork.InRoom)
        {
            return;
        }

        if (IsGameOver)
            return;

       /* if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !IsGameOver)
        {
            IsGameOver = true;
            _gui.ShowWinner((int)PhotonNetwork.LocalPlayer.CustomProperties["customID"]);
            return;
        }
        */

        if (turnManager.Turn == 0)
        {
            
            // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
            if (PhotonNetwork.IsMasterClient)
            {
                if(SpawnTiles.TilesName.Length==MaxSize*MaxSize)
                    turnManager.BeginTurn();
                
            }
        }

        
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (this.turnManager.IsOver)
            {
                return;
            }

            float turnEnd = this.turnManager.RemainingSecondsInTurn;

            // check if we ran out of time, in which case we loose
            if (turnEnd < 0f && !IsShowingResults)
            {
                Debug.Log("Calling OnTurnCompleted with turnEnd =" + turnEnd);
                OnTurnCompleted(-1);
                return;
            }

            if (this._TurnText != null)
            {
                this._TurnText.text = this.turnManager.Turn.ToString();
            }

            if (this.turnManager.Turn > 0 && this._TimeText != null && !IsShowingResults && ((int)PhotonNetwork.LocalPlayer.CustomProperties["customID"] == _currentID))
            {

                this._TimeText.text = turnEnd.ToString("F0") + " SECONDS";
            }
        }


    }

    public void EndTurn()
    {
        turnManager.SendMove((byte)1, true);
    }


    #region TurnManager Callbacks
    public void OnTurnBegins(int turn)
    {
        
        Debug.Log("OnTurnBegins() turn: " + turn);

        IsShowingResults = false;

        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["customID"] == _currentID)
        {
            IsRolling = false;
            _Time.SetActive(true);
            IsShowingResults = false;
            _gameLogic.CheckAtStartTurn();
          
        }

    }


    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);

        //player = PhotonNetwork.LocalPlayer.GetNext();

       // turnManager.BeginTurn();
    }

    public void OnTurnTimeEnds(int obj)
    {
        CreateHighlight.Instance.HideHighlights();
        // not yet implemented.
        Debug.Log("OnTurnTimeEnds");
        OnTurnEnd();
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + player + " turn: " + turn + " action: " + move);
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {

        Debug.Log("OnTurnFinished: " + player + " turn: " + turn + " action: " + move);
        OnTurnEnd();
   
    }
    #endregion

    private void OnTurnEnd()
    {
        _Time.SetActive(false);     
        _gui.HidePlayerController();
        CheckWinner();
       
    }

    public override void OnPlayerLeftRoom(Player other)
    {

        disconnectedID.Add((int)other.CustomProperties["customID"]);

        if (PhotonNetwork.IsMasterClient)
        {
            List<int> tempDist = new List<int>();

            tempDist.Add(other.ActorNumber);

            Hashtable ht = new Hashtable();
            ht.Add("disconnected", ObjectSerializer.Serialize(tempDist));

            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
         
        }

    }

    private void CheckPlayerLeaveRoom()
    {


        object temp = PhotonNetwork.CurrentRoom.CustomProperties["disconnected"];
        if (temp == null)
        { 
            Debug.Log("no disconncted Player");
            return;
        }

        List<int> tempList = (List<int>)ObjectSerializer.Deserialize((byte[])temp);

        foreach(int id in tempList)
        {
            PhotonNetwork.DestroyPlayerObjects(id);
        }

        Hashtable ht = new Hashtable();
        ht.Add("disconnected", null);
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);


        Debug.Log("recvied data" + tempList.Count);

  
    }


    private void CheckWinner()
    {
        if(PhotonNetwork.IsMasterClient)
            CheckPlayerLeaveRoom();



        Player _lp = PhotonNetwork.LocalPlayer;
        if (MaxSize == 8)
        {
            if ((int)_lp.CustomProperties["home"] == 1 && (int)_lp.CustomProperties["rice"] == 2 && (int)_lp.CustomProperties["tree"] == 2 && (int)_lp.CustomProperties["water"] == 2)
            {
                IsGameOver = true;
                _gui.ShowWinner(_currentID);
                base.photonView.RPC("EndGame", RpcTarget.Others, _currentID);
                //game over
                return;
            }
        }
        else
        {
            if ((int)_lp.CustomProperties["home"] == 1 && (int)_lp.CustomProperties["rice"] == 1 && (int)_lp.CustomProperties["tree"] == 1 && (int)_lp.CustomProperties["water"] == 1)
            {
                IsGameOver = true;
                _gui.ShowWinner(_currentID);
                base.photonView.RPC("EndGame", RpcTarget.Others, _currentID);
                //game over
                return;
            }
        }


        //0  
        //1
        //2


        //_currentID = (_currentID + 1) % TotalPlayers;
        while(true)
        {
            _currentID = (_currentID + 1) % TotalPlayers;
            if (!disconnectedID.Contains(_currentID))
                break;
        }


       turnManager.BeginTurn();

    }



    [PunRPC]
    void EndGame(int id, PhotonMessageInfo info)
    {
        _gui.ShowWinner(id);
        IsGameOver = true;

    }

}
